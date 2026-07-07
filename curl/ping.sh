#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${SISTEMAS360_BASE_URL:-https://api.sistemas360.ar}"
TOKEN="${SISTEMAS360_TOKEN:-}"

if [[ -z "$TOKEN" ]]; then
    echo "Error: debés configurar SISTEMAS360_TOKEN." >&2
    exit 1
fi

curl \
    --fail-with-body \
    --silent \
    --show-error \
    -X GET "${BASE_URL}/api/ping" \
    -H "Authorization: Bearer ${TOKEN}" \
    -H "Accept: application/json"

echo
