package ar.sistemas360.examples.model;

import java.math.BigDecimal;

public record ItemRequest(
    String descripcion,
    BigDecimal cantidad,
    BigDecimal precioUnitario,
    String tipoImpuesto,
    BigDecimal iva
) {
}
