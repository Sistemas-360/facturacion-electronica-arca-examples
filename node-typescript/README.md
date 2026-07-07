# Facturación electrónica ARCA con Node.js y TypeScript

Ejemplo oficial para consumir la API REST de facturación electrónica ARCA de Sistemas 360 desde Node.js y TypeScript.

## Requisitos

- Node.js 18 o superior
- Token de pruebas o producción

## Instalación

```bash
npm install
```

Copiar las variables:

```bash
cp .env.example .env
```

En Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Configurar `.env`:

```env
SISTEMAS360_BASE_URL=https://api.sistemas360.ar
SISTEMAS360_TOKEN=TU_TOKEN
COMPROBANTE_ID=59
```

## Validar conexión

```bash
npm run ping
```

## Crear Factura B

```bash
npm run crear-factura
```

## Consultar comprobante

```bash
npm run consultar
```

## Descargar PDF A4

```bash
npm run descargar-pdf
```

## Seguridad

No expongas el token en frontend, aplicaciones móviles ni repositorios públicos.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api
