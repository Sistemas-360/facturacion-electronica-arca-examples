<?php

declare(strict_types=1);

require __DIR__ . '/config.php';

if ($argc < 2 || !ctype_digit($argv[1]) || (int) $argv[1] <= 0) {
    fwrite(STDERR, "Uso: php consultar_comprobante.php ID_COMPROBANTE\n");
    exit(1);
}

$response = apiRequest('GET', '/api/comprobantes/' . $argv[1]);
printJson($response['body']);
