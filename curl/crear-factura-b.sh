#!/usr/bin/env bash
#
# Crea una Factura B de ejemplo.
# Endpoint: POST /api/comprobantes

set -euo pipefail

SISTEMAS360_BASE_URL="${SISTEMAS360_BASE_URL:-https://api.sistemas360.ar}"

if [[ -z "${SISTEMAS360_TOKEN:-}" ]]; then
    echo "Error: falta la variable de entorno SISTEMAS360_TOKEN." >&2
    echo "Exportá tu token antes de ejecutar este script, por ejemplo:" >&2
    echo "  export SISTEMAS360_TOKEN=TU_TOKEN" >&2
    exit 1
fi

fecha_actual=$(date +%Y-%m-%d)
referencia_externa="venta_curl_$(date +%s)_$$"

payload=$(cat <<EOF
{
  "tipo_comprobante": "factura_b",
  "concepto": "productos",
  "fecha": "${fecha_actual}",
  "referencia_externa": "${referencia_externa}",
  "cliente": {
    "documento_tipo": "dni",
    "documento_numero": "30111222",
    "razon_social": "Cliente Demo",
    "condicion_iva_receptor_id": 5
  },
  "items": [
    {
      "descripcion": "Producto de ejemplo",
      "cantidad": 1,
      "precio_unitario": 10000,
      "tipo_impuesto": "gravado",
      "iva": 21
    }
  ],
  "total": 12100,
  "moneda": "PES"
}
EOF
)

response=$(curl -sS -w '\n%{http_code}' \
    -X POST "${SISTEMAS360_BASE_URL}/api/comprobantes" \
    -H "Authorization: Bearer ${SISTEMAS360_TOKEN}" \
    -H "Accept: application/json" \
    -H "Content-Type: application/json" \
    -d "${payload}")

http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if (( http_code < 200 || http_code >= 300 )); then
    echo "Error: la API respondió con código HTTP ${http_code}" >&2
    echo "$body" >&2
    exit 1
fi

echo "$body"
