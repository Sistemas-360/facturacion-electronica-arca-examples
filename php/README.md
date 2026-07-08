# API de facturación electrónica ARCA con PHP

Implementación de referencia para consumir la API REST de facturación electrónica desde PHP sin framework.

## Requisitos

- PHP 8.1 o superior con extensión `curl`.
- Bearer Token de ambiente de pruebas o ambiente de producción.

## Instalación

Entrá a la carpeta:

```bash
cd php
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

## Scripts disponibles

| Script | Endpoint oficial | Descripción |
|---|---|---|
| `php ping.php` | `GET /api/ping` | Valida conexión, token y ambiente |
| `php crear_factura.php` | `POST /api/comprobantes` | Crea una Factura B de ejemplo |
| `php consultar_comprobante.php 151` | `GET /api/comprobantes/{id}` | Consulta un comprobante |
| `php descargar_pdf.php 151` | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 |

## Validar conexión

```bash
php ping.php
```

## Crear una Factura B

```bash
php crear_factura.php
```

El ejemplo genera automáticamente:

- la fecha actual;
- una `referencia_externa` única de ejemplo;
- una Factura B de referencia.

## Consultar un comprobante

```bash
php consultar_comprobante.php 151
```

## Descargar el PDF A4

```bash
php descargar_pdf.php 151
```

## Idempotencia

`referencia_externa` identifica una operación única. En este ejemplo se genera automáticamente para facilitar la prueba manual, pero en una integración real debe generarla y persistirla tu backend integrador.

## Seguridad

El token pertenece al backend. No lo expongas en frontend, aplicaciones móviles, repositorios públicos ni archivos compartidos con credenciales reales.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4. Es una base simple y portable para PHP puro o para migrar luego a Laravel u otro framework.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
