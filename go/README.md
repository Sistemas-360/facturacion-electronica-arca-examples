# Facturación electrónica ARCA con Go

Ejemplo oficial para integrar una aplicación Go con la facturación electrónica de ARCA mediante la API REST de Sistemas 360.

La aplicación consume la API REST de Sistemas 360. La plataforma utiliza la configuración fiscal vinculada al token para gestionar la emisión electrónica y la autorización correspondiente mediante los servicios de ARCA.

## Descripción

Este ejemplo es una aplicación de consola en Go que permite validar la conexión, emitir comprobantes electrónicos autorizados por ARCA, consultar comprobantes existentes y descargar el PDF A4, integrando directamente con los endpoints oficiales de la API REST de facturación electrónica de Sistemas 360. Es un punto de partida adecuado para integrar un ERP, una plataforma SaaS, un sistema administrativo, un backend empresarial, un microservicio o un proceso interno con la emisión fiscal.

## Funcionalidades

- Validación de conexión y autenticación (`ping`).
- Creación de una Factura B con estructura completa de comprobante.
- Consulta de un comprobante por ID.
- Descarga segura del PDF A4.
- Autenticación mediante Bearer Token.
- Idempotencia mediante `referencia_externa`.
- Manejo de errores HTTP claro, con el código y el mensaje recibido.
- Timeout configurado en el cliente HTTP.
- Pruebas automatizadas con `httptest.Server`.

## Requisitos

- Go 1.21 o superior.
- Un token vinculado al contribuyente y al ambiente correspondiente (pruebas o producción).

No se utilizan dependencias externas: el ejemplo está construido exclusivamente con la biblioteca estándar de Go (`net/http`, `encoding/json`, `context`, `os`, `time`, `io`, `fmt`, `errors`, `strconv`, `path/filepath`).

## Estructura

```text
go/
├── README.md
├── go.mod
├── cmd/
│   └── facturacion/
│       └── main.go
└── internal/
    ├── client/
    │   ├── client.go
    │   └── client_test.go
    └── models/
        └── comprobante.go
```

- `cmd/facturacion`: aplicación de consola (punto de entrada, comandos y validación de argumentos).
- `internal/client`: cliente HTTP reutilizable para consumir la API de Sistemas 360.
- `internal/models`: estructuras tipadas para el cuerpo de las solicitudes.

## Variables de entorno

| Variable | Obligatoria | Valor predeterminado |
|---|---|---|
| `SISTEMAS360_TOKEN` | Sí | — |
| `SISTEMAS360_BASE_URL` | No | `https://api.sistemas360.ar` |

Si `SISTEMAS360_TOKEN` no está configurado, la aplicación muestra un error claro, finaliza con un código de salida distinto de cero y no realiza ninguna solicitud.

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

También puede ejecutarse directamente con `go run` sin un paso de compilación previo, como se muestra en los comandos siguientes.

## Validar la conexión

```bash
go run ./cmd/facturacion ping
```

Realiza:

```text
GET /api/ping
```

con los encabezados:

```text
Authorization: Bearer TU_TOKEN
Accept: application/json
```

y muestra el JSON recibido con formato legible.

## Crear una Factura B

```bash
go run ./cmd/facturacion crear
```

Realiza:

```text
POST /api/comprobantes
```

con los encabezados:

```text
Authorization: Bearer TU_TOKEN
Accept: application/json
Content-Type: application/json
```

El comando genera automáticamente la fecha actual (formato `YYYY-MM-DD`) y una `referencia_externa` de ejemplo para facilitar la ejecución manual. El cliente HTTP no realiza cálculos ocultos: la solicitud se envía con todos los valores explícitos, tal como se reciben.

## Consultar un comprobante

```bash
go run ./cmd/facturacion consultar 151
```

Realiza:

```text
GET /api/comprobantes/151
```

Reemplazá `151` por el ID real del comprobante. El comando valida que el ID esté presente, sea numérico y mayor que cero, y muestra el JSON completo recibido en la respuesta.

## Descargar el PDF A4

```bash
go run ./cmd/facturacion descargar-pdf 151
```

Realiza:

```text
GET /api/comprobantes/151/imprimir-a4
```

con el encabezado:

```text
Accept: application/pdf
```

El archivo se guarda como `comprobante-151.pdf`. La descarga se implementa de forma segura: se solicita el PDF, se valida el código HTTP y el `Content-Type` de la respuesta, se escribe en un archivo temporal, se cierra correctamente y recién entonces se renombra al nombre definitivo. Si ocurre un error en cualquier paso, el archivo temporal se elimina y no queda ningún archivo parcial.

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

## `referencia_externa`

En una integración real, `referencia_externa`:

- la genera el ERP, SaaS o sistema integrador, no el cliente HTTP;
- representa una operación comercial única;
- normalmente corresponde al ID de una venta, un pedido o una transacción;
- debe persistirse junto con esa operación;
- debe reutilizarse al reintentar la misma operación, sin generarse de nuevo en cada intento;
- permite que la API evite crear comprobantes duplicados ante reintentos.

## Manejo de errores

Ante una respuesta HTTP no exitosa, el cliente:

- lee el cuerpo de la respuesta con un límite razonable;
- conserva el código HTTP recibido;
- muestra el mensaje devuelto por la API, sea JSON o texto plano;
- no asume una única estructura de error.

Formato del mensaje:

```text
API respondió HTTP 422: ...
```

La aplicación también valida localmente, antes de realizar cualquier solicitud: comando inexistente, ID de comprobante faltante, ID no numérico, ID menor o igual a cero y token faltante. Los errores de red y los timeouts se reportan con un mensaje claro.

## Seguridad

El token debe permanecer en un backend seguro y cargarse únicamente desde variables de entorno. No debe exponerse en:

- frontend;
- aplicaciones móviles;
- repositorios;
- archivos versionados;
- capturas de pantalla;
- logs;
- respuestas de error.

El ejemplo nunca muestra el token ni registra el encabezado `Authorization`.

## Pruebas automatizadas

```bash
go test ./...
```

Las pruebas usan `httptest.Server` (biblioteca estándar) y cubren, entre otros casos: el encabezado `Authorization` con Bearer Token, el encabezado `Accept`, el encabezado `Content-Type` al crear un comprobante, un `ping` exitoso, la creación de un comprobante y el JSON enviado, la consulta por ID, la descarga del PDF y el nombre del archivo generado, respuestas de error en formato JSON y en texto plano, códigos HTTP no exitosos, un `Content-Type` inválido en la descarga del PDF y la limpieza del archivo temporal cuando la descarga falla.

## Ambientes de operación

La API permite trabajar con tokens asociados a un ambiente de pruebas o producción.

El ambiente de pruebas se utiliza para validar autenticación, solicitudes, respuestas, reglas de integración, idempotencia y descarga de documentos, antes de habilitar la operación productiva.

El ambiente de producción utiliza la configuración fiscal completa del contribuyente, incluyendo CUIT, punto de venta, certificado digital, clave privada y la autorización correspondiente del servicio en ARCA.

La configuración fiscal de cada ambiente se administra desde el panel de Sistemas 360.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api

## Repositorio oficial

https://github.com/Sistemas-360/facturacion-electronica-arca-examples
