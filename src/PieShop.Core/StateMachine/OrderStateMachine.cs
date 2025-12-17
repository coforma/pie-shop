using Microsoft.Extensions.Logging;

namespace PieShop.Core.StateMachine;

/// <summary>
/// Well-implemented state machine for order lifecycle management.
/// This class demonstrates proper state management with clear transitions,
/// validation, and comprehensive logging.
/// </summary>
public class OrderStateMachine
{
    private readonly ILogger<OrderStateMachine> _logger;

    // Define valid transitions as a dictionary for O(1) lookup
    private static readonly Dictionary<OrderState, List<OrderState>> ValidTransitions = new()
    {
        { OrderState.ORDERED, new List<OrderState> { OrderState.PICKING, OrderState.ERROR } },
        { OrderState.PICKING, new List<OrderState> { OrderState.PREPPING, OrderState.ERROR } },
        { OrderState.PREPPING, new List<OrderState> { OrderState.BAKING, OrderState.ERROR } },
        { OrderState.BAKING, new List<OrderState> { OrderState.DELIVERING, OrderState.ERROR } },
        { OrderState.DELIVERING, new List<OrderState> { OrderState.COMPLETED, OrderState.ERROR } },
        { OrderState.ERROR, new List<OrderState>() }, // Terminal state - requires manual intervention
        { OrderState.COMPLETED, new List<OrderState>() } // Terminal state
    };

    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates whether a state transition is allowed
    /// </summary>
    /// <param name="fromState">The current state</param>
    /// <param name="toState">The desired state</param>
    /// <returns>True if the transition is valid, false otherwise</returns>
    public bool IsTransitionValid(OrderState fromState, OrderState toState)
    {
        if (!ValidTransitions.ContainsKey(fromState))
        {
            _logger.LogWarning("Invalid from state: {FromState}", fromState);
            return false;
        }

        var allowedTransitions = ValidTransitions[fromState];
        var isValid = allowedTransitions.Contains(toState);

        if (!isValid)
        {
            _logger.LogWarning(
                "Invalid state transition attempted: {FromState} -> {ToState}",
                fromState,
                toState);
        }

        return isValid;
    }

    /// <summary>
    /// Gets the next expected state in the normal flow
    /// </summary>
    /// <param name="currentState">The current state</param>
    /// <returns>The next state in the happy path, or null if terminal/error</returns>
    public OrderState? GetNextState(OrderState currentState)
    {
        return currentState switch
        {
            OrderState.ORDERED => OrderState.PICKING,
            OrderState.PICKING => OrderState.PREPPING,
            OrderState.PREPPING => OrderState.BAKING,
            OrderState.BAKING => OrderState.DELIVERING,
            OrderState.DELIVERING => OrderState.COMPLETED,
            OrderState.COMPLETED => null, // Terminal state
            OrderState.ERROR => null, // Terminal state
            _ => throw new ArgumentException($"Unknown state: {currentState}", nameof(currentState))
        };
    }

    /// <summary>
    /// Gets all valid transitions from the current state
    /// </summary>
    /// <param name="currentState">The current state</param>
    /// <returns>List of valid target states</returns>
    public List<OrderState> GetValidTransitions(OrderState currentState)
    {
        if (!ValidTransitions.ContainsKey(currentState))
        {
            _logger.LogError("Attempted to get transitions for invalid state: {State}", currentState);
            return new List<OrderState>();
        }

        return new List<OrderState>(ValidTransitions[currentState]);
    }

    /// <summary>
    /// Checks if a state is a terminal state (no further transitions allowed)
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if terminal, false otherwise</returns>
    public bool IsTerminalState(OrderState state)
    {
        return state == OrderState.COMPLETED || state == OrderState.ERROR;
    }

    /// <summary>
    /// Creates a state transition record with proper validation and logging
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="fromState">The current state</param>
    /// <param name="toState">The desired target state</param>
    /// <param name="notes">Optional notes about the transition</param>
    /// <returns>A StateTransition object if valid, throws exception if invalid</returns>
    /// <exception cref="InvalidOperationException">Thrown when transition is not allowed</exception>
    public StateTransition CreateTransition(
        Guid orderId, 
        OrderState fromState, 
        OrderState toState, 
        string? notes = null)
    {
        if (!IsTransitionValid(fromState, toState))
        {
            var errorMessage = $"Invalid state transition: {fromState} -> {toState} for order {orderId}";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation(
            "State transition: Order {OrderId} moving from {FromState} to {ToState}",
            orderId,
            fromState,
            toState);

        return new StateTransition(orderId, fromState, toState, notes);
    }

    /// <summary>
    /// Creates an error transition with error details
    /// </summary>
    /// <param name="orderId">The order ID</param>
    /// <param name="fromState">The current state</param>
    /// <param name="errorMessage">The error details</param>
    /// <returns>A StateTransition object marking the error</returns>
    public StateTransition CreateErrorTransition(
        Guid orderId, 
        OrderState fromState, 
        string errorMessage)
    {
        _logger.LogError(
            "Order {OrderId} transitioning to ERROR state from {FromState}. Error: {Error}",
            orderId,
            fromState,
            errorMessage);

        return new StateTransition(orderId, fromState, OrderState.ERROR, $"Error: {errorMessage}")
        {
            ErrorMessage = errorMessage
        };
    }
}
