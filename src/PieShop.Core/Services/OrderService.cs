using PieShop.Core.Models;
using PieShop.Core.StateMachine;
using PieShop.Infrastructure.Repositories;
using PieShop.Infrastructure.ExternalServices;
using Microsoft.Extensions.Logging;

namespace PieShop.Core.Services;

/// <summary>
/// Service for managing order lifecycle and orchestration.
/// Coordinates with external services (fruit picker, baker, delivery) to process orders.
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly OrderStateMachine _stateMachine;
    private readonly IFruitPickerClient _fruitPickerClient;
    private readonly IBakerClient _bakerClient;
    private readonly IDeliveryClient _deliveryClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IRecipeRepository recipeRepository,
        OrderStateMachine stateMachine,
        IFruitPickerClient fruitPickerClient,
        IBakerClient bakerClient,
        IDeliveryClient deliveryClient,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _recipeRepository = recipeRepository;
        _stateMachine = stateMachine;
        _fruitPickerClient = fruitPickerClient;
        _bakerClient = bakerClient;
        _deliveryClient = deliveryClient;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order
    /// TODO: Add input validation
    /// </summary>
    public async Task<Order> CreateOrderAsync(string pieType, Customer customer, DeliveryAddress address)
    {
        // No validation here - should check for null/empty values!
        var recipe = await _recipeRepository.GetRecipeAsync(pieType);
        if (recipe == null)
        {
            throw new ArgumentException($"Invalid pie type: {pieType}");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            PieType = pieType,
            Customer = customer,
            DeliveryAddress = address,
            CurrentState = OrderState.ORDERED,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EstimatedDelivery = DateTime.UtcNow.AddHours(2) // Magic number! Should be calculated
        };

        var transition = _stateMachine.CreateTransition(order.Id, OrderState.ORDERED, OrderState.ORDERED, "Order created");
        order.History.Add(transition);

        await _orderRepository.CreateOrderAsync(order);
        
        _logger.LogInformation("Order created: {OrderId}", order.Id);

        // Start processing asynchronously - fire and forget (is this a good idea?)
        _ = Task.Run(() => ProcessOrderAsync(order.Id));

        return order;
    }

    /// <summary>
    /// Main orchestration method - processes order through all states
    /// WARNING: This method is way too long (100+ lines) and does too many things!
    /// Should be refactored into smaller, focused methods.
    /// </summary>
    public async Task ProcessOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogError("Order not found: {OrderId}", orderId);
            return;
        }

        _logger.LogInformation("Starting to process order {OrderId}", orderId);

        try
        {
            // Step 1: Fruit Picking
            if (order.CurrentState == OrderState.ORDERED)
            {
                _logger.LogInformation("Starting fruit picking for order {OrderId}", orderId);
                
                var transition = _stateMachine.CreateTransition(order.Id, order.CurrentState, OrderState.PICKING);
                order.CurrentState = OrderState.PICKING;
                order.History.Add(transition);
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order);

                var recipe = await _recipeRepository.GetRecipeAsync(order.PieType);
                
                // Call fruit picker service
                try
                {
                    var pickRequest = new
                    {
                        fruitType = order.PieType,
                        quantity = recipe?.Ingredients.FirstOrDefault()?.Quantity ?? 6, // Fallback to 6
                        quality = "premium"
                    };
                    
                    var pickResponse = await _fruitPickerClient.PickFruitAsync(pickRequest);
                    order.PickerJobId = pickResponse.JobId;
                    await _orderRepository.UpdateOrderAsync(order);
                    
                    // Wait for picking to complete - polling approach (not ideal!)
                    var maxAttempts = 30; // Magic number
                    for (int i = 0; i < maxAttempts; i++)
                    {
                        await Task.Delay(2000); // Hard-coded delay
                        var status = await _fruitPickerClient.GetJobStatusAsync(pickResponse.JobId);
                        if (status.Status == "COMPLETED")
                        {
                            break;
                        }
                        if (status.Status == "FAILED")
                        {
                            throw new Exception("Fruit picking failed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fruit picking failed for order {OrderId}", orderId);
                    var errorTransition = _stateMachine.CreateErrorTransition(order.Id, order.CurrentState, ex.Message);
                    order.CurrentState = OrderState.ERROR;
                    order.History.Add(errorTransition);
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateOrderAsync(order);
                    return;
                }
            }

            // Step 2: Prepping
            if (order.CurrentState == OrderState.PICKING)
            {
                _logger.LogInformation("Starting prep for order {OrderId}", orderId);
                
                var transition = _stateMachine.CreateTransition(order.Id, order.CurrentState, OrderState.PREPPING);
                order.CurrentState = OrderState.PREPPING;
                order.History.Add(transition);
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order);

                // Simulate prep time (washing, peeling, etc.)
                await Task.Delay(5000); // Hard-coded 5 seconds
            }

            // Step 3: Baking
            if (order.CurrentState == OrderState.PREPPING)
            {
                _logger.LogInformation("Starting baking for order {OrderId}", orderId);
                
                var transition = _stateMachine.CreateTransition(order.Id, order.CurrentState, OrderState.BAKING);
                order.CurrentState = OrderState.BAKING;
                order.History.Add(transition);
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order);

                var recipe = await _recipeRepository.GetRecipeAsync(order.PieType);
                
                try
                {
                    var bakeRequest = new
                    {
                        pieType = order.PieType,
                        temperature = recipe?.BakingTemp ?? 375,
                        duration = recipe?.BakingTime ?? 45
                    };
                    
                    var bakeResponse = await _bakerClient.BakeAsync(bakeRequest);
                    order.BakerJobId = bakeResponse.JobId;
                    await _orderRepository.UpdateOrderAsync(order);
                    
                    // Wait for baking - same polling approach (repetitive code!)
                    for (int i = 0; i < 60; i++) // Another magic number
                    {
                        await Task.Delay(2000);
                        var status = await _bakerClient.GetJobStatusAsync(bakeResponse.JobId);
                        if (status.Status == "COMPLETED")
                        {
                            break;
                        }
                        if (status.Status == "FAILED")
                        {
                            throw new Exception("Baking failed");
                        }
                    }
                }
                catch (Exception ex) // Same error handling pattern repeated
                {
                    _logger.LogError(ex, "Baking failed for order {OrderId}", orderId);
                    var errorTransition = _stateMachine.CreateErrorTransition(order.Id, order.CurrentState, ex.Message);
                    order.CurrentState = OrderState.ERROR;
                    order.History.Add(errorTransition);
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateOrderAsync(order);
                    return;
                }
            }

            // Step 4: Delivery
            if (order.CurrentState == OrderState.BAKING)
            {
                _logger.LogInformation("Starting delivery for order {OrderId}", orderId);
                
                var transition = _stateMachine.CreateTransition(order.Id, order.CurrentState, OrderState.DELIVERING);
                order.CurrentState = OrderState.DELIVERING;
                order.History.Add(transition);
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order);

                try
                {
                    var deliveryRequest = new
                    {
                        package = new { type = "pie", temperature = "warm" },
                        destination = order.DeliveryAddress.ToFormattedString(),
                        window = DateTime.UtcNow.AddHours(1).ToString("o")
                    };
                    
                    var deliveryResponse = await _deliveryClient.DeliverAsync(deliveryRequest);
                    order.DeliveryId = deliveryResponse.DeliveryId;
                    await _orderRepository.UpdateOrderAsync(order);
                    
                    // Yet another polling loop - this is getting repetitive!
                    for (int i = 0; i < 90; i++) // Yet another magic number
                    {
                        await Task.Delay(2000);
                        var status = await _deliveryClient.GetDeliveryStatusAsync(deliveryResponse.DeliveryId);
                        if (status.Status == "DELIVERED")
                        {
                            break;
                        }
                        if (status.Status == "FAILED")
                        {
                            throw new Exception("Delivery failed");
                        }
                    }
                }
                catch (Exception ex) // Same error handling again!
                {
                    _logger.LogError(ex, "Delivery failed for order {OrderId}", orderId);
                    var errorTransition = _stateMachine.CreateErrorTransition(order.Id, order.CurrentState, ex.Message);
                    order.CurrentState = OrderState.ERROR;
                    order.History.Add(errorTransition);
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateOrderAsync(order);
                    return;
                }
            }

            // Final step: Mark as completed
            if (order.CurrentState == OrderState.DELIVERING)
            {
                var transition = _stateMachine.CreateTransition(order.Id, order.CurrentState, OrderState.COMPLETED, "Order delivered successfully");
                order.CurrentState = OrderState.COMPLETED;
                order.History.Add(transition);
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order);
                
                _logger.LogInformation("Order {OrderId} completed successfully", orderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing order {OrderId}", orderId);
            // General error handling - might hide specific issues
            if (order.CurrentState != OrderState.ERROR)
            {
                var errorTransition = _stateMachine.CreateErrorTransition(order.Id, order.CurrentState, ex.Message);
                order.CurrentState = OrderState.ERROR;
                order.History.Add(errorTransition);
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order);
            }
        }
    }

    public async Task<Order?> GetOrderAsync(Guid orderId)
    {
        return await _orderRepository.GetOrderByIdAsync(orderId);
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        // TODO: Add pagination - this will fail with many orders!
        return await _orderRepository.GetAllOrdersAsync();
    }
}
