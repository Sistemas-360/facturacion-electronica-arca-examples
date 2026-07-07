# Facturación electrónica ARCA con cURL

Ejemplos oficiales para consumir la API REST de facturación electrónica ARCA de Sistemas 360 usando cURL.

## Requisitos

- Bash, Git Bash, WSL, Linux o macOS.
- cURL instalado.
- Token de pruebas o producción de Sistemas 360.

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

## Dar permisos de ejecución

Ejecutá una sola vez:

```bash
chmod +x *.sh
```

## Validar conexión

```bash
./ping.sh
```

## Crear una Factura B

```bash
./crear-factura-b.sh
```

Cada ejecución genera automáticamente:

- La fecha actual.
- Una `referencia_externa` única.
- Una Factura B de ejemplo.

## Consultar un comprobante

```bash
./consultar-comprobante.sh 59
```

Reemplazá `59` por el ID real del comprobante.

## Descargar el PDF A4

```bash
./descargar-pdf.sh 59
```

El archivo se guardará como:

```
comprobante-59.pdf
```

## Scripts disponibles

| Script | Endpoint | Descripción |
|---|---|---|
| [ping.sh](./ping.sh) | `GET /api/ping` | Valida el token y la conexión |
| [crear-factura-b.sh](./crear-factura-b.sh) | `POST /api/comprobantes` | Crea una Factura B |
| [consultar-comprobante.sh](./consultar-comprobante.sh) | `GET /api/comprobantes/{id}` | Consulta un comprobante |
| [descargar-pdf.sh](./descargar-pdf.sh) | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 |

## Seguridad

Nunca publiques tokens reales en GitHub.

Los scripts leen el token desde una variable de entorno y no contienen credenciales.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api
