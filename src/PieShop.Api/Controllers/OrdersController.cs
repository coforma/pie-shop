using Microsoft.AspNetCore.Mvc;
using PieShop.Core.Services;
using PieShop.Core.Models;

namespace PieShop.Api.Controllers;

/// <summary>
/// Orders API endpoints
/// TODO: Add authentication and authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly RecipeService _recipeService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrderService orderService,
        RecipeService recipeService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _recipeService = recipeService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new pie order
    /// </summary>
    /// <param name="request">Order details</param>
    /// <returns>Created order with order ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // TODO: Add input validation! Missing validation for null/empty values
        try
        {
            var customer = new Customer
            {
                Name = request.Customer.Name,
                Email = request.Customer.Email,
                Phone = request.Customer.Phone
            };

            var address = new DeliveryAddress
            {
                Street = request.DeliveryAddress.Street,
                City = request.DeliveryAddress.City,
                State = request.DeliveryAddress.State,
                Zip = request.DeliveryAddress.Zip
            };

            var order = await _orderService.CreateOrderAsync(request.PieType, customer, address);

            var response = new OrderResponse
            {
                OrderId = order.Id.ToString(),
                Status = order.CurrentState.ToString(),
                EstimatedDelivery = order.EstimatedDelivery,
                CreatedAt = order.CreatedAt
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "INVALID_PIE_TYPE",
                Message = ex.Message,
                AvailableTypes = new[] { "apple", "cherry", "pumpkin", "pecan", "blueberry" }
            });
        }
    }

    /// <summary>
    /// Gets order details by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details including status and history</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        
        if (order == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "ORDER_NOT_FOUND",
                Message = $"Order '{id}' does not exist"
            });
        }

        var response = new OrderDetailResponse
        {
            OrderId = order.Id.ToString(),
            PieType = order.PieType,
            Customer = new CustomerDto
            {
                Name = order.Customer.Name,
                Email = order.Customer.Email,
                Phone = order.Customer.Phone
            },
            DeliveryAddress = new DeliveryAddressDto
            {
                Street = order.DeliveryAddress.Street,
                City = order.DeliveryAddress.City,
                State = order.DeliveryAddress.State,
                Zip = order.DeliveryAddress.Zip
            },
            Status = order.CurrentState.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            EstimatedDelivery = order.EstimatedDelivery,
            History = order.History.Select(h => new StateHistoryDto
            {
                State = h.ToState.ToString(),
                Timestamp = h.Timestamp
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets all orders
    /// TODO: Add pagination! This will fail with many orders
    /// TODO: Add authentication - this should be admin only
    /// </summary>
    /// <returns>List of all orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders()
    {
        // No authentication check - anyone can see all orders!
        var orders = await _orderService.GetAllOrdersAsync();
        
        var response = orders.Select(o => new OrderSummaryDto
        {
            OrderId = o.Id.ToString(),
            CustomerName = o.Customer.Name,
            PieType = o.PieType,
            Status = o.CurrentState.ToString(),
            CreatedAt = o.CreatedAt
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Gets available pie types from catalog
    /// </summary>
    /// <returns>List of available pie recipes</returns>
    [HttpGet("catalog")]
    [ProducesResponseType(typeof(List<RecipeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalog()
    {
        var recipes = await _recipeService.GetAllRecipesAsync();
        
        var response = recipes.Select(r => new RecipeDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            BakingTime = r.BakingTime,
            Difficulty = r.Difficulty
        }).ToList();

        return Ok(response);
    }
}

// DTOs (Data Transfer Objects)
public class CreateOrderRequest
{
    public string PieType { get; set; } = string.Empty;
    public CustomerDto Customer { get; set; } = new();
    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
}

public class CustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class DeliveryAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}

public class OrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderDetailResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string PieType { get; set; } = string.Empty;
    public CustomerDto Customer { get; set; } = new();
    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public List<StateHistoryDto> History { get; set; } = new();
}

public class StateHistoryDto
{
    public string State { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class OrderSummaryDto
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PieType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecipeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BakingTime { get; set; }
    public string Difficulty { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string[]? AvailableTypes { get; set; }
}
