terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~>5.62.0"
    }
  }
  required_version = "~>1.9.4"
}

provider "aws" {
  profile = var.profile
  region  = var.region
  alias   = "us-east-1"

  default_tags {
    tags = {
      ManagedBy = "Terraform"
    }
  }
}


# provider "kubernetes" {
#   config_path    = "C:\\Users\\italo\\.kube\\config"
#   config_context = "minikube"
# }

provider "kubernetes" {
  host                   = data.aws_eks_cluster.techchallenge_cluster.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.techchallenge_cluster.certificate_authority[0].data)
  exec {
    api_version = "client.authentication.k8s.io/v1beta1"
    command     = "aws"
    args = ["eks", "get-token", "--cluster-name", data.aws_eks_cluster.techchallenge_cluster.name]
  }
}