using System.Text.Json;
using Sistemas360Example.Api.Client;
using Sistemas360Example.Api.Models;

namespace Sistemas360Example.Api.Services;

public sealed class Sistemas360Service
    : ISistemas360Service
{
    private readonly Sistemas360Client _client;

    public Sistemas360Service(
        Sistemas360Client client
    )
    {
        _client = client;
    }

    public Task<JsonDocument> PingAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _client.PingAsync(
            cancellationToken
        );
    }

    public Task<JsonDocument> CrearFacturaDemoAsync(
        CrearFacturaDemoRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.DocumentoNumero))
        {
            throw new ArgumentException(
                "DocumentoNumero es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(request.RazonSocial))
        {
            throw new ArgumentException(
                "RazonSocial es obligatoria."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Descripcion))
        {
            throw new ArgumentException(
                "Descripcion es obligatoria."
            );
        }

        if (request.PrecioUnitario <= 0)
        {
            throw new ArgumentException(
                "PrecioUnitario debe ser mayor que cero."
            );
        }

        decimal iva =
            decimal.Round(
                request.PrecioUnitario * 0.21m,
                2,
                MidpointRounding.AwayFromZero
            );

        decimal total =
            request.PrecioUnitario + iva;

        var comprobante =
            new CrearComprobanteRequest(
                TipoComprobante: "factura_b",
                Concepto: "productos",
                Fecha: DateOnly
                    .FromDateTime(DateTime.Today)
                    .ToString("yyyy-MM-dd"),
                ReferenciaExterna:
                    $"venta_aspnet_{Guid.NewGuid():N}",
                Cliente: new ClienteRequest(
                    DocumentoTipo: "dni",
                    DocumentoNumero:
                        request.DocumentoNumero,
                    RazonSocial:
                        request.RazonSocial,
                    CondicionIvaReceptorId: 5
                ),
                Items:
                [
                    new ItemRequest(
                        Descripcion:
                            request.Descripcion,
                        Cantidad: 1m,
                        PrecioUnitario:
                            request.PrecioUnitario,
                        TipoImpuesto: "gravado",
                        Iva: 21m
                    )
                ],
                Total: total,
                Moneda: "PES"
            );

        return _client.CrearComprobanteAsync(
            comprobante,
            cancellationToken
        );
    }

    public Task<JsonDocument> ConsultarAsync(
        long comprobanteId,
        CancellationToken cancellationToken = default
    )
    {
        return _client.ConsultarComprobanteAsync(
            comprobanteId,
            cancellationToken
        );
    }

    public Task<byte[]> DescargarPdfA4Async(
        long comprobanteId,
        CancellationToken cancellationToken = default
    )
    {
        return _client.DescargarPdfA4Async(
            comprobanteId,
            cancellationToken
        );
    }
}
