import { apiRequest } from "./config.js";

function obtenerComprobanteId(): string {
    const comprobanteId = process.argv[2];

    if (!comprobanteId) {
        throw new Error(
            "Uso: npm run consultar -- ID_COMPROBANTE"
        );
    }

    if (!/^\d+$/.test(comprobanteId)) {
        throw new Error(
            "El ID del comprobante debe ser un número entero."
        );
    }

    return comprobanteId;
}

async function main(): Promise<void> {
    const comprobanteId = obtenerComprobanteId();

    const response = await apiRequest(
        `/api/comprobantes/${encodeURIComponent(comprobanteId)}`
    );

    const data: unknown = await response.json();

    console.log(JSON.stringify(data, null, 2));
}

main().catch((error: unknown) => {
    console.error(
        error instanceof Error ? error.message : "Error desconocido"
    );

    process.exit(1);
});
