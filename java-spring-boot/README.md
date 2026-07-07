# Facturación electrónica ARCA con Java y Spring Boot

Ejemplo oficial para consumir la API REST de facturación electrónica ARCA de Sistemas 360 desde Java y Spring Boot.

## Requisitos

- Java 17 o superior
- Maven 3.9 o superior
- Token de pruebas o producción de Sistemas 360

## Instalación

Entrá a la carpeta:

```bash
cd java-spring-boot
```

Compilá el proyecto:

```bash
mvn clean package
```

## Configuración

Usá variables de entorno.

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

## Validar conexión

Linux o macOS:

```bash
SISTEMAS360_ACTION=ping mvn spring-boot:run
```

Windows PowerShell:

```powershell
$env:SISTEMAS360_ACTION="ping"
mvn spring-boot:run
```

## Crear una Factura B

Linux o macOS:

```bash
SISTEMAS360_ACTION=crear mvn spring-boot:run
```

Windows PowerShell:

```powershell
$env:SISTEMAS360_ACTION="crear"
mvn spring-boot:run
```

El ejemplo genera automáticamente:

- La fecha actual.
- Una `referencia_externa` única.
- Una Factura B de ejemplo.

## Consultar un comprobante

Linux o macOS:

```bash
SISTEMAS360_ACTION=consultar \
SISTEMAS360_COMPROBANTE_ID=151 \
mvn spring-boot:run
```

Windows PowerShell:

```powershell
$env:SISTEMAS360_ACTION="consultar"
$env:SISTEMAS360_COMPROBANTE_ID="151"
mvn spring-boot:run
```

Reemplazá `151` por el ID real.

## Descargar el PDF A4

Linux o macOS:

```bash
SISTEMAS360_ACTION=descargar-pdf \
SISTEMAS360_COMPROBANTE_ID=151 \
mvn spring-boot:run
```

Windows PowerShell:

```powershell
$env:SISTEMAS360_ACTION="descargar-pdf"
$env:SISTEMAS360_COMPROBANTE_ID="151"
mvn spring-boot:run
```

El archivo se guardará como:

```
comprobante-151.pdf
```

## Seguridad

No expongas el token en:

- Código frontend.
- Aplicaciones móviles.
- Repositorios públicos.
- Archivos versionados.

Las solicitudes deben realizarse desde un backend seguro.

## Documentación oficial

https://api.sistemas360.ar/documentacion-api
