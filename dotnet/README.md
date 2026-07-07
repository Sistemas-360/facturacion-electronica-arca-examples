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
        ├── CrearComprobanteRequest.cs
        └── CrearFacturaDemoRequest.cs
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

Objetivo: mostrar cómo integrar la API dentro de un backend empresarial (ERP, SaaS, microservicios, sistemas administrativos, workers, backends empresariales), exponiendo la funcionalidad como una API web propia.

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

### Endpoints locales

```text
GET  /api/sistemas360/ping
POST /api/sistemas360/facturas-demo
GET  /api/sistemas360/comprobantes/{id}
GET  /api/sistemas360/comprobantes/{id}/pdf
```

### Crear una Factura B

El endpoint `POST /api/sistemas360/facturas-demo` recibe un cuerpo simplificado:

```json
{
  "documentoNumero": "30111222",
  "razonSocial": "Cliente Demo",
  "descripcion": "Producto de ejemplo",
  "precioUnitario": 10000
}
```

El servicio calcula automáticamente el IVA (21%), el total y una `referencia_externa` única antes de enviar la Factura B a la API de Sistemas 360.

Ejemplo con cURL contra el servidor local:

```bash
curl -X POST "http://localhost:5000/api/sistemas360/facturas-demo" \
  -H "Content-Type: application/json" \
  -d '{
    "documentoNumero": "30111222",
    "razonSocial": "Cliente Demo",
    "descripcion": "Producto de ejemplo",
    "precioUnitario": 10000
  }'
```

### Consultar un comprobante

```bash
curl "http://localhost:5000/api/sistemas360/comprobantes/151"
```

### Descargar el PDF A4

```bash
curl "http://localhost:5000/api/sistemas360/comprobantes/151/pdf" --output comprobante-151.pdf
```

## Idempotencia

Ambos ejemplos generan una `referencia_externa` única por operación (`venta_dotnet_<timestamp>` en la consola, `venta_aspnet_<guid>` en la API). Si se reenvía la misma referencia para el mismo emisor, la API devuelve el comprobante existente en lugar de duplicarlo.

## Seguridad

No expongas el token en:

- Código frontend.
- Aplicaciones móviles.
- Repositorios públicos.
- Archivos versionados (`appsettings.json` no contiene tokens reales; se cargan por variable de entorno).
- Capturas de pantalla.
- Logs públicos.

Usá variables de entorno y realizá las solicitudes desde un backend seguro.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api

## Ejemplos oficiales

https://github.com/Sistemas-360/facturacion-electronica-arca-examples
