resource "kubectl_manifest" "sqlserver" {
  yaml_body = file("../k8s/sqlserver-deployment.yaml")
}