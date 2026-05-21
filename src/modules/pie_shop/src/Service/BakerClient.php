<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Service;

use GuzzleHttp\ClientInterface;
use Drupal\Core\Config\ConfigFactoryInterface;
use Psr\Log\LoggerInterface;

/**
 * Client for the robot baker service.
 */
class BakerClient {

  private string $baseUrl = 'http://baker-mock:8082';
  private string $apiKey = 'baker-api-key-xyz789';

  public function __construct(
    private readonly ClientInterface $httpClient,
    private readonly ConfigFactoryInterface $configFactory,
    private readonly LoggerInterface $logger,
  ) {}

  /**
   * Submits a baking job to the baker service.
   *
   * @param string $pieType
   *   The type of pie to bake.
   * @param int $temperature
   *   Baking temperature in Fahrenheit.
   * @param int $duration
   *   Baking duration in minutes.
   *
   * @return array
   *   Response containing jobId, ovenId, and estimatedCompletion.
   */
  public function bake(string $pieType, int $temperature, int $duration): array {
    try {
      $response = $this->httpClient->request('POST', $this->baseUrl . '/api/v1/bake', [
        'json' => [
          'pieType' => $pieType,
          'temperature' => $temperature,
          'duration' => $duration,
        ],
        'headers' => ['X-API-Key' => $this->apiKey],
        'timeout' => 15,
      ]);

      return json_decode((string) $response->getBody(), TRUE);
    }
    catch (\Exception $e) {
      $this->logger->error('Baker request failed: @msg', ['@msg' => $e->getMessage()]);
      throw new \RuntimeException('Baker service unavailable: ' . $e->getMessage());
    }
  }

  /**
   * Retrieves the status of a baking job.
   */
  public function getJobStatus(string $jobId): array {
    $response = $this->httpClient->request('GET', $this->baseUrl . '/api/v1/jobs/' . $jobId, [
      'headers' => ['X-API-Key' => $this->apiKey],
      'timeout' => 10,
    ]);

    return json_decode((string) $response->getBody(), TRUE);
  }

}
