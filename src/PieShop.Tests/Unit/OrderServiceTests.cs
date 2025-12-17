using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PieShop.Core.Services;
using PieShop.Core.Models;
using PieShop.Core.StateMachine;
using PieShop.Infrastructure.Repositories;
using PieShop.Infrastructure.ExternalServices;

namespace PieShop.Tests.Unit;

/// <summary>
/// OrderService tests - some coverage but missing edge cases
/// This is an example of incomplete tests that could be improved
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IFruitPickerClient> _fruitPickerMock;
    private readonly Mock<IBakerClient> _bakerMock;
    private readonly Mock<IDeliveryClient> _deliveryMock;
    private readonly OrderStateMachine _stateMachine;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _fruitPickerMock = new Mock<IFruitPickerClient>();
        _bakerMock = new Mock<IBakerClient>();
        _deliveryMock = new Mock<IDeliveryClient>();
        _stateMachine = new OrderStateMachine(NullLogger<OrderStateMachine>.Instance);
        
        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _recipeRepositoryMock.Object,
            _stateMachine,
            _fruitPickerMock.Object,
            _bakerMock.Object,
            _deliveryMock.Object,
            NullLogger<OrderService>.Instance);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidInput_ShouldCreateOrder()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = "apple",
            Name = "Apple Pie",
            BakingTime = 45,
            BakingTemp = 375
        };
        
        _recipeRepositoryMock
            .Setup(r => r.GetRecipeAsync("apple"))
            .ReturnsAsync(recipe);
        
        var customer = new Customer { Name = "John Doe", Email = "john@example.com" };
        var address = new DeliveryAddress
        {
            Street = "123 Main St",
            City = "Springfield",
            State = "IL",
            Zip = "62701"
        };

        // Act
        var order = await _orderService.CreateOrderAsync("apple", customer, address);

        // Assert
        order.Should().NotBeNull();
        order.PieType.Should().Be("apple");
        order.CurrentState.Should().Be(OrderState.ORDERED);
        order.Customer.Name.Should().Be("John Doe");
        
        _orderRepositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidPieType_ShouldThrowException()
    {
        // Arrange
        _recipeRepositoryMock
            .Setup(r => r.GetRecipeAsync("chocolate"))
            .ReturnsAsync((Recipe?)null);
        
        var customer = new Customer { Name = "John Doe", Email = "john@example.com" };
        var address = new DeliveryAddress
        {
            Street = "123 Main St",
            City = "Springfield",
            State = "IL",
            Zip = "62701"
        };

        // Act
        Func<Task> act = async () => await _orderService.CreateOrderAsync("chocolate", customer, address);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Invalid pie type*");
    }

    // TODO: Missing tests for:
    // - Null customer
    // - Null address
    // - Empty strings in required fields
    // - Special characters in input
    // - Very long strings

    [Fact]
    public async Task GetOrderAsync_WithValidId_ShouldReturnOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            PieType = "apple",
            CurrentState = OrderState.ORDERED,
            Customer = new Customer { Name = "John Doe", Email = "john@example.com" },
            DeliveryAddress = new DeliveryAddress
            {
                Street = "123 Main St",
                City = "Springfield",
                State = "IL",
                Zip = "62701"
            }
        };
        
        _orderRepositoryMock
            .Setup(r => r.GetOrderByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.PieType.Should().Be("apple");
    }

    [Fact]
    public async Task GetOrderAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        
        _orderRepositoryMock
            .Setup(r => r.GetOrderByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.GetOrderAsync(orderId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), PieType = "apple", CurrentState = OrderState.ORDERED },
            new Order { Id = Guid.NewGuid(), PieType = "cherry", CurrentState = OrderState.BAKING }
        };
        
        _orderRepositoryMock
            .Setup(r => r.GetAllOrdersAsync())
            .ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAllOrdersAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].PieType.Should().Be("apple");
        result[1].PieType.Should().Be("cherry");
    }

    // TODO: Test ProcessOrderAsync - this is complex and needs better test coverage
    // - Test each state transition
    // - Test service call failures
    // - Test retry logic
    // - Test concurrent processing
    // - Test timeout scenarios
}
