# Ejemplos de API de Facturación Electrónica ARCA en Argentina

Ejemplos oficiales de Sistemas 360 para integrar facturación electrónica ARCA mediante una API REST JSON.

La API permite emitir Facturas A, B y C, notas de crédito y notas de débito desde cualquier backend capaz de realizar solicitudes HTTP y procesar JSON.

## Funcionalidades

- API REST con JSON
- Autenticación Bearer Token
- Facturas A, B y C
- Notas de crédito y débito
- Autorización y CAE de ARCA
- Código QR fiscal
- PDF A4
- PDF Ticket
- Consulta de comprobantes
- Reintentos técnicos
- Idempotencia mediante `referencia_externa`
- Ambientes de pruebas y producción

## Enlaces oficiales

- Sitio: https://api.sistemas360.ar
- Documentación: https://api.sistemas360.ar/documentacion-api
- Estado del servicio: https://api.sistemas360.ar/estado
- Organización GitHub: https://github.com/Sistemas-360
- Soporte: soporte@sistemas360.ar

## Ejemplos disponibles

| Tecnología | Ejemplos |
|---|---|
| cURL | [Ver ejemplos](./curl) |
| Node.js y TypeScript | [Ver ejemplos](./node-typescript) |
| Python | [Ver ejemplos](./python) |
| Java y Spring Boot | [Ver ejemplos](./java-spring-boot) |
| C# y .NET | [Ver ejemplos](./dotnet) |
| Go | [Ver ejemplos](./go) |

Próximamente:

- Next.js
- NestJS
- FastAPI
- Postman
- OpenAPI

## Autenticación

Todas las solicitudes deben enviar el token en el encabezado:

```http
Authorization: Bearer TU_TOKEN
Accept: application/json
Content-Type: application/json
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

Usá una `referencia_externa` única para cada operación:

```json
{
  "referencia_externa": "venta_1001"
}
```

Si se vuelve a enviar la misma referencia para la misma empresa, la API devuelve el comprobante existente y evita duplicados.

## Seguridad

Nunca publiques tokens reales en GitHub.

No utilices el token directamente desde:

- JavaScript ejecutado en el navegador
- Aplicaciones móviles
- Repositorios públicos
- Código frontend

Las solicitudes deben realizarse desde un backend seguro.

## Ambientes

Los tokens pueden estar asociados a:

- Pruebas
- Producción

Los comprobantes generados en pruebas no tienen validez fiscal.

## Documentación completa

https://api.sistemas360.ar/documentacion-api

## Licencia

Este repositorio se distribuye bajo licencia MIT.
