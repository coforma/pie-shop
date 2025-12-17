namespace PieShop.Infrastructure.ExternalServices;

/// <summary>
/// Interface for fruit picker service client
/// </summary>
public interface IFruitPickerClient
{
    Task<PickFruitResponse> PickFruitAsync(object request);
    Task<JobStatusResponse> GetJobStatusAsync(string jobId);
}

/// <summary>
/// Interface for baker service client
/// </summary>
public interface IBakerClient
{
    Task<BakeResponse> BakeAsync(object request);
    Task<JobStatusResponse> GetJobStatusAsync(string jobId);
}

/// <summary>
/// Interface for delivery service client
/// </summary>
public interface IDeliveryClient
{
    Task<DeliveryResponse> DeliverAsync(object request);
    Task<DeliveryStatusResponse> GetDeliveryStatusAsync(string deliveryId);
}

// Response models
public class PickFruitResponse
{
    public string JobId { get; set; } = string.Empty;
    public DateTime EstimatedCompletion { get; set; }
}

public class BakeResponse
{
    public string JobId { get; set; } = string.Empty;
    public string OvenId { get; set; } = string.Empty;
    public DateTime EstimatedCompletion { get; set; }
}

public class DeliveryResponse
{
    public string DeliveryId { get; set; } = string.Empty;
    public string DroneId { get; set; } = string.Empty;
    public DateTime Eta { get; set; }
}

public class JobStatusResponse
{
    public string JobId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
}

public class DeliveryStatusResponse
{
    public string DeliveryId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public object? Location { get; set; }
}
