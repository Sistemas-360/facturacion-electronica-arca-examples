import { Injectable } from "@nestjs/common";

@Injectable()
export class FacturacionService {
    private readonly baseUrl: string;
    private readonly token: string;

    constructor() {
        this.baseUrl =
            process.env.SISTEMAS360_BASE_URL?.replace(/\/$/, "") ??
            "https://api.sistemas360.ar";

        const token = process.env.SISTEMAS360_TOKEN;

        if (!token) {
            throw new Error(
                "Falta SISTEMAS360_TOKEN. Copiá .env.example como .env y configurá tu token."
            );
        }

        this.token = token;
    }

    async ping(): Promise<unknown> {
        return this.requestJson("GET", "/api/ping");
    }

    async crearComprobante(payload: unknown): Promise<unknown> {
        return this.requestJson("POST", "/api/comprobantes", payload);
    }

    async consultarComprobante(id: number): Promise<unknown> {
        return this.requestJson("GET", `/api/comprobantes/${id}`);
    }

    async descargarPdf(id: number): Promise<Buffer> {
        const response = await fetch(
            `${this.baseUrl}/api/comprobantes/${id}/imprimir-a4`,
            {
                headers: {
                    Authorization: `Bearer ${this.token}`,
                    Accept: "application/pdf",
                },
            }
        );

        if (!response.ok) {
            throw new Error(
                `Error ${response.status} ${response.statusText}: ${await response.text()}`
            );
        }

        const contentType = response.headers.get("content-type") ?? "";

        if (!contentType.toLowerCase().startsWith("application/pdf")) {
            throw new Error(
                `La API no devolvió un PDF válido. Content-Type recibido: ${contentType}`
            );
        }

        return Buffer.from(await response.arrayBuffer());
    }

    private async requestJson(
        method: string,
        path: string,
        payload?: unknown
    ): Promise<unknown> {
        const response = await fetch(`${this.baseUrl}${path}`, {
            method,
            headers: {
                Authorization: `Bearer ${this.token}`,
                Accept: "application/json",
                ...(payload ? { "Content-Type": "application/json" } : {}),
            },
            ...(payload ? { body: JSON.stringify(payload) } : {}),
        });

        if (!response.ok) {
            throw new Error(
                `Error ${response.status} ${response.statusText}: ${await response.text()}`
            );
        }

        return response.json();
    }
}
