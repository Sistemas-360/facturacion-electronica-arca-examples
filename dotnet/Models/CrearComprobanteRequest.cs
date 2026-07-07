using System.Text.Json.Serialization;

namespace Sistemas360Example.Models;

public sealed record CrearComprobanteRequest(
    [property: JsonPropertyName("tipo_comprobante")]
    string TipoComprobante,

    [property: JsonPropertyName("concepto")]
    string Concepto,

    [property: JsonPropertyName("fecha")]
    string Fecha,

    [property: JsonPropertyName("referencia_externa")]
    string ReferenciaExterna,

    [property: JsonPropertyName("cliente")]
    ClienteRequest Cliente,

    [property: JsonPropertyName("items")]
    IReadOnlyList<ItemRequest> Items,

    [property: JsonPropertyName("total")]
    decimal Total,

    [property: JsonPropertyName("moneda")]
    string Moneda
);
