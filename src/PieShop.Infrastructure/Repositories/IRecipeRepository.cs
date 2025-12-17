using PieShop.Core.Models;

namespace PieShop.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Recipe entity data access
/// </summary>
public interface IRecipeRepository
{
    Task<Recipe?> GetRecipeAsync(string pieType);
    Task<List<Recipe>> GetAllRecipesAsync();
}
