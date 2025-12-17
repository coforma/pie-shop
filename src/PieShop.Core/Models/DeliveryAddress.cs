namespace PieShop.Core.Models;

/// <summary>
/// Represents a delivery address
/// </summary>
public class DeliveryAddress
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;

    public DeliveryAddress() { }

    public DeliveryAddress(string street, string city, string state, string zip)
    {
        Street = street;
        City = city;
        State = state;
        Zip = zip;
    }

    public string ToFormattedString()
    {
        return $"{Street}, {City}, {State} {Zip}";
    }
}
