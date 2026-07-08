# API de facturación electrónica ARCA con cURL

Scripts de referencia para consumir la API REST de facturación electrónica desde cURL.

## Requisitos

- Bash, Git Bash, WSL, Linux o macOS.
- cURL instalado.
- Bearer Token de ambiente de pruebas o ambiente de producción.

## Configuración

Los scripts usan estas variables de entorno:

| Variable | Obligatoria | Valor predeterminado |
|---|---|---|
| `SISTEMAS360_TOKEN` | Sí | Sin valor |
| `SISTEMAS360_BASE_URL` | No | `https://api.sistemas360.ar` |

Configurá el token:

```bash
export SISTEMAS360_TOKEN="TU_TOKEN"
```

Opcionalmente, configurá la URL base:

```bash
export SISTEMAS360_BASE_URL="https://api.sistemas360.ar"
```

## Uso inicial

Ejecutá una sola vez:

```bash
chmod +x *.sh
```

## Operaciones incluidas

| Script | Endpoint oficial | Descripción |
|---|---|---|
| [ping.sh](./ping.sh) | `GET /api/ping` | Valida conexión, token y ambiente |
| [crear-factura-b.sh](./crear-factura-b.sh) | `POST /api/comprobantes` | Crea una Factura B de ejemplo |
| [consultar-comprobante.sh](./consultar-comprobante.sh) | `GET /api/comprobantes/{id}` | Consulta un comprobante existente |
| [descargar-pdf.sh](./descargar-pdf.sh) | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 |

## Validar conexión

```bash
./ping.sh
```

## Crear una Factura B

```bash
./crear-factura-b.sh
```

El script genera automáticamente:

- la fecha actual;
- una `referencia_externa` única de ejemplo;
- una Factura B de referencia.

## Consultar un comprobante

```bash
./consultar-comprobante.sh 59
```

Reemplazá `59` por el ID real del comprobante.

## Descargar el PDF A4

```bash
./descargar-pdf.sh 59
```

El archivo se guarda como `comprobante-59.pdf`.

## Idempotencia

`referencia_externa` identifica una operación única. En una integración real debe generarla y persistirla tu backend integrador, no un frontend.

## Seguridad

El token pertenece al backend. No lo expongas en frontend, aplicaciones móviles, repositorios públicos ni scripts compartidos con credenciales reales.

Los scripts leen el token desde variables de entorno y no incluyen secretos embebidos.

## Ambientes

- `Ambiente de pruebas`: permite validar autenticación y flujo de integración. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4. Los casos avanzados, como reintento técnico o descarga de ticket, están documentados en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
