#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${SISTEMAS360_BASE_URL:-https://api.sistemas360.ar}"
TOKEN="${SISTEMAS360_TOKEN:-}"
COMPROBANTE_ID="${1:-}"

if [[ -z "$TOKEN" ]]; then
    echo "Error: debés configurar SISTEMAS360_TOKEN." >&2
    exit 1
fi

if [[ -z "$COMPROBANTE_ID" ]]; then
    echo "Uso: ./consultar-comprobante.sh ID_COMPROBANTE" >&2
    exit 1
fi

if [[ ! "$COMPROBANTE_ID" =~ ^[0-9]+$ ]]; then
    echo "Error: el ID del comprobante debe ser un número entero." >&2
    exit 1
fi

curl \
    --fail-with-body \
    --silent \
    --show-error \
    -X GET "${BASE_URL}/api/comprobantes/${COMPROBANTE_ID}" \
    -H "Authorization: Bearer ${TOKEN}" \
    -H "Accept: application/json"

echo
