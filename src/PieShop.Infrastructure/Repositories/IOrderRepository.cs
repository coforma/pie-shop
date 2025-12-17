using PieShop.Core.Models;

namespace PieShop.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Order entity data access
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(Guid orderId);
    Task<List<Order>> GetAllOrdersAsync();
    Task CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
}
