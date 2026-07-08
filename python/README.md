# API de facturación electrónica ARCA con Python

Implementación de referencia para consumir la API REST de facturación electrónica desde un backend Python.

## Requisitos

- Python 3.10 o superior.
- Bearer Token de ambiente de pruebas o ambiente de producción.

## Instalación

Entrá a la carpeta:

```bash
cd python
```

Creá un entorno virtual:

Linux o macOS:

```bash
python3 -m venv .venv
source .venv/bin/activate
```

Windows PowerShell:

```powershell
python -m venv .venv
.venv\Scripts\Activate.ps1
```

Instalá las dependencias:

```bash
pip install -r requirements.txt
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
| `python ping.py` | `GET /api/ping` | Valida conexión, token y ambiente |
| `python crear_factura.py` | `POST /api/comprobantes` | Crea una Factura B de ejemplo |
| `python consultar_comprobante.py 143` | `GET /api/comprobantes/{id}` | Consulta un comprobante |
| `python descargar_pdf.py 143` | `GET /api/comprobantes/{id}/imprimir-a4` | Descarga el PDF A4 |

## Validar conexión

```bash
python ping.py
```

## Crear una Factura B

```bash
python crear_factura.py
```

El ejemplo genera automáticamente:

- la fecha actual;
- una `referencia_externa` única de ejemplo;
- una Factura B de referencia.

## Consultar un comprobante

```bash
python consultar_comprobante.py 143
```

Reemplazá `143` por el ID real del comprobante.

## Descargar el PDF A4

```bash
python descargar_pdf.py 143
```

El archivo se guarda como `comprobante-143.pdf`.

## Idempotencia

`referencia_externa` identifica una operación única. En este ejemplo se genera automáticamente para facilitar la prueba manual, pero en una integración real debe generarla y persistirla tu backend integrador.

## Seguridad

El token pertenece al backend. No lo expongas en frontend, aplicaciones móviles, archivos versionados ni repositorios públicos.

Las solicitudes deben realizarse desde un backend seguro.

## Ambientes

- `Ambiente de pruebas`: permite validar la integración antes de operar fiscalmente. Los comprobantes generados no tienen validez fiscal.
- `Ambiente de producción`: permite emitir comprobantes fiscales reales y requiere la configuración fiscal completa del contribuyente.

## Alcance del ejemplo

Este ejemplo cubre ping, creación, consulta y descarga de PDF A4. Es un punto de partida para una integración fiscal. Los casos avanzados están en la documentación oficial.

## Documentación oficial

[api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
