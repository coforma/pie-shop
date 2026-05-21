<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Service;

use GuzzleHttp\ClientInterface;
use Drupal\Core\Config\ConfigFactoryInterface;
use Psr\Log\LoggerInterface;

/**
 * Client for the robot fruit picker service.
 */
class FruitPickerClient {

  // TODO: Move to config
  private string $baseUrl = 'http://fruit-picker-mock:8081';
  private string $apiKey = 'picker-api-key-abc123';

  public function __construct(
    private readonly ClientInterface $httpClient,
    private readonly ConfigFactoryInterface $configFactory,
    private readonly LoggerInterface $logger,
  ) {}

  /**
   * Submits a fruit picking job.
   *
   * @param string $fruitType
   *   The type of fruit to pick.
   * @param int $quantity
   *   Number of fruits needed.
   *
   * @return array
   *   Response containing jobId and estimatedCompletion.
   *
   * @throws \RuntimeException
   *   On service failure.
   */
  public function pickFruit(string $fruitType, int $quantity): array {
    try {
      $response = $this->httpClient->request('POST', $this->baseUrl . '/api/v1/pick-fruit', [
        'json' => [
          'fruitType' => $fruitType,
          'quantity' => $quantity,
          'quality' => 'premium',
        ],
        'headers' => [
          'X-API-Key' => $this->apiKey,
        ],
        'timeout' => 10,
      ]);

      return json_decode((string) $response->getBody(), TRUE);
    }
    catch (\Exception $e) {
      // TODO: Implement circuit breaker pattern
      $this->logger->error('Fruit picker request failed: @msg', ['@msg' => $e->getMessage()]);
      throw new \RuntimeException('Fruit picker service unavailable: ' . $e->getMessage());
    }
  }

  /**
   * Retrieves the status of a picking job.
   *
   * @param string $jobId
   *   The job ID returned from pickFruit().
   *
   * @return array
   *   Job status data.
   */
  public function getJobStatus(string $jobId): array {
    try {
      $response = $this->httpClient->request('GET', $this->baseUrl . '/api/v1/jobs/' . $jobId, [
        'headers' => ['X-API-Key' => $this->apiKey],
        'timeout' => 10,
      ]);

      return json_decode((string) $response->getBody(), TRUE);
    }
    catch (\Exception $e) {
      $this->logger->warning('Job status check failed for @job: @msg', [
        '@job' => $jobId,
        '@msg' => $e->getMessage(),
      ]);
      throw $e;
    }
  }

}
