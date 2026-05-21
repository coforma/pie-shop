<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Service;

use Drupal\Core\Config\ConfigFactoryInterface;
use Psr\Log\LoggerInterface;

/**
 * Provides access to the pie recipe catalog stored in MongoDB.
 */
class RecipeCatalog {

  // TODO: Move to config
  private string $mongoUri = 'mongodb://mongo:27017';
  private string $dbName = 'pie_shop';

  private ?\MongoDB\Collection $collection = NULL;

  public function __construct(
    private readonly ConfigFactoryInterface $configFactory,
    private readonly LoggerInterface $logger,
  ) {}

  /**
   * Retrieves a recipe by pie type.
   *
   * @param string $pieType
   *   The pie type identifier (e.g., 'apple').
   *
   * @return array|null
   *   Recipe data or null if not found.
   */
  public function getRecipe(string $pieType): ?array {
    try {
      $collection = $this->getCollection();
      $doc = $collection->findOne(['_id' => $pieType]);

      if (!$doc) {
        return NULL;
      }

      return (array) $doc;
    }
    catch (\Exception $e) {
      $this->logger->error('Failed to fetch recipe for @type: @msg', [
        '@type' => $pieType,
        '@msg' => $e->getMessage(),
      ]);
      return NULL;
    }
  }

  /**
   * Returns all available pie types from the catalog.
   *
   * @return array
   *   List of pie type identifiers.
   */
  public function getAvailableTypes(): array {
    try {
      $collection = $this->getCollection();
      $cursor = $collection->find([], ['projection' => ['_id' => 1]]);

      return array_column(iterator_to_array($cursor), '_id');
    }
    catch (\Exception $e) {
      $this->logger->error('Failed to fetch pie types: @msg', ['@msg' => $e->getMessage()]);
      return ['apple', 'cherry', 'pumpkin', 'pecan', 'blueberry'];
    }
  }

  /**
   * Returns the MongoDB collection, initializing the connection if needed.
   */
  private function getCollection(): \MongoDB\Collection {
    if ($this->collection === NULL) {
      $client = new \MongoDB\Client($this->mongoUri);
      $this->collection = $client->selectCollection($this->dbName, 'recipes');
    }

    return $this->collection;
  }

}
