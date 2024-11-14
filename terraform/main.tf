locals {
  jwt_issuer            = var.jwt_issuer
  jwt_aud               = var.jwt_aud
  docker_image          = var.api_docker_image
  aws_access_key        = var.api_access_key_id
  aws_secret_access_key = var.api_secret_access_key
  aws_region            = "us-east-1"
  jwt_signing_key       = var.jwt_signing_key
}

##############################
# NAMESPACE
##############################

resource "kubernetes_namespace" "payment" {
  metadata {
    name = "fiap-payment"
  }
}

##############################
# DATABASE
##############################

resource "aws_dynamodb_table" "payment_orders_replica" {
  name           = "Payment-OrdersReplica"
  billing_mode   = "PAY_PER_REQUEST"
  hash_key       = "Id"
  stream_enabled = false

  attribute {
    name = "Id"
    type = "S"
  }

  ttl {
    attribute_name = "ExpireAt"
  }
}

resource "aws_dynamodb_table" "payments_table" {
  name           = "Payments"
  billing_mode   = "PAY_PER_REQUEST"
  hash_key       = "Id"
  stream_enabled = false

  attribute {
    name = "Id"
    type = "S"
  }
  attribute {
    name = "ExternalReference"
    type = "S"
  }

  global_secondary_index {
    name            = "ExternalReference-index"
    hash_key        = "ExternalReference"
    projection_type = "ALL"
    write_capacity  = 1
    read_capacity   = 1
  }

  lifecycle {
    ignore_changes = [global_secondary_index]
  }
}

##############################
# CONFIGS/SECRETS
##############################

resource "kubernetes_config_map_v1" "config_map_api" {
  metadata {
    name      = "configmap-payment-api"
    namespace = kubernetes_namespace.payment.metadata[0].name
    labels = {
      "app"       = "payment-api"
      "terraform" = true
    }
  }
  data = {
    "ASPNETCORE_ENVIRONMENT"               = "Development"
    "MercadoPago__NotificationUrl"         = ""
    "Serilog__WriteTo__2__Args__serverUrl" = "http://api-internal.fiap-log.svc.cluster.local"
    "Serilog__Enrich__0"                   = "FromLogContext"
    "JwtOptions__Issuer"                   = local.jwt_issuer
    "JwtOptions__Audience"                 = local.jwt_aud
    "JwtOptions__ExpirationSeconds"        = 3600
    "JwtOptions__UseAccessToken"           = true
  }
}

resource "kubernetes_secret" "secret_api" {
  metadata {
    name      = "secret-payment-api"
    namespace = kubernetes_namespace.payment.metadata[0].name
    labels = {
      app         = "api-pod"
      "terraform" = true
    }
  }
  data = {
    "JwtOptions__SigningKey"     = local.jwt_signing_key
    "MercadoPago__WebhookSecret" = var.mercadopago_webhook_secret
    "MercadoPago__AccessToken"   = var.mercadopago_accesstoken
    "AWS_SECRET_ACCESS_KEY"      = local.aws_secret_access_key
    "AWS_ACCESS_KEY_ID"          = local.aws_access_key
    "AWS_REGION"                 = local.aws_region
  }
  type = "Opaque"
}

####################################
# API
####################################

resource "kubernetes_service" "payment-api-svc" {
  metadata {
    name      = "api-internal"
    namespace = kubernetes_namespace.payment.metadata[0].name
    annotations = {
      "service.beta.kubernetes.io/aws-load-balancer-type"   = "nlb"
      "service.beta.kubernetes.io/aws-load-balancer-scheme" = "internal"
    }
  }
  spec {
    port {
      port        = 80
      target_port = 8080
      node_port   = 30002
      protocol    = "TCP"
    }
    type = "LoadBalancer"
    selector = {
      app : "payment-api"
    }
  }
}

resource "kubernetes_deployment" "deployment_payment_api" {
  depends_on = [
    kubernetes_secret.secret_api,
    kubernetes_config_map_v1.config_map_api,
    # aws_dynamodb_table.payments_table,
    # aws_dynamodb_table.payment_orders_replica
  ]

  metadata {
    name      = "deployment-payment-api"
    namespace = kubernetes_namespace.payment.metadata[0].name
    labels = {
      app         = "payment-api"
      "terraform" = true
    }
  }
  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "payment-api"
      }
    }
    template {
      metadata {
        name = "pod-payment-api"
        labels = {
          app         = "payment-api"
          "terraform" = true
        }
      }
      spec {
        automount_service_account_token = false
        container {
          name  = "payment-api-container"
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
              name = "configmap-payment-api"
            }
          }
          env_from {
            secret_ref {
              name = "secret-payment-api"
            }
          }
        }
      }
    }
  }
}

resource "kubernetes_horizontal_pod_autoscaler_v2" "hpa_api" {
  metadata {
    name      = "hpa-payment-api"
    namespace = kubernetes_namespace.payment.metadata[0].name
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
        container = "payment-api-container"
        name      = "cpu"
        target {
          average_utilization = 65
          type                = "Utilization"
        }
      }
    }
  }
}
