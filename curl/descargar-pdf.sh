#!/usr/bin/env bash
#
# Descarga el PDF A4 de un comprobante.
# Endpoint: GET /api/comprobantes/{id}/imprimir-a4
#
# Uso:
#   ./descargar-pdf.sh <comprobante_id>
#   COMPROBANTE_ID=59 ./descargar-pdf.sh

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

output_file="comprobante-${comprobante_id}.pdf"

http_code=$(curl -sS -w '%{http_code}' -o "$output_file" \
    -X GET "${SISTEMAS360_BASE_URL}/api/comprobantes/${comprobante_id}/imprimir-a4" \
    -H "Authorization: Bearer ${SISTEMAS360_TOKEN}" \
    -H "Accept: application/pdf")

if (( http_code < 200 || http_code >= 300 )); then
    echo "Error: la API respondió con código HTTP ${http_code}" >&2
    cat "$output_file" >&2
    rm -f "$output_file"
    exit 1
fi

echo "PDF guardado en: ${output_file}"
