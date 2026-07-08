# API de facturación electrónica ARCA

API REST de facturación electrónica para integrar emisión, consulta y descarga de comprobantes electrónicos autorizados por ARCA desde cualquier backend que pueda operar con HTTP y JSON.

Este repositorio reúne implementaciones de referencia para consumir la API desde distintos stacks sin cambiar el contrato externo de integración.

## Compatibilidad con cualquier backend

La API es independiente del lenguaje y del framework. Puede integrarse desde cualquier backend capaz de:

- realizar solicitudes HTTP;
- enviar y recibir JSON;
- configurar encabezados HTTP;
- utilizar Bearer Token;
- procesar códigos de estado;
- almacenar credenciales de forma segura.

Los ejemplos publicados son puntos de partida. No representan una lista cerrada de tecnologías compatibles. También es posible integrar desde PHP, Symfony, Ruby, Kotlin, Rust u otras tecnologías con soporte para HTTP y JSON.

## Funcionalidades principales

- Validación de conexión con `GET /api/ping`.
- Creación de comprobantes electrónicos con `POST /api/comprobantes`.
- Consulta de comprobantes con `GET /api/comprobantes/{id}`.
- Reintento técnico con `POST /api/comprobantes/{id}/reintentar`.
- Descarga de PDF A4 con `GET /api/comprobantes/{id}/imprimir-a4`.
- Descarga de PDF ticket con `GET /api/comprobantes/{id}/imprimir-ticket`.
- Autenticación con Bearer Token.
- Idempotencia mediante `referencia_externa`.
- Manejo de ambientes de pruebas y ambiente de producción.

## Ejemplos disponibles

| Tecnología | Descripción | Enlace |
|---|---|---|
| cURL | Scripts directos para validar conexión, crear, consultar y descargar PDF A4 | [Ver ejemplo](./curl/README.md) |
| Node.js y TypeScript | Cliente de consola con scripts para ping, creación, consulta y descarga | [Ver ejemplo](./node-typescript/README.md) |
| Python | Scripts de referencia para autenticación, creación, consulta y descarga | [Ver ejemplo](./python/README.md) |
| Java y Spring Boot | Aplicación de consola con `RestClient` para probar la integración | [Ver ejemplo](./java-spring-boot/README.md) |
| C# y .NET | Ejemplo de consola y backend ASP.NET Core como proxy integrador | [Ver ejemplo](./dotnet/README.md) |
| Go | Cliente de consola tipado con pruebas automatizadas | [Ver ejemplo](./go/README.md) |

## Próximos frameworks

- Laravel
- NestJS
- FastAPI
- Next.js del lado servidor

## Próximas herramientas de integración

- Colección de Postman
- Especificación OpenAPI

## Autenticación

Todas las solicitudes deben enviar el token API en el encabezado `Authorization`.

Para requests JSON:

```http
Authorization: Bearer TU_TOKEN
Accept: application/json
Content-Type: application/json
```

Para descargas PDF:

```http
Authorization: Bearer TU_TOKEN
Accept: application/pdf
```

## Ejemplo rápido

```bash
curl -X POST "https://api.sistemas360.ar/api/comprobantes" \
  -H "Authorization: Bearer TU_TOKEN" \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo_comprobante": "factura_b",
    "concepto": "productos",
    "fecha": "2026-07-07",
    "referencia_externa": "venta_demo_1001",
    "cliente": {
      "documento_tipo": "dni",
      "documento_numero": "30111222",
      "razon_social": "Cliente Demo",
      "condicion_iva_receptor_id": 5
    },
    "items": [
      {
        "descripcion": "Producto de ejemplo",
        "cantidad": 1,
        "precio_unitario": 10000,
        "tipo_impuesto": "gravado",
        "iva": 21
      }
    ],
    "total": 12100,
    "moneda": "PES"
  }'
```

## Idempotencia

`referencia_externa` identifica una operación única del backend integrador.

- Debe generarse una vez por operación comercial.
- Debe persistirse junto con esa operación.
- Debe reutilizarse en un reintento de la misma operación.

Si se vuelve a enviar la misma `referencia_externa` para la misma empresa, la API devuelve el comprobante existente y evita duplicados.

## Seguridad

El Bearer Token pertenece al backend integrador. No debe exponerse en:

- frontend;
- aplicaciones móviles;
- repositorios públicos;
- capturas de pantalla;
- logs;
- código cliente ejecutado en el navegador.

Guardá las credenciales en variables de entorno o en un gestor seguro de secretos. Las solicitudes a la API deben salir siempre desde un backend confiable.

## Ambientes

### Ambiente de pruebas

Permite validar la integración antes de operar fiscalmente. Los comprobantes generados en pruebas no tienen validez fiscal.

### Ambiente de producción

Permite emitir comprobantes fiscales reales. Requiere la configuración fiscal completa del contribuyente, incluyendo su habilitación operativa en ARCA.

## Enlaces oficiales

- API y panel: [api.sistemas360.ar](https://api.sistemas360.ar)
- Documentación oficial: [api.sistemas360.ar/documentacion-api](https://api.sistemas360.ar/documentacion-api)
- Estado del servicio: [api.sistemas360.ar/estado](https://api.sistemas360.ar/estado)
- Organización GitHub: [github.com/Sistemas-360](https://github.com/Sistemas-360)
- Soporte: `soporte@sistemas360.ar`

## Licencia

Este repositorio se distribuye bajo licencia MIT.
