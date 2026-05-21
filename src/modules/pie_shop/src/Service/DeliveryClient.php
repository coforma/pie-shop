<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Service;

use GuzzleHttp\ClientInterface;
use Drupal\Core\Config\ConfigFactoryInterface;
use Psr\Log\LoggerInterface;

/**
 * Client for the drone delivery service.
 */
class DeliveryClient {

  private string $baseUrl = 'http://delivery-mock:8083';
  private string $apiKey = 'delivery-api-key-def456';

  public function __construct(
    private readonly ClientInterface $httpClient,
    private readonly ConfigFactoryInterface $configFactory,
    private readonly LoggerInterface $logger,
  ) {}

  /**
   * Schedules a drone delivery.
   *
   * @param array $package
   *   Package details (type, size).
   * @param array $destination
   *   Delivery address.
   * @param string $window
   *   ISO 8601 time interval for delivery window.
   *
   * @return array
   *   Response containing deliveryId, droneId, and eta.
   */
  public function scheduleDelivery(array $package, array $destination, string $window): array {
    try {
      $response = $this->httpClient->request('POST', $this->baseUrl . '/api/v1/deliveries', [
        'json' => [
          'package' => $package,
          'destination' => $destination,
          'window' => $window,
        ],
        'headers' => ['X-API-Key' => $this->apiKey],
        'timeout' => 10,
      ]);

      return json_decode((string) $response->getBody(), TRUE);
    }
    catch (\Exception $e) {
      $this->logger->error('Delivery scheduling failed: @msg', ['@msg' => $e->getMessage()]);
      throw new \RuntimeException('Delivery service unavailable: ' . $e->getMessage());
    }
  }

  /**
   * Retrieves the current status of a delivery.
   */
  public function getDeliveryStatus(string $deliveryId): array {
    try {
      $response = $this->httpClient->request('GET', $this->baseUrl . '/api/v1/deliveries/' . $deliveryId, [
        'headers' => ['X-API-Key' => $this->apiKey],
        'timeout' => 10,
      ]);

      return json_decode((string) $response->getBody(), TRUE);
    }
    catch (\Exception $e) {
      $this->logger->warning('Delivery status check failed for @id: @msg', [
        '@id' => $deliveryId,
        '@msg' => $e->getMessage(),
      ]);
      throw $e;
    }
  }

}
