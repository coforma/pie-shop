using MongoDB.Driver;
using PieShop.Core.Models;
using PieShop.Infrastructure.Data;

namespace PieShop.Infrastructure.Repositories;

/// <summary>
/// MongoDB implementation of recipe repository
/// </summary>
public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoCollection<Recipe> _recipes;

    public RecipeRepository(MongoDbContext context)
    {
        _recipes = context.Recipes;
    }

    public async Task<Recipe?> GetRecipeAsync(string pieType)
    {
        // MongoDB query - case insensitive search
        var filter = Builders<Recipe>.Filter.Eq(r => r.Id, pieType.ToLower());
        return await _recipes.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        return await _recipes.Find(_ => true).ToListAsync();
    }
}
