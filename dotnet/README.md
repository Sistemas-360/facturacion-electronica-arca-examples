# API de facturación electrónica ARCA con C# y .NET

Ejemplos de referencia para integrar la API REST de facturación electrónica desde C# y .NET.

## Ejemplos disponibles

| Proyecto | Uso |
|---|---|
| `console/` | Cliente de consola para validar conexión, crear, consultar y descargar PDF A4 |
| `aspnet-core/` | Backend ASP.NET Core que actúa como proxy integrador hacia la API oficial |

## Requisitos

- .NET 10 SDK o superior compatible.
- Bearer Token de ambiente de pruebas o ambiente de producción.

No se utilizan paquetes externos en ninguno de los dos proyectos.

## Estructura

```text
dotnet/
|-- README.md
|-- console/
|   |-- Sistemas360Example.Console.csproj
|   |-- Program.cs
|   |-- Client/
|   |   `-- Sistemas360Client.cs
|   `-- Models/
|       |-- ClienteRequest.cs
|       |-- ItemRequest.cs
|       `-- CrearComprobanteRequest.cs
`-- aspnet-core/
    |-- Sistemas360Example.Api.csproj
    |-- Program.cs
    |-- appsettings.json
    |-- appsettings.Development.json
    |-- Controllers/
    |   `-- ComprobantesController.cs
    |-- Services/
    |   |-- ISistemas360Service.cs
    |   `-- Sistemas360Service.cs
    |-- Client/
    |   `-- Sistemas360Client.cs
    |-- Configuration/
    |   `-- Sistemas360Options.cs
    `-- Models/
        |-- ClienteRequest.cs
        |-- ItemRequest.cs
        `-- CrearComprobanteRequest.cs
```

## Consola

Cliente de consola para probar la integración desde terminal.

### Configuración

Linux o macOS:

```bash
export SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
export SISTEMAS360_TOKEN="TU_TOKEN"
```

Windows PowerShell:

```powershell
$env:SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
$env:SISTEMAS360_TOKEN="TU_TOKEN"
```

### Comandos

| Comando | Endpoint oficial |
|---|---|
| `dotnet run --project console -- ping` | `GET /api/ping` |
| `dotnet run --project console -- crear` | `POST /api/comprobantes` |
| `dotnet run --project console -- consultar 151` | `GET /api/comprobantes/{id}` |
| `dotnet run --project console -- descargar-pdf 151` | `GET /api/comprobantes/{id}/imprimir-a4` |

Reemplazá `151` por el ID real del comprobante. El PDF se guarda como `comprobante-151.pdf`.

La operación `crear` genera automáticamente una `referencia_externa` de ejemplo para la prueba manual. En una integración real, ese valor debe generarlo y persistirlo el backend integrador.

## ASP.NET Core

Backend de ejemplo para encapsular el Bearer Token y exponer endpoints locales propios.

### Diferencia entre la API local y la API oficial

El proyecto ASP.NET Core expone endpoints locales bajo `/api/facturacion`. Esos endpoints pertenecen solo al backend de ejemplo y no reemplazan la API oficial.

Flujo:

```text
Cliente o sistema integrador
  -> Backend ASP.NET Core local
  -> API de facturación electrónica de Sistemas 360
  -> ARCA
```

### Endpoints locales

```text
GET  /api/facturacion/ping
POST /api/facturacion/comprobantes
GET  /api/facturacion/comprobantes/{id}
GET  /api/facturacion/comprobantes/{id}/pdf
```

### Endpoints oficiales consumidos internamente

```text
GET  https://api.sistemas360.ar/api/ping
POST https://api.sistemas360.ar/api/comprobantes
GET  https://api.sistemas360.ar/api/comprobantes/{id}
GET  https://api.sistemas360.ar/api/comprobantes/{id}/imprimir-a4
```

### Configurar token

Linux o macOS:

```bash
export Sistemas360__Token="TU_TOKEN"
```

Windows PowerShell:

```powershell
$env:Sistemas360__Token="TU_TOKEN"
```

Opcionalmente, podés sobreescribir la URL base:

Linux o macOS:

```bash
export Sistemas360__BaseUrl="https://api.sistemas360.ar"
```

Windows PowerShell:

```powershell
$env:Sistemas360__BaseUrl="https://api.sistemas360.ar"
```

### Ejecutar

```bash
dotnet run --project aspnet-core
```

### Crear una Factura B

El endpoint local `POST /api/facturacion/comprobantes` recibe la misma estructura JSON del endpoint oficial `POST /api/comprobantes`.

```json
{
  "tipo_comprobante": "factura_b",
  "concepto": "productos",
  "fecha": "2026-07-07",
  "referencia_externa": "venta_erp_000001",
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

Ejemplo con cURL contra el servidor local:

```bash
curl -X POST "http://localhost:5000/api/facturacion/comprobantes" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo_comprobante": "factura_b",
    "concepto": "productos",
    "fecha": "2026-07-07",
    "referencia_externa": "venta_erp_000001",
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

La URL y el puerto locales pueden variar según la configuración de ASP.NET Core.

### Consultar un comprobante

```bash
curl "http://localhost:5000/api/facturacion/comprobantes/151"
```

### Descargar el PDF A4

```bash
curl "http://localhost:5000/api/facturacion/comprobantes/151/pdf" --output comprobante-151.pdf
```

### Manejo de errores

Ante un error, el backend local devuelve JSON:

```json
{
  "ok": false,
  "error": "Mensaje del error"
}
```

- `400`: datos inválidos en la solicitud.
- `502` o código remoto original: error devuelto por la API externa.
- `500`: error interno sin exponer stack trace ni credenciales.

## Idempotencia

`referencia_externa` identifica una operación única del sistema integrador.

- Debe generarla el backend que origina la operación.
- Debe persistirse para poder reintentar la misma operación.
- No debe regenerarse en cada reintento.

Si se reenvía la misma referencia para el mismo emisor, la API devuelve el comprobante existente y evita duplicados.

## Seguridad

El token pertenece al backend. No debe exponerse en frontend, aplicaciones móviles, repositorios públicos, logs ni capturas.

El ejemplo ASP.NET Core encapsula el Bearer Token del lado servidor y no lo devuelve en respuestas ni errores.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Los proyectos incluidos cubren ping, creación, consulta y descarga de PDF A4. Son puntos de partida para un backend integrador. Los casos avanzados, como reintento técnico o PDF ticket, están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
