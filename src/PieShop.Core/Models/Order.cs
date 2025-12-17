using PieShop.Core.StateMachine;

namespace PieShop.Core.Models;

/// <summary>
/// Represents a pie order with all its associated information
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public string PieType { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
    public DeliveryAddress DeliveryAddress { get; set; } = new();
    public OrderState CurrentState { get; set; }
    public string? PickerJobId { get; set; }
    public string? BakerJobId { get; set; }
    public string? DeliveryId { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<StateTransition> History { get; set; } = new();
}
