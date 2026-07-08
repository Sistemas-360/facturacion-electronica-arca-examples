# API de facturación electrónica ARCA con Go

Implementación de referencia para integrar un backend Go con la API REST de facturación electrónica.

## Descripción

Este ejemplo de consola permite validar la conexión, crear comprobantes electrónicos, consultar comprobantes existentes y descargar el PDF A4 usando los endpoints oficiales de la API.

Es un punto de partida para ERP, SaaS, sistemas administrativos, workers, microservicios o cualquier backend integrador que necesite emitir comprobantes electrónicos autorizados por ARCA.

## Funcionalidades

- Validación de conexión y autenticación con `ping`.
- Creación de una Factura B con estructura completa de comprobante.
- Consulta de un comprobante por ID.
- Descarga segura del PDF A4.
- Autenticación mediante Bearer Token.
- Idempotencia mediante `referencia_externa`.
- Manejo claro de errores HTTP.
- Timeout configurado en el cliente HTTP.
- Pruebas automatizadas con `httptest.Server`.

## Requisitos

- Go 1.21 o superior.
- Bearer Token de ambiente de pruebas o ambiente de producción.

No se utilizan dependencias externas.

## Estructura

```text
go/
|-- README.md
|-- go.mod
|-- cmd/
|   `-- facturacion/
|       `-- main.go
`-- internal/
    |-- client/
    |   |-- client.go
    |   `-- client_test.go
    `-- models/
        `-- comprobante.go
```

## Variables de entorno

| Variable | Obligatoria | Valor predeterminado |
|---|---|---|
| `SISTEMAS360_TOKEN` | Sí | Sin valor |
| `SISTEMAS360_BASE_URL` | No | `https://api.sistemas360.ar` |

Si `SISTEMAS360_TOKEN` no está configurado, la aplicación finaliza con error sin realizar ninguna solicitud.

### Configuración en Linux o macOS

```bash
export SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
export SISTEMAS360_TOKEN="TU_TOKEN"
```

### Configuración en Windows PowerShell

```powershell
$env:SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
$env:SISTEMAS360_TOKEN="TU_TOKEN"
```

## Compilación

```bash
cd go
go build ./...
```

## Comandos disponibles

| Comando | Endpoint oficial | Descripción |
|---|---|---|
| `go run ./cmd/facturacion ping` | `GET /api/ping` | Valida conexión, token y ambiente |
| `go run ./cmd/facturacion crear` | `POST /api/comprobantes` | Crea una Factura B de ejemplo |
| `go run ./cmd/facturacion consultar 151` | `GET /api/comprobantes/{id}` | Consulta un comprobante |
| `go run ./cmd/facturacion descargar-pdf 151` | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 |

## Validar la conexión

```bash
go run ./cmd/facturacion ping
```

## Crear una Factura B

```bash
go run ./cmd/facturacion crear
```

El comando genera automáticamente:

- la fecha actual;
- una `referencia_externa` única de ejemplo;
- una Factura B de referencia.

## Consultar un comprobante

```bash
go run ./cmd/facturacion consultar 151
```

Reemplazá `151` por el ID real del comprobante.

## Descargar el PDF A4

```bash
go run ./cmd/facturacion descargar-pdf 151
```

El archivo se guarda como `comprobante-151.pdf`.

## Estructura JSON

El comando `crear` envía una estructura equivalente a:

```json
{
  "tipo_comprobante": "factura_b",
  "concepto": "productos",
  "fecha": "2026-07-07",
  "referencia_externa": "venta_go_000001",
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
```

## Idempotencia

`referencia_externa` identifica una operación única.

- Debe generarla el backend integrador.
- Debe persistirse junto con la operación comercial.
- Debe reutilizarse cuando se repite la misma operación.

En este ejemplo se genera automáticamente solo para facilitar la prueba manual.

## Manejo de errores

Ante una respuesta HTTP no exitosa, el cliente conserva el código HTTP y el mensaje devuelto por la API, sin asumir una única estructura de error.

También valida localmente:

- comando inexistente;
- ID faltante;
- ID no numérico;
- ID menor o igual a cero;
- token faltante.

## Seguridad

El token pertenece al backend y debe cargarse desde variables de entorno. No debe exponerse en frontend, aplicaciones móviles, repositorios, logs ni respuestas de error.

## Pruebas automatizadas

```bash
go test ./...
```

Las pruebas cubren autenticación Bearer, encabezados HTTP, creación de comprobantes, consulta por ID, descarga de PDF A4 y distintos escenarios de error.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4. Es un punto de partida para un backend integrador. Los casos avanzados están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
