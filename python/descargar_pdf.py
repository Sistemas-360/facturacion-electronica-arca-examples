import sys
from pathlib import Path

import requests

from config import BASE_URL, TOKEN


def obtener_comprobante_id() -> str:
    if len(sys.argv) < 2:
        raise RuntimeError(
            "Uso: python descargar_pdf.py ID_COMPROBANTE"
        )

    comprobante_id = sys.argv[1]

    if not comprobante_id.isdigit():
        raise RuntimeError(
            "El ID del comprobante debe ser un número entero."
        )

    return comprobante_id


def main() -> None:
    comprobante_id = obtener_comprobante_id()

    response = requests.get(
        f"{BASE_URL}/api/comprobantes/"
        f"{comprobante_id}/imprimir-a4",
        headers={
            "Authorization": f"Bearer {TOKEN}",
            "Accept": "application/pdf",
        },
        timeout=30,
    )

    if not response.ok:
        raise RuntimeError(
            f"Error {response.status_code}: {response.text}"
        )

    archivo = Path(f"comprobante-{comprobante_id}.pdf")
    temporal = archivo.with_suffix(".pdf.tmp")

    temporal.write_bytes(response.content)
    temporal.replace(archivo)

    print(f"PDF guardado en: {archivo}")


if __name__ == "__main__":
    main()
