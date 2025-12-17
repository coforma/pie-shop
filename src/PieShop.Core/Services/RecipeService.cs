using PieShop.Core.Models;
using PieShop.Infrastructure.Repositories;

namespace PieShop.Core.Services;

/// <summary>
/// Service for managing pie recipes
/// </summary>
public class RecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Recipe?> GetRecipeAsync(string pieType)
    {
        return await _repository.GetRecipeAsync(pieType);
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        return await _repository.GetAllRecipesAsync();
    }
}
