#!/usr/bin/env php
<?php

/**
 * Seeds the MongoDB recipe catalog with initial pie data.
 *
 * Usage: php migrations/002_seed_recipes.php
 */

require_once __DIR__ . '/../vendor/autoload.php';

$mongoUri = getenv('MONGO_URI') ?: 'mongodb://localhost:27017';
$client = new \MongoDB\Client($mongoUri);
$collection = $client->pie_shop->recipes;

$recipes = [
  [
    '_id' => 'apple',
    'name' => 'Classic Apple Pie',
    'description' => 'Traditional American apple pie with cinnamon',
    'bakingTime' => 45,
    'bakingTemp' => 375,
    'ingredients' => [
      ['item' => 'apples', 'quantity' => 6, 'unit' => 'whole'],
      ['item' => 'sugar', 'quantity' => 0.75, 'unit' => 'cup'],
      ['item' => 'cinnamon', 'quantity' => 1, 'unit' => 'tsp'],
      ['item' => 'butter', 'quantity' => 2, 'unit' => 'tbsp'],
      ['item' => 'flour', 'quantity' => 2.5, 'unit' => 'cup'],
      ['item' => 'salt', 'quantity' => 1, 'unit' => 'tsp'],
    ],
    'prepSteps' => ['wash_fruit', 'peel_fruit', 'slice_fruit', 'make_dough', 'assemble'],
    'difficulty' => 'medium',
  ],
  [
    '_id' => 'cherry',
    'name' => 'Sweet Cherry Pie',
    'description' => 'Classic cherry pie with lattice crust',
    'bakingTime' => 40,
    'bakingTemp' => 400,
    'ingredients' => [
      ['item' => 'cherries', 'quantity' => 4, 'unit' => 'cup'],
      ['item' => 'sugar', 'quantity' => 1, 'unit' => 'cup'],
      ['item' => 'cornstarch', 'quantity' => 3, 'unit' => 'tbsp'],
      ['item' => 'butter', 'quantity' => 2, 'unit' => 'tbsp'],
      ['item' => 'flour', 'quantity' => 2.5, 'unit' => 'cup'],
    ],
    'prepSteps' => ['wash_fruit', 'pit_fruit', 'make_dough', 'assemble'],
    'difficulty' => 'medium',
  ],
  [
    '_id' => 'pumpkin',
    'name' => 'Spiced Pumpkin Pie',
    'description' => 'Autumn classic with warm spices',
    'bakingTime' => 55,
    'bakingTemp' => 350,
    'ingredients' => [
      ['item' => 'pumpkin_puree', 'quantity' => 2, 'unit' => 'cup'],
      ['item' => 'eggs', 'quantity' => 2, 'unit' => 'whole'],
      ['item' => 'brown_sugar', 'quantity' => 0.75, 'unit' => 'cup'],
      ['item' => 'cinnamon', 'quantity' => 1, 'unit' => 'tsp'],
      ['item' => 'nutmeg', 'quantity' => 0.5, 'unit' => 'tsp'],
      ['item' => 'flour', 'quantity' => 1.5, 'unit' => 'cup'],
    ],
    'prepSteps' => ['prepare_filling', 'make_dough', 'assemble'],
    'difficulty' => 'easy',
  ],
  [
    '_id' => 'pecan',
    'name' => 'Southern Pecan Pie',
    'description' => 'Rich and sweet Southern-style pecan pie',
    'bakingTime' => 50,
    'bakingTemp' => 350,
    'ingredients' => [
      ['item' => 'pecans', 'quantity' => 2, 'unit' => 'cup'],
      ['item' => 'corn_syrup', 'quantity' => 1, 'unit' => 'cup'],
      ['item' => 'eggs', 'quantity' => 3, 'unit' => 'whole'],
      ['item' => 'butter', 'quantity' => 4, 'unit' => 'tbsp'],
      ['item' => 'vanilla', 'quantity' => 1, 'unit' => 'tsp'],
      ['item' => 'flour', 'quantity' => 1.5, 'unit' => 'cup'],
    ],
    'prepSteps' => ['prepare_filling', 'make_dough', 'assemble'],
    'difficulty' => 'easy',
  ],
  [
    '_id' => 'blueberry',
    'name' => 'Fresh Blueberry Pie',
    'description' => 'Bursting with fresh blueberries',
    'bakingTime' => 45,
    'bakingTemp' => 375,
    'ingredients' => [
      ['item' => 'blueberries', 'quantity' => 4, 'unit' => 'cup'],
      ['item' => 'sugar', 'quantity' => 0.75, 'unit' => 'cup'],
      ['item' => 'cornstarch', 'quantity' => 3, 'unit' => 'tbsp'],
      ['item' => 'lemon_juice', 'quantity' => 1, 'unit' => 'tbsp'],
      ['item' => 'flour', 'quantity' => 2.5, 'unit' => 'cup'],
    ],
    'prepSteps' => ['wash_fruit', 'make_dough', 'assemble'],
    'difficulty' => 'easy',
  ],
];

foreach ($recipes as $recipe) {
  $collection->replaceOne(['_id' => $recipe['_id']], $recipe, ['upsert' => TRUE]);
  echo "Seeded recipe: {$recipe['name']}\n";
}

echo "Recipe catalog seeded successfully.\n";
