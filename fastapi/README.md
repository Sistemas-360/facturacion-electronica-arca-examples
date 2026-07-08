# API de facturación electrónica ARCA con FastAPI

Backend de referencia con FastAPI para encapsular el Bearer Token y exponer endpoints locales propios.

## Requisitos

- Python 3.10 o superior.
- Bearer Token de ambiente de pruebas o ambiente de producción.

## Instalación

Entrá a la carpeta:

```bash
cd fastapi
```

Creá un entorno virtual:

```bash
python -m venv .venv
```

Linux o macOS:

```bash
source .venv/bin/activate
pip install -r requirements.txt
cp .env.example .env
```

Windows PowerShell:

```powershell
.venv\Scripts\Activate.ps1
pip install -r requirements.txt
Copy-Item .env.example .env
```

Configurá `.env`:

```env
SISTEMAS360_BASE_URL=https://api.sistemas360.ar
SISTEMAS360_TOKEN=TU_TOKEN
```

## Ejecutar

```bash
uvicorn app.main:app --reload
```

## Endpoints locales

| Endpoint local | Endpoint oficial |
|---|---|
| `GET /api/facturacion/ping` | `GET /api/ping` |
| `POST /api/facturacion/comprobantes` | `POST /api/comprobantes` |
| `GET /api/facturacion/comprobantes/{id}` | `GET /api/comprobantes/{id}` |
| `GET /api/facturacion/comprobantes/{id}/pdf` | `GET /api/comprobantes/{id}/imprimir-a4` |

## Ejemplo de creación

```bash
curl -X POST "http://127.0.0.1:8000/api/facturacion/comprobantes" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo_comprobante": "factura_b",
    "concepto": "productos",
    "fecha": "2026-07-07",
    "referencia_externa": "venta_fastapi_000001",
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
  }'
```

## Idempotencia

`referencia_externa` debe generarla el backend integrador y reutilizarse al reintentar la misma operación. Este ejemplo no la inventa dentro del endpoint local: espera recibirla en el JSON enviado.

## Seguridad

El token queda del lado del backend FastAPI y no debe exponerse al frontend ni a clientes móviles.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4 desde un backend HTTP propio. Los casos avanzados están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
