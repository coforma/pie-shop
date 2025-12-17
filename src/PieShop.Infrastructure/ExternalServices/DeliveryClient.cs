using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PieShop.Infrastructure.ExternalServices;

/// <summary>
/// Client for delivery service
/// ISSUES: No error categorization, no bulkhead pattern, missing timeout configuration
/// </summary>
public class DeliveryClient : IDeliveryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeliveryClient> _logger;
    // Another hard-coded URL
    private readonly string _baseUrl = "http://localhost:8083";

    public DeliveryClient(HttpClient httpClient, ILogger<DeliveryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DeliveryResponse> DeliverAsync(object request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/deliveries", content);
            
            // Doesn't categorize errors (400 vs 500 vs 503)
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Delivery service returned error: {StatusCode} - {Error}", 
                    response.StatusCode, errorBody);
                throw new Exception($"Delivery service error: {response.StatusCode}");
            }
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<DeliveryResponse>(responseBody);
            
            if (result == null)
            {
                throw new Exception("Failed to parse delivery response");
            }
            
            _logger.LogInformation("Delivery scheduled: {DeliveryId} via drone {DroneId}", 
                result.DeliveryId, result.DroneId);
                
            return result;
        }
        catch (HttpRequestException ex)
        {
            // Doesn't distinguish network errors from service errors
            _logger.LogError(ex, "HTTP request to delivery service failed");
            throw new Exception("Delivery service unreachable", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse delivery service response");
            throw new Exception("Invalid response from delivery service", ex);
        }
        // No catch for TaskCanceledException - timeouts will propagate as unhandled
    }

    public async Task<DeliveryStatusResponse> GetDeliveryStatusAsync(string deliveryId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/deliveries/{deliveryId}");
        response.EnsureSuccessStatusCode(); // Throws on error - not user-friendly
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DeliveryStatusResponse>(responseBody) 
            ?? throw new Exception("Deserialization failed");
    }
}
