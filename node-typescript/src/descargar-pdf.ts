import { writeFile } from "node:fs/promises";
import { config } from "./config.js";

function obtenerComprobanteId(): string {
    const comprobanteId = process.argv[2];

    if (!comprobanteId) {
        throw new Error(
            "Uso: npm run descargar-pdf -- ID_COMPROBANTE"
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

    const response = await fetch(
        `${config.baseUrl}/api/comprobantes/${encodeURIComponent(
            comprobanteId
        )}/imprimir-a4`,
        {
            headers: {
                Authorization: `Bearer ${config.token}`,
                Accept: "application/pdf",
            },
        }
    );

    if (!response.ok) {
        const contentType = response.headers.get("content-type") ?? "";
        const body = contentType.includes("application/json")
            ? JSON.stringify(await response.json())
            : await response.text();

        throw new Error(
            `Error ${response.status} ${response.statusText}: ${body}`
        );
    }

    const buffer = Buffer.from(await response.arrayBuffer());
    const filename = `comprobante-${comprobanteId}.pdf`;

    await writeFile(filename, buffer);

    console.log(`PDF guardado en: ${filename}`);
}

main().catch((error: unknown) => {
    console.error(
        error instanceof Error ? error.message : "Error desconocido"
    );

    process.exit(1);
});
