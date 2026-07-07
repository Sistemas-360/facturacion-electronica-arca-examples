using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Sistemas360Example.Models;

namespace Sistemas360Example.Client;

public sealed class Sistemas360Client : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public Sistemas360Client(
        string baseUrl,
        string token
    )
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException(
                "La URL base no puede estar vacía.",
                nameof(baseUrl)
            );
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                "El token no puede estar vacío.",
                nameof(token)
            );
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(
                baseUrl.TrimEnd('/') + "/",
                UriKind.Absolute
            ),
            Timeout = TimeSpan.FromSeconds(30)
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token
            );

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"
            )
        );

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

    public async Task<string> DescargarPdfA4Async(
        long comprobanteId,
        string? outputDirectory = null,
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
            string body = await response.Content.ReadAsStringAsync(
                cancellationToken
            );

            throw new InvalidOperationException(
                $"La API no devolvió un PDF. " +
                $"Content-Type recibido: {contentType ?? "desconocido"}. " +
                $"Respuesta: {body}"
            );
        }

        string directory = string.IsNullOrWhiteSpace(outputDirectory)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(outputDirectory);

        Directory.CreateDirectory(directory);

        string fileName =
            $"comprobante-{comprobanteId}.pdf";

        string finalPath =
            Path.Combine(directory, fileName);

        string temporaryPath =
            finalPath + ".tmp";

        try
        {
            await using Stream source =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken
                );

            await using FileStream destination =
                File.Create(temporaryPath);

            await source.CopyToAsync(
                destination,
                cancellationToken
            );

            await destination.FlushAsync(
                cancellationToken
            );

            File.Move(
                temporaryPath,
                finalPath,
                overwrite: true
            );
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }

        return finalPath;
    }

    private async Task<JsonDocument> ReadJsonResponseAsync(
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

        JsonDocument? document =
            await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken
            );

        return document;
    }

    private static async Task ThrowApiExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        string body = await response.Content.ReadAsStringAsync(
            cancellationToken
        );

        throw new HttpRequestException(
            $"Error {(int)response.StatusCode} " +
            $"{response.ReasonPhrase}: {body}",
            inner: null,
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
                "El ID del comprobante debe ser mayor que cero."
            );
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
