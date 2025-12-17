using Microsoft.EntityFrameworkCore;
using PieShop.Core.Models;
using PieShop.Infrastructure.Data;

namespace PieShop.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of order repository
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly PieShopDbContext _context;

    public OrderRepository(PieShopDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.History)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        // TODO: Add pagination! This will load all orders into memory
        return await _context.Orders
            .Include(o => o.History)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
