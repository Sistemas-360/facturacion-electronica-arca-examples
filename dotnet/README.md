# Facturación electrónica ARCA con C# y .NET

Ejemplos oficiales para integrar la API REST de facturación electrónica ARCA en Argentina desde C# y .NET.

## Ejemplos disponibles

| Proyecto | Uso |
|---|---|
| `console/` | Prueba rápida desde terminal |
| `aspnet-core/` | Integración empresarial mediante API web |

## Requisitos

- .NET 10 SDK o superior compatible
- Token de pruebas o producción de Sistemas 360

No se utilizan paquetes externos en ninguno de los dos proyectos.

## Estructura

```text
dotnet/
├── README.md
├── console/
│   ├── Sistemas360Example.Console.csproj
│   ├── Program.cs
│   ├── Client/
│   │   └── Sistemas360Client.cs
│   └── Models/
│       ├── ClienteRequest.cs
│       ├── ItemRequest.cs
│       └── CrearComprobanteRequest.cs
└── aspnet-core/
    ├── Sistemas360Example.Api.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Controllers/
    │   └── ComprobantesController.cs
    ├── Services/
    │   ├── ISistemas360Service.cs
    │   └── Sistemas360Service.cs
    ├── Client/
    │   └── Sistemas360Client.cs
    ├── Configuration/
    │   └── Sistemas360Options.cs
    └── Models/
        ├── ClienteRequest.cs
        ├── ItemRequest.cs
        └── CrearComprobanteRequest.cs
```

## Consola

Objetivo: probar la API rápidamente desde la terminal, sin infraestructura adicional.

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

```bash
dotnet run --project console -- ping
dotnet run --project console -- crear
dotnet run --project console -- consultar 151
dotnet run --project console -- descargar-pdf 151
```

Reemplazá `151` por el ID real del comprobante. El PDF se guarda en la carpeta actual como `comprobante-151.pdf`.

## ASP.NET Core

Objetivo: mostrar cómo integrar la API dentro de un backend empresarial (ERP, SaaS, microservicios, sistemas administrativos, workers, backends empresariales), exponiendo un proxy local propio que reenvía las solicitudes a la API oficial de Sistemas 360.

### Diferencia entre la API local y Sistemas 360

El proyecto ASP.NET Core expone endpoints propios bajo `/api/facturacion`. Estos endpoints pertenecen únicamente al backend de ejemplo.

Cuando el backend recibe una solicitud, utiliza el token privado para consumir la API oficial de Sistemas 360 en `https://api.sistemas360.ar`.

Flujo:

```text
Cliente o ERP
  → Backend ASP.NET Core local
  → API Sistemas 360
  → ARCA
```

La API local no reemplaza ni modifica los endpoints oficiales.

### Endpoints locales

Estos endpoints son del proyecto ASP.NET Core de ejemplo:

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

El endpoint local `POST /api/facturacion/comprobantes` recibe la misma estructura completa de comprobante que utiliza la API oficial de Sistemas 360. No hay cálculo automático de IVA, total, cliente ni tipo de comprobante: todos los valores deben venir explícitos en la solicitud, igual que al llamar directamente a `https://api.sistemas360.ar/api/comprobantes`.

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

La URL y el puerto locales pueden variar según la configuración de ASP.NET Core (por ejemplo, el puerto que asigne `dotnet run` o tu `launchSettings.json`).

### Consultar un comprobante

```bash
curl "http://localhost:5000/api/facturacion/comprobantes/151"
```

### Descargar el PDF A4

```bash
curl "http://localhost:5000/api/facturacion/comprobantes/151/pdf" --output comprobante-151.pdf
```

### Manejo de errores

El backend local devuelve siempre JSON, nunca una página HTML ni un stack trace:

```json
{
  "ok": false,
  "error": "Mensaje del error"
}
```

- Datos inválidos en la solicitud (`ArgumentException`) → HTTP 400.
- Error devuelto por la API de Sistemas 360 (`HttpRequestException`) → se conserva el código HTTP remoto si está disponible; si no, HTTP 502.
- Cualquier otro error → HTTP 500, sin exponer stack trace, token, encabezado `Authorization` ni configuración interna.

## Idempotencia

La propiedad `referencia_externa` debe generarla el ERP, SaaS o sistema integrador que llama al backend, no el backend en sí:

- Debe representar una operación comercial única (por ejemplo, el ID de una venta o pedido).
- Debe reutilizarse al reintentar la misma operación.
- No debe generarse una referencia nueva en cada reintento.
- Si se reenvía la misma referencia para el mismo emisor, la API de Sistemas 360 devuelve el comprobante existente en lugar de crear uno duplicado.

El servicio ASP.NET Core no genera `referencia_externa` automáticamente: el valor debe venir explícito en el JSON recibido, igual que el resto de los campos del comprobante.

## Seguridad

No expongas el token en:

- Código frontend.
- Aplicaciones móviles.
- Repositorios públicos.
- Archivos versionados (`appsettings.json` no contiene tokens reales; se cargan por variable de entorno).
- Capturas de pantalla.
- Logs públicos.

El Bearer Token solo se agrega al `HttpClient` que consume la API de Sistemas 360; nunca se refleja en las respuestas del backend local, ni siquiera en los mensajes de error.

Usá variables de entorno y realizá las solicitudes desde un backend seguro.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api

## Ejemplos oficiales

https://github.com/Sistemas-360/facturacion-electronica-arca-examples
