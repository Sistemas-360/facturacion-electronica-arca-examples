using System.Text.Json.Serialization;

namespace Sistemas360Example.Api.Models;

public sealed record ItemRequest(
    [property: JsonPropertyName("descripcion")]
    string Descripcion,

    [property: JsonPropertyName("cantidad")]
    decimal Cantidad,

    [property: JsonPropertyName("precio_unitario")]
    decimal PrecioUnitario,

    [property: JsonPropertyName("tipo_impuesto")]
    string TipoImpuesto,

    [property: JsonPropertyName("iva")]
    decimal Iva
);
