using System.Diagnostics;
using System.Text;
using Microsoft.IO;

namespace SmartHomeManager.Api.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _streamManager;

    public LoggingMiddleware(
        RequestDelegate next,
        ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _streamManager = new RecyclableMemoryStreamManager();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Start timing
        var stopwatch = Stopwatch.StartNew();

        // Capture the request
        var requestBody = await GetRequestBodyAsync(context.Request);
        var requestInfo = new
        {
            context.Request.Method,
            context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            RequestBody = requestBody,
            context.Request.Headers
        };

        _logger.LogInformation(
            "HTTP {Method} {Path} started at {Time}",
            context.Request.Method,
            context.Request.Path,
            DateTime.UtcNow);

        // Enable response buffering
        var originalBody = context.Response.Body;
        using var responseStream = _streamManager.GetStream();
        context.Response.Body = responseStream;

        try
        {
            // Continue down the middleware pipeline
            await _next(context);

            // Capture timing and response
            stopwatch.Stop();
            var responseBody = await GetResponseBodyAsync(context.Response);

            var responseInfo = new
            {
                context.Response.StatusCode,
                ResponseBody = responseBody,
                context.Response.Headers,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };

            // Log based on status code
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }

            // Log detailed request/response for debugging if needed
            _logger.LogDebug(
                "Request/Response Details: {@RequestInfo} {@ResponseInfo}",
                requestInfo,
                responseInfo);

            // Copy the response to the original stream
            responseStream.Position = 0;
            await responseStream.CopyToAsync(originalBody);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "HTTP {Method} {Path} failed in {Elapsed:0.0000}ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        // Ensure the request body can be read multiple times
        request.EnableBuffering();

        using var streamReader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1024,
            leaveOpen: true);

        var requestBody = await streamReader.ReadToEndAsync();
        request.Body.Position = 0;
        return requestBody;
    }

    private static async Task<string> GetResponseBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Position = 0;
        return responseBody;
    }
}