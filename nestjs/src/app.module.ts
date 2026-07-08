import { Module } from "@nestjs/common";
import { ConfigModule } from "@nestjs/config";
import { FacturacionController } from "./facturacion.controller";
import { FacturacionService } from "./facturacion.service";

@Module({
    imports: [ConfigModule.forRoot({ isGlobal: true })],
    controllers: [FacturacionController],
    providers: [FacturacionService],
})
export class AppModule {}
