using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyMonster.Exceptions; 

namespace PennyMonster.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var title = "An unexpected server error occurred.";
        var detail = "Please contact support if this issue persists.";

        switch (exception)
        {
            // Case 1: Our Custom Business Rule Violations
            // 100% safe to show the message because WE wrote it.
            case BusinessRuleException businessEx:
                statusCode = (int)HttpStatusCode.BadRequest; // 400
                title = "Business Rule Violation";
                detail = businessEx.Message;
                // We don't need to log this as an "Error" because it's just a user mistake, but you could log it as a Warning.
                break;

            // Case 2: User cancelled the request mid-flight (e.g., closed the app)
            case OperationCanceledException:
                statusCode = 499; // 499 Client Closed Request (Nginx standard)
                title = "Request Cancelled";
                detail = "The request was cancelled by the client.";
                // We purposely DO NOT log this as an error to prevent log spam!
                logger.LogInformation("Request was cancelled by the user.");
                break;

            // Case 3: Security / Permissions
            case UnauthorizedAccessException unauthEx:
                statusCode = (int)HttpStatusCode.Forbidden; // 403
                title = "Access Denied";
                // SECURITY COMMENT: We purposely ignore unauthEx.Message here. 
                // Exposing the exact reason for authorization failure can leak internal system details to attackers.
                detail = "You do not have permission to access or modify this resource.";
                logger.LogWarning("Unauthorized access attempt: {Message}", unauthEx.Message);
                break;

            case DbUpdateConcurrencyException dbConcurrencyEx:
                statusCode = (int)HttpStatusCode.Conflict; // 409
                title = "Data Conflict";
                detail = "The resource was modified by another process. Please refresh and try again.";
                logger.LogError(dbConcurrencyEx, "Concurrency exception occurred.");
                break;

            case DbUpdateException dbUpdateEx:
                statusCode = (int)HttpStatusCode.BadRequest; // 400
                title = "Database Update Error";
                detail = "Could not save changes. Please ensure your data is valid and try again.";
                logger.LogError(dbUpdateEx, "Database update failed.");
                break;

            // Default Case: True 500 server crashes
            default:
                // Log the massive ugly error for us to fix later
                logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                break;
        }

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Fix JSON Naming Policy (camelCase) so 'Title' becomes 'title', matching the rest of the API
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var result = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(result);
    }
}