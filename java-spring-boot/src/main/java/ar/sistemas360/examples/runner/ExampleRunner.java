package ar.sistemas360.examples.runner;

import ar.sistemas360.examples.client.Sistemas360Client;
import ar.sistemas360.examples.config.Sistemas360Properties;
import ar.sistemas360.examples.model.ClienteRequest;
import ar.sistemas360.examples.model.CrearComprobanteRequest;
import ar.sistemas360.examples.model.ItemRequest;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.boot.CommandLineRunner;
import org.springframework.core.io.ByteArrayResource;
import org.springframework.stereotype.Component;

import java.math.BigDecimal;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.List;

@Component
public class ExampleRunner implements CommandLineRunner {

    private final Sistemas360Client client;
    private final Sistemas360Properties properties;
    private final ObjectMapper objectMapper;

    public ExampleRunner(
        Sistemas360Client client,
        Sistemas360Properties properties,
        ObjectMapper objectMapper
    ) {
        this.client = client;
        this.properties = properties;
        this.objectMapper = objectMapper;
    }

    @Override
    public void run(String... args) throws Exception {
        switch (properties.action()) {
            case "ping" -> ejecutarPing();
            case "crear" -> ejecutarCrear();
            case "consultar" -> ejecutarConsultar();
            case "descargar-pdf" -> ejecutarDescargarPdf();
            default -> throw new IllegalArgumentException(
                "Acción inválida. Usá ping, crear, consultar o descargar-pdf."
            );
        }
    }

    private void ejecutarPing() throws Exception {
        JsonNode response = client.ping();
        imprimirJson(response);
    }

    private void ejecutarCrear() throws Exception {
        CrearComprobanteRequest request =
            new CrearComprobanteRequest(
                "factura_b",
                "productos",
                LocalDate.now(),
                "venta_java_" + System.currentTimeMillis(),
                new ClienteRequest(
                    "dni",
                    "30111222",
                    "Cliente Demo",
                    5
                ),
                List.of(
                    new ItemRequest(
                        "Producto de ejemplo",
                        BigDecimal.ONE,
                        new BigDecimal("10000"),
                        "gravado",
                        new BigDecimal("21")
                    )
                ),
                new BigDecimal("12100"),
                "PES"
            );

        JsonNode response =
            client.crearComprobante(request);

        imprimirJson(response);
    }

    private void ejecutarConsultar() throws Exception {
        long comprobanteId = obtenerComprobanteId();

        JsonNode response =
            client.consultarComprobante(comprobanteId);

        imprimirJson(response);
    }

    private void ejecutarDescargarPdf() throws Exception {
        long comprobanteId = obtenerComprobanteId();

        ByteArrayResource pdf =
            client.descargarPdfA4(comprobanteId);

        Path archivo = Path.of(
            "comprobante-" + comprobanteId + ".pdf"
        );

        Files.write(
            archivo,
            pdf.getByteArray()
        );

        System.out.println(
            "PDF guardado en: " + archivo.toAbsolutePath()
        );
    }

    private long obtenerComprobanteId() {
        String value = properties.comprobanteId();

        if (value == null || value.isBlank()) {
            throw new IllegalArgumentException(
                "Falta SISTEMAS360_COMPROBANTE_ID."
            );
        }

        try {
            return Long.parseLong(value);
        } catch (NumberFormatException exception) {
            throw new IllegalArgumentException(
                "SISTEMAS360_COMPROBANTE_ID debe ser un número entero."
            );
        }
    }

    private void imprimirJson(JsonNode response)
        throws Exception {
        System.out.println(
            objectMapper
                .writerWithDefaultPrettyPrinter()
                .writeValueAsString(response)
        );
    }
}
