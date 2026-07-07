# Facturación electrónica ARCA con Node.js y TypeScript

Ejemplo oficial para consumir la API REST de facturación electrónica ARCA de Sistemas 360 desde Node.js y TypeScript.

## Requisitos

- Node.js 18 o superior
- Token de pruebas o producción de Sistemas 360

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

## Validar conexión

```bash
npm run ping
```

## Crear una Factura B

```bash
npm run crear-factura
```

El ejemplo genera automáticamente:

- La fecha actual.
- Una `referencia_externa` única.
- Una Factura B de prueba.

## Consultar un comprobante

Pasá el ID como argumento:

```bash
npm run consultar -- 59
```

Reemplazá `59` por el ID real del comprobante.

## Descargar el PDF A4

```bash
npm run descargar-pdf -- 59
```

El archivo se guardará como:

```
comprobante-59.pdf
```

## Seguridad

No expongas el token en:

- Código frontend.
- Aplicaciones móviles.
- Repositorios públicos.
- JavaScript ejecutado en el navegador.

Las solicitudes deben realizarse desde un backend seguro.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api
