using MongoDB.Driver;
using PieShop.Core.Models;

namespace PieShop.Infrastructure.Data;

/// <summary>
/// MongoDB context for recipe catalog
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        // TODO: Add connection error handling
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        
        // Seed data if collection is empty (for demo purposes)
        SeedDataIfEmpty().Wait();
    }

    public IMongoCollection<Recipe> Recipes =>
        _database.GetCollection<Recipe>("recipes");

    private async Task SeedDataIfEmpty()
    {
        var count = await Recipes.CountDocumentsAsync(_ => true);
        if (count == 0)
        {
            // Seed default recipes
            var recipes = new[]
            {
                new Recipe
                {
                    Id = "apple",
                    Name = "Classic Apple Pie",
                    Description = "Traditional American apple pie with cinnamon",
                    BakingTime = 45,
                    BakingTemp = 375,
                    Ingredients = new List<RecipeIngredient>
                    {
                        new() { Item = "apples", Quantity = 6, Unit = "whole" },
                        new() { Item = "sugar", Quantity = 0.75, Unit = "cup" },
                        new() { Item = "cinnamon", Quantity = 1, Unit = "tsp" },
                        new() { Item = "butter", Quantity = 2, Unit = "tbsp" },
                        new() { Item = "flour", Quantity = 2.5, Unit = "cup" },
                        new() { Item = "salt", Quantity = 1, Unit = "tsp" }
                    },
                    PrepSteps = new List<string> { "wash_fruit", "peel_fruit", "slice_fruit", "make_dough", "assemble" },
                    Difficulty = "medium"
                },
                new Recipe
                {
                    Id = "cherry",
                    Name = "Sweet Cherry Pie",
                    Description = "Fresh cherry pie with lattice crust",
                    BakingTime = 50,
                    BakingTemp = 375,
                    Ingredients = new List<RecipeIngredient>
                    {
                        new() { Item = "cherries", Quantity = 4, Unit = "cup" },
                        new() { Item = "sugar", Quantity = 1, Unit = "cup" },
                        new() { Item = "cornstarch", Quantity = 3, Unit = "tbsp" },
                        new() { Item = "flour", Quantity = 2.5, Unit = "cup" },
                        new() { Item = "salt", Quantity = 1, Unit = "tsp" }
                    },
                    PrepSteps = new List<string> { "pit_cherries", "make_dough", "prepare_filling", "assemble" },
                    Difficulty = "medium"
                },
                new Recipe
                {
                    Id = "pumpkin",
                    Name = "Spiced Pumpkin Pie",
                    Description = "Smooth pumpkin pie with fall spices",
                    BakingTime = 60,
                    BakingTemp = 350,
                    Ingredients = new List<RecipeIngredient>
                    {
                        new() { Item = "pumpkin_puree", Quantity = 2, Unit = "cup" },
                        new() { Item = "eggs", Quantity = 3, Unit = "whole" },
                        new() { Item = "brown_sugar", Quantity = 0.75, Unit = "cup" },
                        new() { Item = "cinnamon", Quantity = 1, Unit = "tsp" },
                        new() { Item = "ginger", Quantity = 0.5, Unit = "tsp" },
                        new() { Item = "evaporated_milk", Quantity = 1, Unit = "can" }
                    },
                    PrepSteps = new List<string> { "mix_filling", "make_crust", "pour", "bake" },
                    Difficulty = "easy"
                },
                new Recipe
                {
                    Id = "pecan",
                    Name = "Southern Pecan Pie",
                    Description = "Rich, sweet pecan pie with corn syrup",
                    BakingTime = 55,
                    BakingTemp = 350,
                    Ingredients = new List<RecipeIngredient>
                    {
                        new() { Item = "pecans", Quantity = 1.5, Unit = "cup" },
                        new() { Item = "corn_syrup", Quantity = 1, Unit = "cup" },
                        new() { Item = "eggs", Quantity = 3, Unit = "whole" },
                        new() { Item = "sugar", Quantity = 0.5, Unit = "cup" },
                        new() { Item = "butter", Quantity = 3, Unit = "tbsp" },
                        new() { Item = "vanilla", Quantity = 1, Unit = "tsp" }
                    },
                    PrepSteps = new List<string> { "toast_pecans", "make_filling", "assemble", "bake" },
                    Difficulty = "easy"
                },
                new Recipe
                {
                    Id = "blueberry",
                    Name = "Wild Blueberry Pie",
                    Description = "Bursting with fresh blueberries",
                    BakingTime = 50,
                    BakingTemp = 375,
                    Ingredients = new List<RecipeIngredient>
                    {
                        new() { Item = "blueberries", Quantity = 4, Unit = "cup" },
                        new() { Item = "sugar", Quantity = 0.75, Unit = "cup" },
                        new() { Item = "cornstarch", Quantity = 3, Unit = "tbsp" },
                        new() { Item = "lemon_juice", Quantity = 1, Unit = "tbsp" },
                        new() { Item = "flour", Quantity = 2.5, Unit = "cup" }
                    },
                    PrepSteps = new List<string> { "wash_berries", "make_filling", "make_dough", "assemble" },
                    Difficulty = "medium"
                }
            };

            await Recipes.InsertManyAsync(recipes);
        }
    }
}
