#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${SISTEMAS360_BASE_URL:-https://api.sistemas360.ar}"
TOKEN="${SISTEMAS360_TOKEN:-}"

if [[ -z "$TOKEN" ]]; then
    echo "Error: debés configurar SISTEMAS360_TOKEN." >&2
    exit 1
fi

FECHA="$(date +%Y-%m-%d)"
REFERENCIA="venta_curl_$(date +%s)_$$"

PAYLOAD=$(cat <<JSON
{
  "tipo_comprobante": "factura_b",
  "concepto": "productos",
  "fecha": "${FECHA}",
  "referencia_externa": "${REFERENCIA}",
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
JSON
)

curl \
    --fail-with-body \
    --silent \
    --show-error \
    -X POST "${BASE_URL}/api/comprobantes" \
    -H "Authorization: Bearer ${TOKEN}" \
    -H "Accept: application/json" \
    -H "Content-Type: application/json" \
    --data "$PAYLOAD"

echo
