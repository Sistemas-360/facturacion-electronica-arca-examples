namespace Sistemas360Example.Api.Models;

public sealed record CrearFacturaDemoRequest(
    string DocumentoNumero,
    string RazonSocial,
    string Descripcion,
    decimal PrecioUnitario
);
