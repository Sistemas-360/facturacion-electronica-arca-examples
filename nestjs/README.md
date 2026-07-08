# API de facturación electrónica ARCA con NestJS

Backend de referencia con NestJS para encapsular el Bearer Token y exponer endpoints locales propios.

## Requisitos

- Node.js 18 o superior.
- Bearer Token de ambiente de pruebas o ambiente de producción.

## Instalación

Entrá a la carpeta:

```bash
cd nestjs
```

Instalá las dependencias:

```bash
npm install
```

Copiá el archivo de variables:

```bash
cp .env.example .env
```

Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Configurá `.env`:

```env
SISTEMAS360_BASE_URL=https://api.sistemas360.ar
SISTEMAS360_TOKEN=TU_TOKEN
```

## Ejecutar

```bash
npm run start:dev
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
curl -X POST "http://localhost:3000/api/facturacion/comprobantes" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo_comprobante": "factura_b",
    "concepto": "productos",
    "fecha": "2026-07-07",
    "referencia_externa": "venta_nest_000001",
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

`referencia_externa` debe generarla el backend integrador y reutilizarse al reintentar la misma operación. Este ejemplo espera ese valor en el body recibido.

## Seguridad

El token queda del lado del backend NestJS y no debe exponerse al frontend ni a clientes móviles.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4 desde un backend NestJS propio. Los casos avanzados están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
