<?php

declare(strict_types=1);

require __DIR__ . '/config.php';

if ($argc < 2 || !ctype_digit($argv[1]) || (int) $argv[1] <= 0) {
    fwrite(STDERR, "Uso: php descargar_pdf.php ID_COMPROBANTE\n");
    exit(1);
}

$comprobanteId = $argv[1];
$response = apiRequest('GET', '/api/comprobantes/' . $comprobanteId . '/imprimir-a4', null, 'application/pdf');

if (!str_starts_with(strtolower($response['content_type']), 'application/pdf')) {
    throw new RuntimeException(
        'La API no devolvió un PDF válido. Content-Type recibido: ' . $response['content_type']
    );
}

$outputPath = __DIR__ . DIRECTORY_SEPARATOR . 'comprobante-' . $comprobanteId . '.pdf';
file_put_contents($outputPath, $response['body']);

echo 'PDF guardado en: ' . $outputPath . PHP_EOL;
