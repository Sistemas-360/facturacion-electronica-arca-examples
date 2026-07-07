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

    public Task<JsonDocument> CrearComprobanteAsync(
        CrearComprobanteRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ValidarComprobante(request);

        return _client.CrearComprobanteAsync(
            request,
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

    private static void ValidarComprobante(
        CrearComprobanteRequest request
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.TipoComprobante))
        {
            throw new ArgumentException(
                "tipo_comprobante es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Concepto))
        {
            throw new ArgumentException(
                "concepto es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Fecha))
        {
            throw new ArgumentException(
                "fecha es obligatoria."
            );
        }

        if (string.IsNullOrWhiteSpace(request.ReferenciaExterna))
        {
            throw new ArgumentException(
                "referencia_externa es obligatoria."
            );
        }

        if (request.Cliente is null)
        {
            throw new ArgumentException(
                "cliente es obligatorio."
            );
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException(
                "Debe incluir al menos un ítem."
            );
        }

        if (request.Total <= 0)
        {
            throw new ArgumentException(
                "total debe ser mayor que cero."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Moneda))
        {
            throw new ArgumentException(
                "moneda es obligatoria."
            );
        }
    }
}
