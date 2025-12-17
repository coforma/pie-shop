using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PieShop.Core.StateMachine;

namespace PieShop.Tests.Unit;

/// <summary>
/// Comprehensive tests for OrderStateMachine
/// This is an example of well-written tests with clear assertions
/// </summary>
public class StateMachineTests
{
    private readonly OrderStateMachine _stateMachine;

    public StateMachineTests()
    {
        _stateMachine = new OrderStateMachine(NullLogger<OrderStateMachine>.Instance);
    }

    [Theory]
    [InlineData(OrderState.ORDERED, OrderState.PICKING, true)]
    [InlineData(OrderState.PICKING, OrderState.PREPPING, true)]
    [InlineData(OrderState.PREPPING, OrderState.BAKING, true)]
    [InlineData(OrderState.BAKING, OrderState.DELIVERING, true)]
    [InlineData(OrderState.DELIVERING, OrderState.COMPLETED, true)]
    [InlineData(OrderState.ORDERED, OrderState.BAKING, false)] // Skipping states not allowed
    [InlineData(OrderState.COMPLETED, OrderState.ORDERED, false)] // Can't go back from terminal
    [InlineData(OrderState.ERROR, OrderState.ORDERED, false)] // Can't recover from error
    public void IsTransitionValid_ShouldValidateTransitionCorrectly(
        OrderState fromState, 
        OrderState toState, 
        bool expected)
    {
        // Act
        var result = _stateMachine.IsTransitionValid(fromState, toState);

        // Assert
        result.Should().Be(expected, 
            because: $"transition from {fromState} to {toState} should be {(expected ? "valid" : "invalid")}");
    }

    [Theory]
    [InlineData(OrderState.ORDERED, OrderState.PICKING)]
    [InlineData(OrderState.PICKING, OrderState.PREPPING)]
    [InlineData(OrderState.PREPPING, OrderState.BAKING)]
    [InlineData(OrderState.BAKING, OrderState.DELIVERING)]
    [InlineData(OrderState.DELIVERING, OrderState.COMPLETED)]
    public void GetNextState_ShouldReturnCorrectNextState(OrderState currentState, OrderState expectedNext)
    {
        // Act
        var nextState = _stateMachine.GetNextState(currentState);

        // Assert
        nextState.Should().Be(expectedNext, 
            because: $"the next state after {currentState} should be {expectedNext}");
    }

    [Theory]
    [InlineData(OrderState.COMPLETED)]
    [InlineData(OrderState.ERROR)]
    public void GetNextState_ShouldReturnNullForTerminalStates(OrderState terminalState)
    {
        // Act
        var nextState = _stateMachine.GetNextState(terminalState);

        // Assert
        nextState.Should().BeNull(
            because: $"{terminalState} is a terminal state with no next state");
    }

    [Theory]
    [InlineData(OrderState.COMPLETED, true)]
    [InlineData(OrderState.ERROR, true)]
    [InlineData(OrderState.ORDERED, false)]
    [InlineData(OrderState.PICKING, false)]
    [InlineData(OrderState.PREPPING, false)]
    [InlineData(OrderState.BAKING, false)]
    [InlineData(OrderState.DELIVERING, false)]
    public void IsTerminalState_ShouldIdentifyTerminalStatesCorrectly(
        OrderState state, 
        bool expectedIsTerminal)
    {
        // Act
        var result = _stateMachine.IsTerminalState(state);

        // Assert
        result.Should().Be(expectedIsTerminal,
            because: $"{state} should {(expectedIsTerminal ? "" : "not ")}be a terminal state");
    }

    [Fact]
    public void CreateTransition_WithValidTransition_ShouldReturnStateTransition()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var fromState = OrderState.ORDERED;
        var toState = OrderState.PICKING;
        var notes = "Starting fruit picking";

        // Act
        var transition = _stateMachine.CreateTransition(orderId, fromState, toState, notes);

        // Assert
        transition.Should().NotBeNull();
        transition.OrderId.Should().Be(orderId);
        transition.FromState.Should().Be(fromState);
        transition.ToState.Should().Be(toState);
        transition.Notes.Should().Be(notes);
        transition.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateTransition_WithInvalidTransition_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var fromState = OrderState.ORDERED;
        var invalidToState = OrderState.BAKING; // Skipping states

        // Act
        Action act = () => _stateMachine.CreateTransition(orderId, fromState, invalidToState);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Invalid state transition*")
            .And.Message.Should().Contain(orderId.ToString());
    }

    [Fact]
    public void CreateErrorTransition_ShouldCreateTransitionToErrorState()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var fromState = OrderState.PICKING;
        var errorMessage = "Fruit picker robot malfunction";

        // Act
        var transition = _stateMachine.CreateErrorTransition(orderId, fromState, errorMessage);

        // Assert
        transition.Should().NotBeNull();
        transition.OrderId.Should().Be(orderId);
        transition.FromState.Should().Be(fromState);
        transition.ToState.Should().Be(OrderState.ERROR);
        transition.ErrorMessage.Should().Be(errorMessage);
        transition.Notes.Should().Contain("Error:");
    }

    [Fact]
    public void GetValidTransitions_ShouldReturnAllPossibleTransitions()
    {
        // Arrange
        var state = OrderState.ORDERED;

        // Act
        var validTransitions = _stateMachine.GetValidTransitions(state);

        // Assert
        validTransitions.Should().HaveCount(2);
        validTransitions.Should().Contain(OrderState.PICKING);
        validTransitions.Should().Contain(OrderState.ERROR);
    }

    [Fact]
    public void GetValidTransitions_ForTerminalState_ShouldReturnEmptyList()
    {
        // Arrange
        var terminalState = OrderState.COMPLETED;

        // Act
        var validTransitions = _stateMachine.GetValidTransitions(terminalState);

        // Assert
        validTransitions.Should().BeEmpty(
            because: "terminal states should have no valid transitions");
    }

    [Fact]
    public void AllStates_ShouldBeAbleToTransitionToError()
    {
        // Arrange
        var nonTerminalStates = new[]
        {
            OrderState.ORDERED,
            OrderState.PICKING,
            OrderState.PREPPING,
            OrderState.BAKING,
            OrderState.DELIVERING
        };

        // Act & Assert
        foreach (var state in nonTerminalStates)
        {
            var validTransitions = _stateMachine.GetValidTransitions(state);
            validTransitions.Should().Contain(OrderState.ERROR,
                because: $"all non-terminal states should be able to transition to ERROR");
        }
    }
}
