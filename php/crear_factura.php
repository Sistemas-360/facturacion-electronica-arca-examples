<?php

declare(strict_types=1);

require __DIR__ . '/config.php';

$payload = [
    'tipo_comprobante' => 'factura_b',
    'concepto' => 'productos',
    'fecha' => date('Y-m-d'),
    'referencia_externa' => 'venta_php_' . time(),
    'cliente' => [
        'documento_tipo' => 'dni',
        'documento_numero' => '30111222',
        'razon_social' => 'Cliente Demo',
        'condicion_iva_receptor_id' => 5,
    ],
    'items' => [
        [
            'descripcion' => 'Producto de ejemplo',
            'cantidad' => 1,
            'precio_unitario' => 10000,
            'tipo_impuesto' => 'gravado',
            'iva' => 21,
        ],
    ],
    'total' => 12100,
    'moneda' => 'PES',
];

$response = apiRequest('POST', '/api/comprobantes', $payload);
printJson($response['body']);
