# Facturación electrónica ARCA con cURL

Ejemplos oficiales para consumir la API REST de facturación electrónica ARCA de Sistemas 360 usando cURL desde la línea de comandos.

## Requisitos

- `curl` disponible en el sistema (Bash, Git Bash o WSL en Windows)
- Token de pruebas o producción

## Configuración

Los scripts leen la configuración desde variables de entorno:

| Variable | Obligatoria | Valor predeterminado |
|---|---|---|
| `SISTEMAS360_TOKEN` | Sí | — |
| `SISTEMAS360_BASE_URL` | No | `https://api.sistemas360.ar` |

Exportá tu token antes de ejecutar cualquier script:

```bash
export SISTEMAS360_TOKEN=TU_TOKEN
```

Si necesitás apuntar a otro ambiente:

```bash
export SISTEMAS360_BASE_URL=https://api.sistemas360.ar
```

## Scripts disponibles

| Script | Endpoint | Descripción |
|---|---|---|
| [ping.sh](./ping.sh) | `GET /api/ping` | Verifica la conexión con la API |
| [crear-factura-b.sh](./crear-factura-b.sh) | `POST /api/comprobantes` | Crea una Factura B de ejemplo |
| [consultar-comprobante.sh](./consultar-comprobante.sh) | `GET /api/comprobantes/{id}` | Consulta un comprobante por ID |
| [descargar-pdf.sh](./descargar-pdf.sh) | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 de un comprobante |

## Uso

Dar permisos de ejecución (una sola vez):

```bash
chmod +x *.sh
```

Validar conexión:

```bash
./ping.sh
```

Crear Factura B:

```bash
./crear-factura-b.sh
```

Cada ejecución genera automáticamente la fecha actual y una `referencia_externa` única, evitando comprobantes duplicados.

Consultar comprobante:

```bash
./consultar-comprobante.sh 59
```

O mediante variable de entorno:

```bash
COMPROBANTE_ID=59 ./consultar-comprobante.sh
```

Descargar PDF A4:

```bash
./descargar-pdf.sh 59
```

## Manejo de errores

Todos los scripts usan `set -euo pipefail` y validan:

- Que `SISTEMAS360_TOKEN` esté definido antes de realizar la solicitud.
- El código de estado HTTP de la respuesta, mostrando un mensaje de error claro por `stderr` si la API responde con un código distinto de 2xx.

## Seguridad

Nunca publiques tokens reales en GitHub. Estos scripts no contienen credenciales: siempre se leen desde variables de entorno.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api
