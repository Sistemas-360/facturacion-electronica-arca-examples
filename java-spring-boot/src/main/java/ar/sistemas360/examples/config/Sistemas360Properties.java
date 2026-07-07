package ar.sistemas360.examples.config;

import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties(prefix = "sistemas360")
public record Sistemas360Properties(
    String baseUrl,
    String token,
    String action,
    String comprobanteId
) {
    public Sistemas360Properties {
        baseUrl = baseUrl == null || baseUrl.isBlank()
            ? "https://api.sistemas360.ar"
            : baseUrl.replaceAll("/+$", "");

        action = action == null || action.isBlank()
            ? "ping"
            : action.trim().toLowerCase();
    }
}
