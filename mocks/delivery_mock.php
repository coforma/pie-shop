<?php

/**
 * Mock Delivery Service
 *
 * Simulates drone delivery with distance-based timing.
 * Run with: php -S 0.0.0.0:8083 mocks/delivery_mock.php
 */

$uri = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);
$method = $_SERVER['REQUEST_METHOD'];

header('Content-Type: application/json');

if ($method === 'POST' && $uri === '/api/v1/deliveries') {
    $body = json_decode(file_get_contents('php://input'), true);

    if (rand(1, 10) === 1) {
        http_response_code(503);
        echo json_encode(['error' => 'NO_DRONES_AVAILABLE', 'message' => 'All drones are currently deployed']);
        exit;
    }

    $deliveryId = 'del_' . bin2hex(random_bytes(4));
    $droneId = 'drone-' . rand(1, 12);
    $etaMinutes = rand(10, 30);
    $eta = time() + ($etaMinutes * 60);

    $deliveryFile = sys_get_temp_dir() . '/pie_shop_delivery_' . $deliveryId . '.json';
    file_put_contents($deliveryFile, json_encode([
        'deliveryId' => $deliveryId,
        'droneId' => $droneId,
        'status' => 'DISPATCHED',
        'destination' => $body['destination'] ?? [],
        'eta' => $eta,
        'location' => ['lat' => 39.7817, 'lng' => -89.6501],
    ]));

    http_response_code(202);
    echo json_encode([
        'deliveryId' => $deliveryId,
        'droneId' => $droneId,
        'eta' => date('c', $eta),
    ]);
    exit;
}

if ($method === 'GET' && preg_match('#^/api/v1/deliveries/(.+)$#', $uri, $matches)) {
    $deliveryId = $matches[1];
    $deliveryFile = sys_get_temp_dir() . '/pie_shop_delivery_' . $deliveryId . '.json';

    if (!file_exists($deliveryFile)) {
        http_response_code(404);
        echo json_encode(['error' => 'DELIVERY_NOT_FOUND']);
        exit;
    }

    $delivery = json_decode(file_get_contents($deliveryFile), true);

    if (time() >= $delivery['eta']) {
        $delivery['status'] = 'DELIVERED';
        $delivery['location'] = $delivery['destination'];
        file_put_contents($deliveryFile, json_encode($delivery));
    } else {
        $delivery['status'] = 'IN_TRANSIT';
    }

    echo json_encode($delivery);
    exit;
}

http_response_code(404);
echo json_encode(['error' => 'NOT_FOUND']);
