variable "environment" {
  description = "Environment name (development or production)"
  type        = string
  default     = "development"
}

variable "image" {
  description = "Docker image URI for the API"
  type        = string
}

variable "namespace" {
  description = "Kubernetes namespace"
  type        = string
  default     = "oficina"
}

variable "minikube_profile" {
  description = "Minikube profile name"
  type        = string
  default     = "dev"
}
