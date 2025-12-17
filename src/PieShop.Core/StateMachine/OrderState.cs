namespace PieShop.Core.Models;

/// <summary>
/// Represents the possible states an order can be in during its lifecycle
/// </summary>
public enum OrderState
{
    ORDERED,
    PICKING,
    PREPPING,
    BAKING,
    DELIVERING,
    COMPLETED,
    ERROR
}
