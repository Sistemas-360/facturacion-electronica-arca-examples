# API de facturación electrónica ARCA con Node.js y TypeScript

Implementación de referencia para consumir la API REST de facturación electrónica desde un backend Node.js con TypeScript.

## Requisitos

- Node.js 18 o superior.
- Bearer Token de ambiente de pruebas o ambiente de producción.

## Instalación

Entrá a la carpeta:

```bash
cd node-typescript
```

Instalá las dependencias:

```bash
npm install
```

Copiá el archivo de variables:

Linux o macOS:

```bash
cp .env.example .env
```

Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Configurá el archivo `.env`:

```env
SISTEMAS360_BASE_URL=https://api.sistemas360.ar
SISTEMAS360_TOKEN=TU_TOKEN
```

## Scripts disponibles

| Script | Endpoint oficial | Descripción |
|---|---|---|
| `npm run ping` | `GET /api/ping` | Valida conexión, token y ambiente |
| `npm run crear-factura` | `POST /api/comprobantes` | Crea una Factura B de ejemplo |
| `npm run consultar -- 59` | `GET /api/comprobantes/{id}` | Consulta un comprobante |
| `npm run descargar-pdf -- 59` | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 |
| `npm run typecheck` | — | Valida tipado TypeScript |

## Validar conexión

```bash
npm run ping
```

## Crear una Factura B

```bash
npm run crear-factura
```

El ejemplo genera automáticamente:

- la fecha actual;
- una `referencia_externa` única de ejemplo;
- una Factura B de referencia.

## Consultar un comprobante

```bash
npm run consultar -- 59
```

Reemplazá `59` por el ID real del comprobante.

## Descargar el PDF A4

```bash
npm run descargar-pdf -- 59
```

El archivo se guarda como `comprobante-59.pdf`.

## Idempotencia

`referencia_externa` identifica una operación única. En este ejemplo se genera automáticamente para facilitar la prueba manual, pero en una integración real debe generarla y persistirla tu backend integrador.

## Seguridad

El token pertenece al backend y no debe exponerse en frontend, aplicaciones móviles, repositorios públicos ni JavaScript ejecutado en el navegador.

Las solicitudes deben salir desde un backend seguro.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4. Es un punto de partida para un backend integrador. Los casos avanzados están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
