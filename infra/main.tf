locals {
  yaml_files = fileset("${path.module}/../k8s", "*.yaml")
}

resource "kubectl_manifest" "k8s" {
  for_each = local.yaml_files

  yaml_body = templatefile(
    "${path.module}/../k8s/${each.value}",
    {
      image = var.image
    }
  )

  depends_on = [
    kubernetes_namespace.oficina
  ]
}