variable "rds_cluster_identifier" {
  type    = string
  default = "techchallenge-mysql-default"
}

variable "profile" {
  description = "AWS profile name"
  type        = string
  default     = "default"
}

variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "eks_cluster_name" {
  type    = string
  default = "quixada"
}

variable "mercadopago_webhook_secret" {
  type      = string
  sensitive = true
}

variable "mercadopago_accesstoken" {
  type      = string
  sensitive = true
}

variable "jwt_signing_key" {
  type      = string
  sensitive = true
}

variable "jwt_issuer" {
  type      = string
  sensitive = false
  default   = "https://localhost:7004"
}

variable "jwt_aud" {
  type      = string
  sensitive = false
  default   = "https://localhost:7004"
}

variable "api_docker_image" {
  type    = string
  default = "ghcr.io/soat-fiap/bmb.payment/api:1.0.0"
}

variable "internal_elb_name" {
  type    = string
  default = "payment-api-internal-elb"
}

variable "api_access_key_id" {
  type      = string
  nullable  = false
  sensitive = true
}

variable "api_secret_access_key" {
  type      = string
  nullable  = false
  sensitive = true
}