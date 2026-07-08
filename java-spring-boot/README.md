# API de facturación electrónica ARCA con Java y Spring Boot

Implementación de referencia para consumir la API REST de facturación electrónica desde un backend Java con Spring Boot.

## Requisitos

- Java 17 o superior.
- Maven 3.9 o superior.
- Bearer Token de ambiente de pruebas o ambiente de producción.

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

## Operaciones incluidas

| Acción | Endpoint oficial | Comando |
|---|---|---|
| Ping | `GET /api/ping` | `SISTEMAS360_ACTION=ping mvn spring-boot:run` |
| Crear comprobante | `POST /api/comprobantes` | `SISTEMAS360_ACTION=crear mvn spring-boot:run` |
| Consultar comprobante | `GET /api/comprobantes/{id}` | `SISTEMAS360_ACTION=consultar` + `SISTEMAS360_COMPROBANTE_ID=151` |
| Descargar PDF A4 | `GET /api/comprobantes/{id}/imprimir-a4` | `SISTEMAS360_ACTION=descargar-pdf` + `SISTEMAS360_COMPROBANTE_ID=151` |

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

- la fecha actual;
- una `referencia_externa` única de ejemplo;
- una Factura B de referencia.

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

El archivo se guarda como `comprobante-151.pdf`.

## Idempotencia

`referencia_externa` identifica una operación única. En este ejemplo se genera automáticamente para simplificar la ejecución manual, pero en una integración real debe generarla y persistirla tu backend integrador.

## Seguridad

El token pertenece al backend. No lo expongas en frontend, aplicaciones móviles, repositorios públicos ni archivos versionados.

Las solicitudes deben realizarse desde un backend seguro.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4. Es un punto de partida para un backend integrador con Spring Boot. Los casos avanzados están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
