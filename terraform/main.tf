##############################
# EKS CLUSTER
##############################

data "aws_eks_cluster" "techchallenge_cluster" {
  name = var.eks_cluster_name
}

##############################
# DATABASE
##############################

locals {
  jwt_issuer                  = var.jwt_issuer
  jwt_aud                     = var.jwt_aud
  docker_image                = var.api_docker_image
}


##############################
# CONFIGS/SECRETS
##############################

resource "kubernetes_namespace" "payment" {
  metadata {
    name = "payment"
  }
}

resource "kubernetes_config_map_v1" "config_map_api" {
  metadata {
    name = "configmap-api"
    namespace = "payment"
    labels = {
      "app"       = "api"
      "terraform" = true
    }
  }
  data = {
    "ASPNETCORE_ENVIRONMENT"               = "Development"
    "Serilog__WriteTo__2__Args__serverUrl" = "http://svc-seq:80"
    "Serilog__WriteTo__2__Args__formatter" = "Serilog.Formatting.Json.JsonFormatter, Serilog"
    "MercadoPago__NotificationUrl"         = ""
    "Serilog__Enrich__0"                   = "FromLogContext"
    "HybridCache__Expiration"              = "01:00:00"
    "HybridCache__LocalCacheExpiration"    = "01:00:00"
    "HybridCache__Flags"                   = "DisableDistributedCache"
    "JwtOptions__Issuer"                   = local.jwt_issuer
    "JwtOptions__Audience"                 = local.jwt_aud
    "JwtOptions__SigningKey"               = var.jwt_signing_key
    "JwtOptions__ExpirationSeconds"        = 3600
    "JwtOptions__UseAccessToken"           = true
    # "SqsSettings__QueueName"               = local.events_queue_name
    "SqsSettings__Enabled"                 = true
    "SqsSettings__Region"                  = "us-east-1"
    "SqsSettings__ClientId"                = var.api_access_key_id
    "SqsSettings__ClientSecret"            = var.api_secret_access_key
  }
}

resource "kubernetes_secret" "secret_mercadopago" {
  metadata {
    name = "secret-mercadopago"
    namespace = "payment"
    labels = {
      app         = "api-pod"
      "terraform" = true
    }
  }
  data = {
    "MercadoPago__WebhookSecret" = var.mercadopago_webhook_secret
    "MercadoPago__AccessToken"   = var.mercadopago_accesstoken
  }
  type = "Opaque"
}

####################################
# API
####################################


resource "kubernetes_service" "payment-api-svc" {
  metadata {
    name = var.internal_elb_name
    namespace = "payment"
    annotations = {
      "service.beta.kubernetes.io/aws-load-balancer-type"   = "nlb"
      "service.beta.kubernetes.io/aws-load-balancer-scheme" = "internal"
    }
  }
  spec {
    port {
      port        = 80
      target_port = 8080
      node_port   = 30000
      protocol    = "TCP"
    }
    type = "LoadBalancer"
    selector = {
      app : "api"
    }
  }
}

resource "kubernetes_deployment" "deployment_payment_api" {
  depends_on = [kubernetes_secret.secret_mercadopago, kubernetes_config_map_v1.config_map_api]
  metadata {
    name = "deployment-payment-api"
    namespace = "payment"
    labels = {
      app         = "api"
      "terraform" = true
    }
  }
  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "api"
      }
    }
    template {
      metadata {
        name = "pod-api"
        labels = {
          app         = "api"
          "terraform" = true
        }
      }
      spec {
        automount_service_account_token = false
        container {
          name  = "api-container"
          image = local.docker_image
          port {
            name           = "liveness-port"
            container_port = 8080
          }
          port {
            container_port = 80
          }

          image_pull_policy = "IfNotPresent"
          liveness_probe {
            http_get {
              path = "/healthz"
              port = "liveness-port"
            }
            period_seconds        = 10
            failure_threshold     = 3
            initial_delay_seconds = 20
          }
          readiness_probe {
            http_get {
              path = "/healthz"
              port = "liveness-port"
            }
            period_seconds        = 10
            failure_threshold     = 3
            initial_delay_seconds = 10
          }

          resources {
            requests = {
              cpu    = "100m"
              memory = "120Mi"
            }
            limits = {
              cpu    = "150m"
              memory = "200Mi"
            }
          }
          env_from {
            config_map_ref {
              name = "configmap-api"
            }
          }
          env_from {
            secret_ref {
              name = "secret-mercadopago"
            }
          }
        }
      }
    }
  }
}

resource "kubernetes_horizontal_pod_autoscaler_v2" "hpa_api" {
  metadata {
    name = "hpa-api"
    namespace = "payment"
  }
  spec {
    max_replicas = 3
    min_replicas = 1
    scale_target_ref {
      api_version = "apps/v1"
      kind        = "Deployment"
      name        = "deployment-payment-api"
    }

    metric {
      type = "ContainerResource"
      container_resource {
        container = "api-container"
        name      = "cpu"
        target {
          average_utilization = 65
          type                = "Utilization"
        }
      }
    }
  }
}

#################################
# SEQ
#################################

resource "kubernetes_service" "svc_seq" {
  metadata {
    name = "svc-seq"
    namespace = "payment"
    labels = {
      "terraform" = true
    }
  }
  spec {
    type = "NodePort"
    port {
      port      = 80
      node_port = 30008
    }
    selector = {
      app = "seq"
    }
  }
}

resource "kubernetes_deployment" "deployment_seq" {
  metadata {
    name = "deployment-seq"
    namespace = "payment"
    labels = {
      app         = "seq"
      "terraform" = true
    }
  }
  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "seq"
      }
    }
    template {
      metadata {
        name = "pod-seq"
        labels = {
          app         = "seq"
          "terraform" = true
        }
      }
      spec {
        automount_service_account_token = false
        container {
          name  = "seq-container"
          image = "datalust/seq:latest"
          port {
            container_port = 80
          }
          image_pull_policy = "IfNotPresent"
          env {
            name  = "ACCEPT_EULA"
            value = "Y"
          }
          resources {
            requests = {
              cpu    = "50m"
              memory = "120Mi"
            }
            limits = {
              cpu    = "100m"
              memory = "220Mi"
            }
          }
        }
        volume {
          name = "dashboards-volume"
          host_path {
            path = "/home/docker/seq"
          }
        }
      }
    }
  }
}