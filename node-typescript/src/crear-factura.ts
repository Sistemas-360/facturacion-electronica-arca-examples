import { apiRequest } from "./config.js";

function fechaActual(): string {
    return new Date().toISOString().slice(0, 10);
}

async function main(): Promise<void> {
    const payload = {
        tipo_comprobante: "factura_b",
        concepto: "productos",
        fecha: fechaActual(),
        referencia_externa: `venta_node_${Date.now()}`,
        cliente: {
            documento_tipo: "dni",
            documento_numero: "30111222",
            razon_social: "Cliente Demo",
            condicion_iva_receptor_id: 5,
        },
        items: [
            {
                descripcion: "Producto de ejemplo",
                cantidad: 1,
                precio_unitario: 10000,
                tipo_impuesto: "gravado",
                iva: 21,
            },
        ],
        total: 12100,
        moneda: "PES",
    };

    const response = await apiRequest("/api/comprobantes", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
    });

    const data: unknown = await response.json();

    console.log(JSON.stringify(data, null, 2));
}

main().catch((error: unknown) => {
    console.error(
        error instanceof Error ? error.message : "Error desconocido"
    );

    process.exit(1);
});
