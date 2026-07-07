using System.Text.Json;
using Sistemas360Example.Api.Models;

namespace Sistemas360Example.Api.Services;

public interface ISistemas360Service
{
    Task<JsonDocument> PingAsync(
        CancellationToken cancellationToken = default
    );

    Task<JsonDocument> CrearFacturaDemoAsync(
        CrearFacturaDemoRequest request,
        CancellationToken cancellationToken = default
    );

    Task<JsonDocument> ConsultarAsync(
        long comprobanteId,
        CancellationToken cancellationToken = default
    );

    Task<byte[]> DescargarPdfA4Async(
        long comprobanteId,
        CancellationToken cancellationToken = default
    );
}
