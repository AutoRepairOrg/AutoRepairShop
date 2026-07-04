#!/usr/bin/env bash
set -euo pipefail

CONTEXT="${1:-dev}"
NAMESPACE="${2:-oficina}"
API_LOCAL_PORT="${3:-8080}"
SQL_LOCAL_PORT="${4:-1433}"

cleanup() {
  echo
  echo "Stopping port-forwards..."
  if [[ -n "${API_PID:-}" ]] && kill -0 "$API_PID" 2>/dev/null; then
    kill "$API_PID" 2>/dev/null || true
  fi
  if [[ -n "${SQL_PID:-}" ]] && kill -0 "$SQL_PID" 2>/dev/null; then
    kill "$SQL_PID" 2>/dev/null || true
  fi
}

trap cleanup INT TERM EXIT

echo "Using context: $CONTEXT"
kubectl config use-context "$CONTEXT" >/dev/null

echo "Checking required services in namespace: $NAMESPACE"
kubectl -n "$NAMESPACE" get svc api sqlserver >/dev/null

echo "Starting API port-forward on localhost:$API_LOCAL_PORT -> svc/api:80"
kubectl -n "$NAMESPACE" port-forward svc/api "$API_LOCAL_PORT:80" &
API_PID=$!

echo "Starting SQL Server port-forward on localhost:$SQL_LOCAL_PORT -> svc/sqlserver:1433"
kubectl -n "$NAMESPACE" port-forward svc/sqlserver "$SQL_LOCAL_PORT:1433" &
SQL_PID=$!

echo "Port-forwards running. Press Ctrl+C to stop both."

wait "$API_PID" "$SQL_PID"
