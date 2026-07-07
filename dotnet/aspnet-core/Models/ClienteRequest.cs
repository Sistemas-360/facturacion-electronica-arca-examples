using System.Text.Json.Serialization;

namespace Sistemas360Example.Api.Models;

public sealed record ClienteRequest(
    [property: JsonPropertyName("documento_tipo")]
    string DocumentoTipo,

    [property: JsonPropertyName("documento_numero")]
    string? DocumentoNumero,

    [property: JsonPropertyName("razon_social")]
    string RazonSocial,

    [property: JsonPropertyName("condicion_iva_receptor_id")]
    int CondicionIvaReceptorId
);
