using Microsoft.AspNetCore.Mvc;
using Sistemas360Example.Api.Models;
using Sistemas360Example.Api.Services;

namespace Sistemas360Example.Api.Controllers;

[ApiController]
[Route("api/facturacion")]
public sealed class ComprobantesController
    : ControllerBase
{
    private readonly ISistemas360Service _service;

    public ComprobantesController(
        ISistemas360Service service
    )
    {
        _service = service;
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping(
        CancellationToken cancellationToken
    )
    {
        using var response =
            await _service.PingAsync(
                cancellationToken
            );

        return Content(
            response.RootElement.GetRawText(),
            "application/json"
        );
    }

    [HttpPost("comprobantes")]
    public async Task<IActionResult> CrearComprobante(
        [FromBody] CrearComprobanteRequest request,
        CancellationToken cancellationToken
    )
    {
        using var response =
            await _service.CrearComprobanteAsync(
                request,
                cancellationToken
            );

        return Content(
            response.RootElement.GetRawText(),
            "application/json"
        );
    }

    [HttpGet("comprobantes/{id:long}")]
    public async Task<IActionResult> Consultar(
        long id,
        CancellationToken cancellationToken
    )
    {
        using var response =
            await _service.ConsultarAsync(
                id,
                cancellationToken
            );

        return Content(
            response.RootElement.GetRawText(),
            "application/json"
        );
    }

    [HttpGet("comprobantes/{id:long}/pdf")]
    public async Task<IActionResult> DescargarPdf(
        long id,
        CancellationToken cancellationToken
    )
    {
        byte[] pdf =
            await _service.DescargarPdfA4Async(
                id,
                cancellationToken
            );

        return File(
            pdf,
            "application/pdf",
            $"comprobante-{id}.pdf"
        );
    }
}
