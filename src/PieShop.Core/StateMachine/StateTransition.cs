namespace PieShop.Core.StateMachine;

/// <summary>
/// Represents a state transition in an order's lifecycle
/// </summary>
public class StateTransition
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public OrderState? FromState { get; set; }
    public OrderState ToState { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Notes { get; set; }
    public string? ErrorMessage { get; set; }

    public StateTransition() 
    {
        Timestamp = DateTime.UtcNow;
    }

    public StateTransition(Guid orderId, OrderState? fromState, OrderState toState, string? notes = null)
    {
        OrderId = orderId;
        FromState = fromState;
        ToState = toState;
        Notes = notes;
        Timestamp = DateTime.UtcNow;
    }
}
