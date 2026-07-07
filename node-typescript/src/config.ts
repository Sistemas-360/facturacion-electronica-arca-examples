import "dotenv/config";

const baseUrl =
    process.env.SISTEMAS360_BASE_URL?.replace(/\/$/, "") ??
    "https://api.sistemas360.ar";

const token = process.env.SISTEMAS360_TOKEN;

if (!token) {
    throw new Error(
        "Falta SISTEMAS360_TOKEN. Copiá .env.example como .env y configurá tu token."
    );
}

export const config = {
    baseUrl,
    token,
};

export async function apiRequest(
    path: string,
    options: RequestInit = {}
): Promise<Response> {
    const response = await fetch(`${config.baseUrl}${path}`, {
        ...options,
        headers: {
            Authorization: `Bearer ${config.token}`,
            Accept: "application/json",
            ...options.headers,
        },
    });

    if (!response.ok) {
        const body = await response.text();

        throw new Error(
            `Error ${response.status} ${response.statusText}: ${body}`
        );
    }

    return response;
}
