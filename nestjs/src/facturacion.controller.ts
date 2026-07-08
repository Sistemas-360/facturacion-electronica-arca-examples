import {
    Body,
    Controller,
    Get,
    Param,
    ParseIntPipe,
    Post,
    Res,
} from "@nestjs/common";
import type { Response } from "express";
import { FacturacionService } from "./facturacion.service";

@Controller("api/facturacion")
export class FacturacionController {
    constructor(private readonly facturacionService: FacturacionService) {}

    @Get("ping")
    ping(): Promise<unknown> {
        return this.facturacionService.ping();
    }

    @Post("comprobantes")
    crearComprobante(@Body() body: Record<string, unknown>): Promise<unknown> {
        return this.facturacionService.crearComprobante(body);
    }

    @Get("comprobantes/:id")
    consultarComprobante(
        @Param("id", ParseIntPipe) id: number
    ): Promise<unknown> {
        return this.facturacionService.consultarComprobante(id);
    }

    @Get("comprobantes/:id/pdf")
    async descargarPdf(
        @Param("id", ParseIntPipe) id: number,
        @Res() response: Response
    ): Promise<void> {
        const pdf = await this.facturacionService.descargarPdf(id);

        response.setHeader("Content-Type", "application/pdf");
        response.setHeader(
            "Content-Disposition",
            `attachment; filename="comprobante-${id}.pdf"`
        );
        response.send(pdf);
    }
}
