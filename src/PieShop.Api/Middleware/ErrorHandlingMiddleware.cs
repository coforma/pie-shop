using System.Net;
using System.Text.Json;

namespace PieShop.Api.Middleware;

/// <summary>
/// Basic error handling middleware
/// TODO: Add more sophisticated error handling, logging, correlation IDs
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // TODO: Don't expose internal error details in production!
        // TODO: Add correlation ID for tracking
        // TODO: Different handling for different exception types
        
        var code = HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(new
        {
            error = "INTERNAL_ERROR",
            message = exception.Message, // Exposing internal messages is a security risk!
            stackTrace = exception.StackTrace // Should NEVER be returned in production!
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
