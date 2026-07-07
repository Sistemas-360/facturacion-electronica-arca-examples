#!/usr/bin/env bash
#
# Consulta un comprobante por su ID.
# Endpoint: GET /api/comprobantes/{id}
#
# Uso:
#   ./consultar-comprobante.sh <comprobante_id>
#   COMPROBANTE_ID=59 ./consultar-comprobante.sh

set -euo pipefail

SISTEMAS360_BASE_URL="${SISTEMAS360_BASE_URL:-https://api.sistemas360.ar}"

if [[ -z "${SISTEMAS360_TOKEN:-}" ]]; then
    echo "Error: falta la variable de entorno SISTEMAS360_TOKEN." >&2
    echo "Exportá tu token antes de ejecutar este script, por ejemplo:" >&2
    echo "  export SISTEMAS360_TOKEN=TU_TOKEN" >&2
    exit 1
fi

comprobante_id="${1:-${COMPROBANTE_ID:-}}"

if [[ -z "$comprobante_id" ]]; then
    echo "Error: falta el ID del comprobante." >&2
    echo "Uso: $0 <comprobante_id>" >&2
    echo "O definí la variable de entorno COMPROBANTE_ID." >&2
    exit 1
fi

response=$(curl -sS -w '\n%{http_code}' \
    -X GET "${SISTEMAS360_BASE_URL}/api/comprobantes/${comprobante_id}" \
    -H "Authorization: Bearer ${SISTEMAS360_TOKEN}" \
    -H "Accept: application/json")

http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if (( http_code < 200 || http_code >= 300 )); then
    echo "Error: la API respondió con código HTTP ${http_code}" >&2
    echo "$body" >&2
    exit 1
fi

echo "$body"
