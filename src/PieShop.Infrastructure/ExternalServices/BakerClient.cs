using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PieShop.Infrastructure.ExternalServices;

/// <summary>
/// Client for baker service
/// ISSUES: No timeout configuration, no circuit breaker, basic retry only
/// </summary>
public class BakerClient : IBakerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BakerClient> _logger;
    private const string BaseUrl = "http://localhost:8082"; // Hard-coded!

    public BakerClient(HttpClient httpClient, ILogger<BakerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        // Using default timeout - not explicitly configured
    }

    public async Task<BakeResponse> BakeAsync(object request)
    {
        var maxRetries = 3; // Magic number
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Calling baker service (attempt {Attempt})", retryCount + 1);
                
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/v1/bake", content);
                response.EnsureSuccessStatusCode();
                
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BakeResponse>(responseBody);
                
                if (result == null)
                {
                    throw new Exception("Failed to deserialize baker response");
                }
                
                _logger.LogInformation("Baking job created: {JobId} in oven {OvenId}", 
                    result.JobId, result.OvenId);
                    
                return result;
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "Baker service call failed (attempt {Attempt})", retryCount);
                
                if (retryCount >= maxRetries)
                {
                    _logger.LogError("Baker service failed after {MaxRetries} attempts", maxRetries);
                    throw;
                }
                
                // Simple linear backoff - should be exponential!
                await Task.Delay(1000 * retryCount); // 1s, 2s, 3s
            }
        }

        throw new Exception("Baker service call failed");
    }

    public async Task<JobStatusResponse> GetJobStatusAsync(string jobId)
    {
        // No error handling at all here!
        var response = await _httpClient.GetAsync($"{BaseUrl}/api/v1/jobs/{jobId}");
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JobStatusResponse>(responseBody) 
            ?? throw new Exception("Deserialization failed");
    }
}
