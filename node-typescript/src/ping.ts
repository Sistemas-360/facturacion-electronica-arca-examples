import { apiRequest } from "./config.js";

async function main(): Promise<void> {
    const response = await apiRequest("/api/ping");
    const data: unknown = await response.json();

    console.log(JSON.stringify(data, null, 2));
}

main().catch((error: unknown) => {
    console.error(
        error instanceof Error ? error.message : "Error desconocido"
    );

    process.exit(1);
});
