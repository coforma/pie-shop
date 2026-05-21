<?php

declare(strict_types=1);

namespace Drupal\Tests\pie_shop\Functional;

use Drupal\Tests\BrowserTestBase;

/**
 * Functional tests for the order API endpoints.
 *
 * @group pie_shop
 */
class OrderApiTest extends BrowserTestBase {

  protected static $modules = ['pie_shop'];

  protected $defaultTheme = 'stark';

  /**
   * Tests that a valid order can be created via the API.
   */
  public function testCreateOrderSuccess(): void {
    $payload = [
      'pieType' => 'apple',
      'customer' => [
        'name' => 'Jane Doe',
        'email' => 'jane@example.com',
        'phone' => '+1-555-0123',
      ],
      'deliveryAddress' => [
        'street' => '123 Main St',
        'city' => 'Springfield',
        'state' => 'IL',
        'zip' => '62701',
      ],
    ];

    $this->drupalGet('/api/orders', [
      'query' => [],
    ]);

    // POST request via curl since BrowserTestBase doesn't support POST JSON natively
    $response = \Drupal::httpClient()->post($this->buildUrl('/api/orders'), [
      'json' => $payload,
      'http_errors' => FALSE,
    ]);

    $this->assertEquals(201, $response->getStatusCode());

    $data = json_decode((string) $response->getBody(), TRUE);
    $this->assertArrayHasKey('orderId', $data);
    $this->assertEquals('ORDERED', $data['status']);
  }

  /**
   * Tests that an invalid pie type returns a 400 error.
   */
  public function testCreateOrderInvalidPieType(): void {
    $payload = [
      'pieType' => 'chocolate',
      'customer' => ['name' => 'Test', 'email' => 'test@example.com', 'phone' => ''],
      'deliveryAddress' => ['street' => '1 Test St', 'city' => 'City', 'state' => 'IL', 'zip' => '12345'],
    ];

    $response = \Drupal::httpClient()->post($this->buildUrl('/api/orders'), [
      'json' => $payload,
      'http_errors' => FALSE,
    ]);

    $this->assertEquals(400, $response->getStatusCode());

    $data = json_decode((string) $response->getBody(), TRUE);
    $this->assertEquals('INVALID_PIE_TYPE', $data['error']);
  }

  /**
   * Tests that fetching a non-existent order returns 404.
   */
  public function testGetOrderNotFound(): void {
    $response = \Drupal::httpClient()->get($this->buildUrl('/api/orders/non-existent-id'), [
      'http_errors' => FALSE,
    ]);

    $this->assertEquals(404, $response->getStatusCode());
  }

  /**
   * Tests that the orders list endpoint returns an array.
   */
  public function testListOrdersReturnsArray(): void {
    $response = \Drupal::httpClient()->get($this->buildUrl('/api/orders'), [
      'http_errors' => FALSE,
    ]);

    $this->assertEquals(200, $response->getStatusCode());

    $data = json_decode((string) $response->getBody(), TRUE);
    $this->assertArrayHasKey('orders', $data);
    $this->assertIsArray($data['orders']);
  }

}
