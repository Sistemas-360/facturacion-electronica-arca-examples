import json
import sys

from config import api_request


def obtener_comprobante_id() -> str:
    if len(sys.argv) < 2:
        raise RuntimeError(
            "Uso: python consultar_comprobante.py ID_COMPROBANTE"
        )

    comprobante_id = sys.argv[1]

    if not comprobante_id.isdigit():
        raise RuntimeError(
            "El ID del comprobante debe ser un número entero."
        )

    return comprobante_id


def main() -> None:
    comprobante_id = obtener_comprobante_id()

    response = api_request(
        "GET",
        f"/api/comprobantes/{comprobante_id}",
    )

    print(json.dumps(response.json(), indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
