<?php

declare(strict_types=1);

$envPath = __DIR__ . DIRECTORY_SEPARATOR . '.env';

if (is_file($envPath)) {
    $lines = file($envPath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);

    foreach ($lines as $line) {
        $trimmed = trim($line);

        if ($trimmed === '' || str_starts_with($trimmed, '#') || !str_contains($trimmed, '=')) {
            continue;
        }

        [$name, $value] = explode('=', $trimmed, 2);
        $name = trim($name);
        $value = trim($value);

        if ($name !== '' && getenv($name) === false) {
            putenv(sprintf('%s=%s', $name, $value));
            $_ENV[$name] = $value;
        }
    }
}

$baseUrl = rtrim(getenv('SISTEMAS360_BASE_URL') ?: 'https://api.sistemas360.ar', '/');
$token = getenv('SISTEMAS360_TOKEN') ?: '';

if ($token === '') {
    fwrite(
        STDERR,
        "Falta SISTEMAS360_TOKEN. Copiá .env.example como .env y configurá tu token.\n"
    );
    exit(1);
}

function apiRequest(string $method, string $path, ?array $payload = null, string $accept = 'application/json'): array
{
    global $baseUrl;
    global $token;

    $headers = [
        'Authorization: Bearer ' . $token,
        'Accept: ' . $accept,
    ];

    if ($payload !== null) {
        $headers[] = 'Content-Type: application/json';
    }

    $ch = curl_init($baseUrl . $path);

    curl_setopt_array($ch, [
        CURLOPT_CUSTOMREQUEST => $method,
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_HTTPHEADER => $headers,
        CURLOPT_TIMEOUT => 30,
    ]);

    if ($payload !== null) {
        curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($payload, JSON_UNESCAPED_UNICODE));
    }

    $responseBody = curl_exec($ch);

    if ($responseBody === false) {
        $message = curl_error($ch);
        curl_close($ch);
        throw new RuntimeException('Error de red: ' . $message);
    }

    $statusCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $contentType = curl_getinfo($ch, CURLINFO_CONTENT_TYPE) ?: '';
    curl_close($ch);

    if ($statusCode < 200 || $statusCode >= 300) {
        throw new RuntimeException(sprintf('Error %d: %s', $statusCode, $responseBody));
    }

    return [
        'status' => $statusCode,
        'content_type' => $contentType,
        'body' => $responseBody,
    ];
}

function printJson(string $body): void
{
    $decoded = json_decode($body, true);

    if (!is_array($decoded)) {
        echo $body . PHP_EOL;
        return;
    }

    echo json_encode($decoded, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE) . PHP_EOL;
}
