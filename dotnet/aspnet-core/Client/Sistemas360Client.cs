using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Sistemas360Example.Api.Models;

namespace Sistemas360Example.Api.Client;

public sealed class Sistemas360Client
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public Sistemas360Client(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task<JsonDocument> PingAsync(
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response =
            await _httpClient.GetAsync(
                "api/ping",
                cancellationToken
            );

        return await ReadJsonResponseAsync(
            response,
            cancellationToken
        );
    }

    public async Task<JsonDocument> CrearComprobanteAsync(
        CrearComprobanteRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "api/comprobantes",
                request,
                _jsonOptions,
                cancellationToken
            );

        return await ReadJsonResponseAsync(
            response,
            cancellationToken
        );
    }

    public async Task<JsonDocument> ConsultarComprobanteAsync(
        long comprobanteId,
        CancellationToken cancellationToken = default
    )
    {
        ValidarComprobanteId(comprobanteId);

        using HttpResponseMessage response =
            await _httpClient.GetAsync(
                $"api/comprobantes/{comprobanteId}",
                cancellationToken
            );

        return await ReadJsonResponseAsync(
            response,
            cancellationToken
        );
    }

    public async Task<byte[]> DescargarPdfA4Async(
        long comprobanteId,
        CancellationToken cancellationToken = default
    )
    {
        ValidarComprobanteId(comprobanteId);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/comprobantes/{comprobanteId}/imprimir-a4"
        );

        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/pdf"
            )
        );

        using HttpResponseMessage response =
            await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );

        if (!response.IsSuccessStatusCode)
        {
            await ThrowApiExceptionAsync(
                response,
                cancellationToken
            );
        }

        string? contentType =
            response.Content.Headers.ContentType?.MediaType;

        if (
            !string.Equals(
                contentType,
                "application/pdf",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            string body =
                await response.Content.ReadAsStringAsync(
                    cancellationToken
                );

            throw new InvalidOperationException(
                $"La API no devolvió un PDF. " +
                $"Content-Type recibido: {contentType ?? "desconocido"}. " +
                $"Respuesta: {body}"
            );
        }

        return await response.Content.ReadAsByteArrayAsync(
            cancellationToken
        );
    }

    private static async Task<JsonDocument> ReadJsonResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        if (!response.IsSuccessStatusCode)
        {
            await ThrowApiExceptionAsync(
                response,
                cancellationToken
            );
        }

        Stream stream =
            await response.Content.ReadAsStreamAsync(
                cancellationToken
            );

        return await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken
        );
    }

    private static async Task ThrowApiExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        string body =
            await response.Content.ReadAsStringAsync(
                cancellationToken
            );

        throw new HttpRequestException(
            $"Error {(int)response.StatusCode} " +
            $"{response.ReasonPhrase}: {body}",
            null,
            response.StatusCode
        );
    }

    private static void ValidarComprobanteId(
        long comprobanteId
    )
    {
        if (comprobanteId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(comprobanteId),
                "El ID debe ser mayor que cero."
            );
        }
    }
}
