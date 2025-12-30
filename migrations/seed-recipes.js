const mongoose = require('mongoose');
const Recipe = require('../src/models/recipe');
const config = require('../src/core/config');

const recipes = [
  {
    _id: 'apple',
    name: 'Classic Apple Pie',
    description: 'Traditional American apple pie with cinnamon',
    bakingTime: 45,
    bakingTemp: 375,
    ingredients: [
      { item: 'apples', quantity: 6, unit: 'whole' },
      { item: 'sugar', quantity: 0.75, unit: 'cup' },
      { item: 'cinnamon', quantity: 1, unit: 'tsp' },
      { item: 'butter', quantity: 2, unit: 'tbsp' },
      { item: 'flour', quantity: 2.5, unit: 'cup' },
      { item: 'salt', quantity: 1, unit: 'tsp' }
    ],
    prepSteps: ['wash_fruit', 'peel_fruit', 'slice_fruit', 'make_dough', 'assemble'],
    difficulty: 'medium'
  },
  {
    _id: 'cherry',
    name: 'Cherry Pie',
    description: 'Sweet and tart cherry pie',
    bakingTime: 50,
    bakingTemp: 350,
    ingredients: [
      { item: 'cherries', quantity: 4, unit: 'cup' },
      { item: 'sugar', quantity: 1, unit: 'cup' },
      { item: 'cornstarch', quantity: 3, unit: 'tbsp' },
      { item: 'flour', quantity: 2.5, unit: 'cup' },
      { item: 'butter', quantity: 3, unit: 'tbsp' }
    ],
    prepSteps: ['pit_fruit', 'make_dough', 'assemble'],
    difficulty: 'medium'
  },
  {
    _id: 'pumpkin',
    name: 'Pumpkin Pie',
    description: 'Classic pumpkin pie with spices',
    bakingTime: 60,
    bakingTemp: 325,
    ingredients: [
      { item: 'pumpkin_puree', quantity: 2, unit: 'cup' },
      { item: 'eggs', quantity: 2, unit: 'whole' },
      { item: 'sugar', quantity: 0.75, unit: 'cup' },
      { item: 'cinnamon', quantity: 1, unit: 'tsp' },
      { item: 'nutmeg', quantity: 0.5, unit: 'tsp' },
      { item: 'flour', quantity: 1.5, unit: 'cup' }
    ],
    prepSteps: ['mix_filling', 'make_crust', 'assemble'],
    difficulty: 'easy'
  },
  {
    _id: 'pecan',
    name: 'Pecan Pie',
    description: 'Rich and nutty pecan pie',
    bakingTime: 55,
    bakingTemp: 350,
    ingredients: [
      { item: 'pecans', quantity: 1.5, unit: 'cup' },
      { item: 'corn_syrup', quantity: 1, unit: 'cup' },
      { item: 'eggs', quantity: 3, unit: 'whole' },
      { item: 'sugar', quantity: 1, unit: 'cup' },
      { item: 'butter', quantity: 3, unit: 'tbsp' },
      { item: 'flour', quantity: 1.5, unit: 'cup' }
    ],
    prepSteps: ['toast_pecans', 'make_filling', 'make_crust', 'assemble'],
    difficulty: 'medium'
  },
  {
    _id: 'blueberry',
    name: 'Blueberry Pie',
    description: 'Fresh blueberry pie',
    bakingTime: 45,
    bakingTemp: 375,
    ingredients: [
      { item: 'blueberries', quantity: 5, unit: 'cup' },
      { item: 'sugar', quantity: 0.75, unit: 'cup' },
      { item: 'cornstarch', quantity: 3, unit: 'tbsp' },
      { item: 'lemon_juice', quantity: 1, unit: 'tbsp' },
      { item: 'flour', quantity: 2.5, unit: 'cup' }
    ],
    prepSteps: ['wash_fruit', 'make_dough', 'assemble'],
    difficulty: 'easy'
  }
];

async function seedRecipes() {
  try {
    await mongoose.connect(config.mongodb.url);
    console.log('Connected to MongoDB');

    await Recipe.deleteMany({});
    console.log('Cleared existing recipes');

    await Recipe.insertMany(recipes);
    console.log('Seeded recipes successfully');

    await mongoose.connection.close();
    console.log('Database connection closed');
  } catch (error) {
    console.error('Error seeding recipes:', error);
    process.exit(1);
  }
}

seedRecipes();
