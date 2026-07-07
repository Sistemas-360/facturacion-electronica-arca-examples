import json
import time
from datetime import date

from config import api_request


def main() -> None:
    payload = {
        "tipo_comprobante": "factura_b",
        "concepto": "productos",
        "fecha": date.today().isoformat(),
        "referencia_externa": f"venta_python_{int(time.time())}",
        "cliente": {
            "documento_tipo": "dni",
            "documento_numero": "30111222",
            "razon_social": "Cliente Demo",
            "condicion_iva_receptor_id": 5,
        },
        "items": [
            {
                "descripcion": "Producto de ejemplo",
                "cantidad": 1,
                "precio_unitario": 10000,
                "tipo_impuesto": "gravado",
                "iva": 21,
            }
        ],
        "total": 12100,
        "moneda": "PES",
    }

    response = api_request(
        "POST",
        "/api/comprobantes",
        headers={
            "Content-Type": "application/json",
        },
        json=payload,
    )

    print(json.dumps(response.json(), indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
