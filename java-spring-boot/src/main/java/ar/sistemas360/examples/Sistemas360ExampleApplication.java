package ar.sistemas360.examples;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.ConfigurationPropertiesScan;

@SpringBootApplication
@ConfigurationPropertiesScan
public class Sistemas360ExampleApplication {

    public static void main(String[] args) {
        SpringApplication.run(
            Sistemas360ExampleApplication.class,
            args
        );
    }
}
