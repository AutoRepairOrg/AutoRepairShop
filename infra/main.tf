resource "kubectl_manifest" "k8s" {
  for_each  = fileset("../k8s", "*.yaml")
  yaml_body = file("../k8s/${each.value}")
}

# Aplique TODOS os YAMLs da pasta k8s/