<?php

declare(strict_types=1);

namespace Drupal\Tests\pie_shop\Unit;

use Drupal\pie_shop\Service\FruitPickerClient;
use Drupal\Tests\UnitTestCase;
use GuzzleHttp\ClientInterface;
use GuzzleHttp\Psr7\Response;
use Psr\Log\LoggerInterface;

/**
 * Unit tests for the fruit picker service client.
 *
 * @group pie_shop
 */
class FruitPickerClientTest extends UnitTestCase {

  /**
   * Tests successful fruit picking job submission.
   */
  public function testPickFruitReturnsJobId(): void {
    $responseBody = json_encode([
      'jobId' => 'pick_abc123',
      'estimatedCompletion' => '2025-12-17T16:35:00Z',
    ]);

    $httpClient = $this->createMock(ClientInterface::class);
    $httpClient->expects($this->once())
      ->method('request')
      ->willReturn(new Response(202, [], $responseBody));

    $configFactory = $this->createMock(\Drupal\Core\Config\ConfigFactoryInterface::class);
    $logger = $this->createMock(LoggerInterface::class);

    $client = new FruitPickerClient($httpClient, $configFactory, $logger);
    $result = $client->pickFruit('apple', 6);

    $this->assertEquals('pick_abc123', $result['jobId']);
  }

  /**
   * Tests that service errors are wrapped in RuntimeException.
   */
  public function testPickFruitThrowsOnServiceError(): void {
    $this->expectException(\RuntimeException::class);

    $httpClient = $this->createMock(ClientInterface::class);
    $httpClient->method('request')
      ->willThrowException(new \Exception('Connection refused'));

    $configFactory = $this->createMock(\Drupal\Core\Config\ConfigFactoryInterface::class);
    $logger = $this->createMock(LoggerInterface::class);

    $client = new FruitPickerClient($httpClient, $configFactory, $logger);
    $client->pickFruit('apple', 6);
  }

  /**
   * Tests job status retrieval.
   */
  public function testGetJobStatusReturnsStatus(): void {
    $responseBody = json_encode([
      'jobId' => 'pick_abc123',
      'status' => 'COMPLETED',
      'fruits' => [],
    ]);

    $httpClient = $this->createMock(ClientInterface::class);
    $httpClient->method('request')
      ->willReturn(new Response(200, [], $responseBody));

    $configFactory = $this->createMock(\Drupal\Core\Config\ConfigFactoryInterface::class);
    $logger = $this->createMock(LoggerInterface::class);

    $client = new FruitPickerClient($httpClient, $configFactory, $logger);
    $result = $client->getJobStatus('pick_abc123');

    $this->assertEquals('COMPLETED', $result['status']);
  }

}
