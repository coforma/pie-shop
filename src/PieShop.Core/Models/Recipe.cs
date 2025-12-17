namespace PieShop.Core.Models;

/// <summary>
/// Represents a pie recipe with baking instructions and ingredient list
/// </summary>
public class Recipe
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BakingTime { get; set; }  // in minutes
    public int BakingTemp { get; set; }  // in Fahrenheit
    public List<Ingredient> Ingredients { get; set; } = new();
    public List<string> PrepSteps { get; set; } = new();
    public string Difficulty { get; set; } = "medium";
}

/// <summary>
/// Represents an ingredient with quantity and unit
/// </summary>
public class Ingredient
{
    public string Item { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
