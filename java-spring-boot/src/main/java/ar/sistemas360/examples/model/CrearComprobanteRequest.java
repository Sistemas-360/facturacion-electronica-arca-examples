package ar.sistemas360.examples.model;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

public record CrearComprobanteRequest(
    @JsonProperty("tipo_comprobante")
    String tipoComprobante,

    String concepto,

    LocalDate fecha,

    @JsonProperty("referencia_externa")
    String referenciaExterna,

    ClienteRequest cliente,

    List<ItemRequest> items,

    BigDecimal total,

    String moneda
) {
}
