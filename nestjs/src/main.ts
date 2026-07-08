import "reflect-metadata";
import { NestFactory } from "@nestjs/core";
import { AppModule } from "./app.module";

async function bootstrap(): Promise<void> {
    const app = await NestFactory.create(AppModule);
    await app.listen(3000);
}

bootstrap().catch((error: unknown) => {
    const message = error instanceof Error ? error.message : "Error desconocido";
    console.error(message);
    process.exit(1);
});
