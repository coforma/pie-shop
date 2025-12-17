using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PieShop.Infrastructure.ExternalServices;

/// <summary>
/// Client for fruit picker service
/// ISSUES: No circuit breaker, no exponential backoff, hard-coded timeouts, basic error handling
/// </summary>
public class FruitPickerClient : IFruitPickerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FruitPickerClient> _logger;
    // Hard-coded base URL - should be in configuration!
    private const string BaseUrl = "http://localhost:8081";

    public FruitPickerClient(HttpClient httpClient, ILogger<FruitPickerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        // Hard-coded timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Initiates fruit picking
    /// TODO: Implement circuit breaker pattern
    /// TODO: Add exponential backoff retry
    /// </summary>
    public async Task<PickFruitResponse> PickFruitAsync(object request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // No retry logic, no timeout handling
            var response = await _httpClient.PostAsync($"{BaseUrl}/api/v1/pick-fruit", content);
            response.EnsureSuccessStatusCode();
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PickFruitResponse>(responseBody);
            
            if (result == null)
            {
                throw new Exception("Failed to deserialize response");
            }
            
            _logger.LogInformation("Fruit picking job created: {JobId}", result.JobId);
            return result;
        }
        catch (HttpRequestException ex)
        {
            // Basic error handling - doesn't distinguish between transient and permanent failures
            _logger.LogError(ex, "Failed to call fruit picker service");
            throw; // Just rethrow - no recovery attempt
        }
        catch (TaskCanceledException ex)
        {
            // Timeout handling
            _logger.LogError(ex, "Fruit picker service timed out");
            throw new Exception("Fruit picker service timeout", ex);
        }
    }

    /// <summary>
    /// Gets the status of a fruit picking job
    /// </summary>
    public async Task<JobStatusResponse> GetJobStatusAsync(string jobId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/v1/jobs/{jobId}");
            response.EnsureSuccessStatusCode();
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JobStatusResponse>(responseBody);
            
            return result ?? throw new Exception("Failed to deserialize status response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get job status for {JobId}", jobId);
            throw;
        }
    }
}
