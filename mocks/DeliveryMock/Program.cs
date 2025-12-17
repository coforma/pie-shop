var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// In-memory delivery storage
var deliveries = new Dictionary<string, DeliveryInfo>();
var random = new Random();

app.MapPost("/api/v1/deliveries", (DeliveryRequest request) =>
{
    var deliveryId = $"del_{Guid.NewGuid():N}";
    
    // Simulate 8% failure rate
    var willFail = random.Next(100) < 8;
    
    // Calculate delay based on "distance" (hash of destination for consistency)
    var destinationHash = request.Destination.GetHashCode();
    var distance = Math.Abs(destinationHash % 20) + 5; // 5-25 km
    var delaySeconds = distance + random.Next(5, 16); // 10-40 seconds (simulating minutes)
    
    var estimatedCompletion = DateTime.UtcNow.AddSeconds(delaySeconds);
    var droneId = $"drone-{random.Next(1, 21)}";
    
    var delivery = new DeliveryInfo
    {
        DeliveryId = deliveryId,
        Status = "ASSIGNED",
        DroneId = droneId,
        Destination = request.Destination,
        EstimatedArrival = estimatedCompletion,
        WillFail = willFail,
        CompletionTime = estimatedCompletion,
        Distance = distance
    };
    
    deliveries[deliveryId] = delivery;
    
    // Simulate delivery progress
    _ = Task.Run(async () =>
    {
        await Task.Delay(2000);
        delivery.Status = "IN_TRANSIT";
        delivery.Location = "En route to destination";
        
        await Task.Delay((delaySeconds - 4) * 1000);
        delivery.Status = "APPROACHING";
        delivery.Location = "Approaching destination";
        
        await Task.Delay(2000);
        
        if (delivery.WillFail)
        {
            var failures = new[]
            {
                "Weather conditions unsuitable for drone flight",
                "Destination address not accessible by drone",
                "Drone battery low - returned to base",
                "Air traffic congestion - delivery aborted"
            };
            delivery.Status = "FAILED";
            delivery.ErrorMessage = failures[random.Next(failures.Length)];
        }
        else
        {
            delivery.Status = "DELIVERED";
            delivery.Location = request.Destination;
        }
    });
    
    return Results.Ok(new DeliveryResponse
    {
        DeliveryId = deliveryId,
        DroneId = droneId,
        Eta = estimatedCompletion
    });
});

app.MapGet("/api/v1/deliveries/{deliveryId}", (string deliveryId) =>
{
    if (!deliveries.TryGetValue(deliveryId, out var delivery))
    {
        return Results.NotFound(new { error = "Delivery not found" });
    }
    
    return Results.Ok(new DeliveryStatusResponse
    {
        DeliveryId = delivery.DeliveryId,
        Status = delivery.Status,
        DroneId = delivery.DroneId,
        Location = delivery.Location,
        EstimatedArrival = delivery.EstimatedArrival,
        ErrorMessage = delivery.ErrorMessage
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "delivery" }));

Console.WriteLine("Delivery Mock Service starting on port 8083");
Console.WriteLine("Simulating 10-40 second deliveries with 8% failure rate");
app.Run("http://0.0.0.0:8083");

// Models
record DeliveryRequest(object Package, string Destination, string Window);
record DeliveryResponse(string DeliveryId, string DroneId, DateTime Eta);
record DeliveryStatusResponse(string DeliveryId, string Status, string DroneId, string? Location, DateTime? EstimatedArrival, string? ErrorMessage);

class DeliveryInfo
{
    public string DeliveryId { get; set; } = "";
    public string Status { get; set; } = "";
    public string DroneId { get; set; } = "";
    public string Destination { get; set; } = "";
    public string? Location { get; set; }
    public DateTime? EstimatedArrival { get; set; }
    public DateTime CompletionTime { get; set; }
    public bool WillFail { get; set; }
    public int Distance { get; set; }
    public string? ErrorMessage { get; set; }
}
