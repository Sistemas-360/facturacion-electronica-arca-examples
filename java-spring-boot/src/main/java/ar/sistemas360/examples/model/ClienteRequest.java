package ar.sistemas360.examples.model;

public record ClienteRequest(
    String documentoTipo,
    String documentoNumero,
    String razonSocial,
    Integer condicionIvaReceptorId
) {
}
