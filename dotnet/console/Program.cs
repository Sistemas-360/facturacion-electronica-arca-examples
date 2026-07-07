using System.Text.Json;
using Sistemas360Example.Client;
using Sistemas360Example.Models;

const string DefaultBaseUrl =
    "https://api.sistemas360.ar";

try
{
    string baseUrl =
        Environment.GetEnvironmentVariable(
            "SISTEMAS360_BASE_URL"
        ) ?? DefaultBaseUrl;

    string? token =
        Environment.GetEnvironmentVariable(
            "SISTEMAS360_TOKEN"
        );

    if (string.IsNullOrWhiteSpace(token))
    {
        throw new InvalidOperationException(
            "Falta SISTEMAS360_TOKEN. " +
            "Configurá el token como variable de entorno."
        );
    }

    if (args.Length == 0)
    {
        MostrarAyuda();
        return 1;
    }

    string action =
        args[0].Trim().ToLowerInvariant();

    using var client =
        new Sistemas360Client(
            baseUrl,
            token
        );

    switch (action)
    {
        case "ping":
            await EjecutarPingAsync(client);
            break;

        case "crear":
            await EjecutarCrearAsync(client);
            break;

        case "consultar":
            await EjecutarConsultarAsync(
                client,
                args
            );
            break;

        case "descargar-pdf":
            await EjecutarDescargarPdfAsync(
                client,
                args
            );
            break;

        default:
            Console.Error.WriteLine(
                $"Acción desconocida: {action}"
            );

            MostrarAyuda();
            return 1;
    }

    return 0;
}
catch (OperationCanceledException)
{
    Console.Error.WriteLine(
        "La operación fue cancelada."
    );

    return 1;
}
catch (HttpRequestException exception)
{
    Console.Error.WriteLine(
        exception.Message
    );

    return 1;
}
catch (Exception exception)
{
    Console.Error.WriteLine(
        $"Error: {exception.Message}"
    );

    return 1;
}

static async Task EjecutarPingAsync(
    Sistemas360Client client
)
{
    using JsonDocument response =
        await client.PingAsync();

    ImprimirJson(response);
}

static async Task EjecutarCrearAsync(
    Sistemas360Client client
)
{
    var request = new CrearComprobanteRequest(
        TipoComprobante: "factura_b",
        Concepto: "productos",
        Fecha: DateOnly
            .FromDateTime(DateTime.Today)
            .ToString("yyyy-MM-dd"),
        ReferenciaExterna:
            $"venta_dotnet_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
        Cliente: new ClienteRequest(
            DocumentoTipo: "dni",
            DocumentoNumero: "30111222",
            RazonSocial: "Cliente Demo",
            CondicionIvaReceptorId: 5
        ),
        Items:
        [
            new ItemRequest(
                Descripcion: "Producto de ejemplo",
                Cantidad: 1m,
                PrecioUnitario: 10000m,
                TipoImpuesto: "gravado",
                Iva: 21m
            )
        ],
        Total: 12100m,
        Moneda: "PES"
    );

    using JsonDocument response =
        await client.CrearComprobanteAsync(
            request
        );

    ImprimirJson(response);
}

static async Task EjecutarConsultarAsync(
    Sistemas360Client client,
    string[] arguments
)
{
    long comprobanteId =
        ObtenerComprobanteId(
            arguments,
            "dotnet run -- consultar ID_COMPROBANTE"
        );

    using JsonDocument response =
        await client.ConsultarComprobanteAsync(
            comprobanteId
        );

    ImprimirJson(response);
}

static async Task EjecutarDescargarPdfAsync(
    Sistemas360Client client,
    string[] arguments
)
{
    long comprobanteId =
        ObtenerComprobanteId(
            arguments,
            "dotnet run -- descargar-pdf ID_COMPROBANTE"
        );

    string filePath =
        await client.DescargarPdfA4Async(
            comprobanteId
        );

    Console.WriteLine(
        $"PDF guardado en: {filePath}"
    );
}

static long ObtenerComprobanteId(
    string[] arguments,
    string usage
)
{
    if (arguments.Length < 2)
    {
        throw new ArgumentException(
            $"Uso: {usage}"
        );
    }

    if (
        !long.TryParse(
            arguments[1],
            out long comprobanteId
        ) ||
        comprobanteId <= 0
    )
    {
        throw new ArgumentException(
            "El ID del comprobante debe ser " +
            "un número entero mayor que cero."
        );
    }

    return comprobanteId;
}

static void ImprimirJson(
    JsonDocument document
)
{
    string json =
        JsonSerializer.Serialize(
            document.RootElement,
            new JsonSerializerOptions
            {
                WriteIndented = true
            }
        );

    Console.WriteLine(json);
}

static void MostrarAyuda()
{
    Console.WriteLine(
        """
        API Sistemas 360 - Ejemplo C# y .NET

        Uso:

          dotnet run -- ping
          dotnet run -- crear
          dotnet run -- consultar ID_COMPROBANTE
          dotnet run -- descargar-pdf ID_COMPROBANTE

        Variables de entorno:

          SISTEMAS360_TOKEN       Obligatoria
          SISTEMAS360_BASE_URL    Opcional
        """
    );
}
