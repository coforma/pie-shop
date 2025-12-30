const mongoose = require('mongoose');
const config = require('../core/config');

mongoose.connect(config.mongodb.url);

const recipeSchema = new mongoose.Schema({
  _id: String,
  name: String,
  description: String,
  bakingTime: Number,
  bakingTemp: Number,
  ingredients: [{
    item: String,
    quantity: Number,
    unit: String
  }],
  prepSteps: [String],
  difficulty: String
});

const Recipe = mongoose.model('Recipe', recipeSchema);

module.exports = Recipe;
