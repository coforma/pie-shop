<?php

/**
 * Mock Fruit Picker Service
 *
 * Simulates the robot fruit picker with realistic delays and occasional failures.
 * Run with: php -S 0.0.0.0:8081 mocks/fruit_picker_mock.php
 */

$jobs = [];

$uri = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);
$method = $_SERVER['REQUEST_METHOD'];

header('Content-Type: application/json');

if ($method === 'POST' && $uri === '/api/v1/pick-fruit') {
    $body = json_decode(file_get_contents('php://input'), true);

    // Simulate occasional failures
    if (rand(1, 10) === 1) {
        http_response_code(503);
        echo json_encode(['error' => 'SERVICE_UNAVAILABLE', 'message' => 'Picker arm malfunction']);
        exit;
    }

    $jobId = 'pick_' . bin2hex(random_bytes(4));
    $completionTime = time() + rand(30, 60);

    // Store job in temp file (stateless mock workaround)
    $jobFile = sys_get_temp_dir() . '/pie_shop_job_' . $jobId . '.json';
    file_put_contents($jobFile, json_encode([
        'jobId' => $jobId,
        'status' => 'IN_PROGRESS',
        'fruitType' => $body['fruitType'] ?? 'apple',
        'quantity' => $body['quantity'] ?? 6,
        'completionTime' => $completionTime,
    ]));

    http_response_code(202);
    echo json_encode([
        'jobId' => $jobId,
        'estimatedCompletion' => date('c', $completionTime),
    ]);
    exit;
}

if ($method === 'GET' && preg_match('#^/api/v1/jobs/(.+)$#', $uri, $matches)) {
    $jobId = $matches[1];
    $jobFile = sys_get_temp_dir() . '/pie_shop_job_' . $jobId . '.json';

    if (!file_exists($jobFile)) {
        http_response_code(404);
        echo json_encode(['error' => 'JOB_NOT_FOUND']);
        exit;
    }

    $job = json_decode(file_get_contents($jobFile), true);

    if (time() >= $job['completionTime']) {
        $job['status'] = 'COMPLETED';
        $job['fruits'] = array_fill(0, $job['quantity'], ['type' => $job['fruitType'], 'quality' => 'premium']);
        file_put_contents($jobFile, json_encode($job));
    }

    echo json_encode($job);
    exit;
}

http_response_code(404);
echo json_encode(['error' => 'NOT_FOUND']);
