<?php

/**
 * Mock Baker Service
 *
 * Simulates the robot baker with realistic baking delays.
 * Run with: php -S 0.0.0.0:8082 mocks/baker_mock.php
 */

$uri = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);
$method = $_SERVER['REQUEST_METHOD'];

header('Content-Type: application/json');

if ($method === 'POST' && $uri === '/api/v1/bake') {
    $body = json_decode(file_get_contents('php://input'), true);

    if (rand(1, 10) === 1) {
        http_response_code(503);
        echo json_encode(['error' => 'OVEN_UNAVAILABLE', 'message' => 'All ovens currently occupied']);
        exit;
    }

    $jobId = 'bake_' . bin2hex(random_bytes(4));
    $duration = (int) ($body['duration'] ?? 45);
    // Simulate faster for demo purposes (scale down by 60x)
    $completionTime = time() + ($duration * 10);
    $ovenId = 'oven-' . rand(1, 6);

    $jobFile = sys_get_temp_dir() . '/pie_shop_job_' . $jobId . '.json';
    file_put_contents($jobFile, json_encode([
        'jobId' => $jobId,
        'status' => 'IN_PROGRESS',
        'pieType' => $body['pieType'] ?? 'apple',
        'ovenId' => $ovenId,
        'completionTime' => $completionTime,
        'progress' => 0,
    ]));

    http_response_code(202);
    echo json_encode([
        'jobId' => $jobId,
        'ovenId' => $ovenId,
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
        $job['progress'] = 100;
    } else {
        $elapsed = time() - ($job['completionTime'] - 450);
        $job['progress'] = min(99, (int) (($elapsed / 450) * 100));
    }

    file_put_contents($jobFile, json_encode($job));
    echo json_encode($job);
    exit;
}

http_response_code(404);
echo json_encode(['error' => 'NOT_FOUND']);
