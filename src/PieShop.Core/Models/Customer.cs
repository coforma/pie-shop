namespace PieShop.Core.Models;

/// <summary>
/// Represents a customer's information for order processing
/// </summary>
public class Customer
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public Customer() { }

    public Customer(string name, string email, string? phone = null)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }
}
