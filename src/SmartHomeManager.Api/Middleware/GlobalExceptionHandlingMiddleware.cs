namespace SmartHomeManager.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Process the request
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "[GlobalExceptionHandling] An unhandled exception has occurred.");

            // Generate custom error response
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set response status code and content type
        context.Response.StatusCode = exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.ContentType = "application/json";

        // Custom error response
        var errorResponse = new
        {
            Error = new
            {
                exception.Message,
                Type = exception.GetType().Name, // Exception type
                context.Response.StatusCode
            },
            TraceId = context.TraceIdentifier
        };

        return context.Response.WriteAsJsonAsync(errorResponse);
    }
}
