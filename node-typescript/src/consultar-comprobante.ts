import { apiRequest } from "./config.js";

async function main(): Promise<void> {
    const comprobanteId = process.env.COMPROBANTE_ID;

    if (!comprobanteId) {
        throw new Error("Falta COMPROBANTE_ID en el archivo .env.");
    }

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
