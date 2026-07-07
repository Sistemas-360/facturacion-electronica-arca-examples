package ar.sistemas360.examples.client;

import ar.sistemas360.examples.config.Sistemas360Properties;
import ar.sistemas360.examples.model.CrearComprobanteRequest;
import com.fasterxml.jackson.databind.JsonNode;
import org.springframework.core.io.ByteArrayResource;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestClient;

@Component
public class Sistemas360Client {

    private final RestClient restClient;

    public Sistemas360Client(
        RestClient.Builder builder,
        Sistemas360Properties properties
    ) {
        if (properties.token() == null || properties.token().isBlank()) {
            throw new IllegalStateException(
                "Falta SISTEMAS360_TOKEN."
            );
        }

        this.restClient = builder
            .baseUrl(properties.baseUrl())
            .defaultHeader(
                HttpHeaders.AUTHORIZATION,
                "Bearer " + properties.token()
            )
            .build();
    }

    public JsonNode ping() {
        return restClient
            .get()
            .uri("/api/ping")
            .accept(MediaType.APPLICATION_JSON)
            .retrieve()
            .body(JsonNode.class);
    }

    public JsonNode crearComprobante(
        CrearComprobanteRequest request
    ) {
        return restClient
            .post()
            .uri("/api/comprobantes")
            .contentType(MediaType.APPLICATION_JSON)
            .accept(MediaType.APPLICATION_JSON)
            .body(request)
            .retrieve()
            .body(JsonNode.class);
    }

    public JsonNode consultarComprobante(
        long comprobanteId
    ) {
        return restClient
            .get()
            .uri("/api/comprobantes/{id}", comprobanteId)
            .accept(MediaType.APPLICATION_JSON)
            .retrieve()
            .body(JsonNode.class);
    }

    public ByteArrayResource descargarPdfA4(
        long comprobanteId
    ) {
        byte[] bytes = restClient
            .get()
            .uri(
                "/api/comprobantes/{id}/imprimir-a4",
                comprobanteId
            )
            .accept(MediaType.APPLICATION_PDF)
            .retrieve()
            .body(byte[].class);

        if (bytes == null || bytes.length == 0) {
            throw new IllegalStateException(
                "La API devolvió un PDF vacío."
            );
        }

        return new ByteArrayResource(bytes);
    }
}
