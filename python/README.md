# Facturación electrónica ARCA con Python

Ejemplo oficial para consumir la API REST de facturación electrónica ARCA de Sistemas 360 desde Python.

## Requisitos

- Python 3.10 o superior
- Token de pruebas o producción de Sistemas 360

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

## Validar conexión

```bash
python ping.py
```

## Crear una Factura B

```bash
python crear_factura.py
```

El ejemplo genera automáticamente:

- La fecha actual.
- Una `referencia_externa` única.
- Una Factura B de ejemplo.

## Consultar un comprobante

```bash
python consultar_comprobante.py 143
```

Reemplazá `143` por el ID real del comprobante.

## Descargar el PDF A4

```bash
python descargar_pdf.py 143
```

El archivo se guardará como:

```
comprobante-143.pdf
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
