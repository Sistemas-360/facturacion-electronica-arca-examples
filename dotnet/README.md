# Facturación electrónica ARCA con C# y .NET

Ejemplo oficial para consumir la API REST de facturación electrónica ARCA en Argentina desde C# y .NET.

El proyecto permite validar la conexión, crear una Factura B, consultar un comprobante y descargar el PDF A4.

## Requisitos

- .NET 10 SDK o superior compatible
- Token de pruebas o producción de Sistemas 360

## Tecnologías utilizadas

- C#
- .NET 10
- HttpClient
- System.Text.Json
- API REST
- JSON
- Bearer Token

No se utilizan paquetes externos.

## Instalación

Entrá a la carpeta:

```bash
cd dotnet
```

Restaurá y compilá el proyecto:

```bash
dotnet restore
dotnet build
```

## Configuración

El ejemplo usa variables de entorno.

### Linux o macOS

```bash
export SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
export SISTEMAS360_TOKEN="TU_TOKEN"
```

### Windows PowerShell

```powershell
$env:SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
$env:SISTEMAS360_TOKEN="TU_TOKEN"
```

El token no debe escribirse directamente en el código ni subirse al repositorio.

## Validar la conexión

```bash
dotnet run -- ping
```

El comando realiza:

```text
GET /api/ping
```

## Crear una Factura B

```bash
dotnet run -- crear
```

El ejemplo genera automáticamente:

- La fecha actual.
- Una `referencia_externa` única.
- Un cliente de prueba.
- Un ítem gravado.
- IVA del 21%.
- Total de $12.100.
- Moneda PES.

La solicitud se envía a:

```text
POST /api/comprobantes
```

## Consultar un comprobante

Pasá el ID devuelto por la creación:

```bash
dotnet run -- consultar 151
```

Reemplazá `151` por el ID real del comprobante.

El comando realiza:

```text
GET /api/comprobantes/151
```

## Descargar el PDF A4

```bash
dotnet run -- descargar-pdf 151
```

El archivo se guardará en la carpeta actual:

```text
comprobante-151.pdf
```

## Comandos disponibles

| Comando | Descripción |
|---|---|
| `dotnet run -- ping` | Valida el token y la conexión |
| `dotnet run -- crear` | Crea una Factura B de ejemplo |
| `dotnet run -- consultar ID` | Consulta un comprobante |
| `dotnet run -- descargar-pdf ID` | Descarga el PDF A4 |

## Ejemplo de Factura B

El ejemplo envía:

```json
{
  "tipo_comprobante": "factura_b",
  "concepto": "productos",
  "fecha": "2026-07-07",
  "referencia_externa": "venta_dotnet_1783456789000",
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

La propiedad `referencia_externa` identifica una operación única.

Si se vuelve a enviar la misma referencia para el mismo emisor, la API devuelve el comprobante existente y evita generar un duplicado.

## Manejo de errores

El cliente muestra:

- Código HTTP.
- Motivo del error.
- Respuesta JSON enviada por la API.
- Errores por token faltante.
- Errores por ID inválido.
- Errores por timeout.
- Error si la descarga no devuelve un PDF.

## Seguridad

No expongas el token en:

- Código frontend.
- Aplicaciones móviles.
- Repositorios públicos.
- Archivos versionados.
- Capturas de pantalla.
- Logs públicos.

Usá variables de entorno y realizá las solicitudes desde un backend seguro.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api

## Ejemplos oficiales

https://github.com/Sistemas-360/facturacion-electronica-arca-examples
