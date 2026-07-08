<?php

declare(strict_types=1);

require __DIR__ . '/config.php';

$response = apiRequest('GET', '/api/ping');
printJson($response['body']);
