using Asp.Versioning;
using Asp.Versioning.Conventions;
using Scalar.AspNetCore;
using SmartHomeManager.Api.Middleware;
using SmartHomeManager.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; // Use default version if none is specified
    options.DefaultApiVersion = new ApiVersion(1, 0); // Default API version is 1.0
    options.ReportApiVersions = true; // Add version info in response headers

    // Read API version from header
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

// Global Logging Configuration
builder.ConfigureLogging();

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
                       .HasApiVersion(1, 0)
                       .HasApiVersion(2, 0)
                       .ReportApiVersions() // Add version info in response headers
                       .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Global Error Handling Middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Global Logging Middleware
app.UseMiddleware<LoggingMiddleware>();

// Redirects HTTP traffic to HTTPS
app.UseHttpsRedirection();

app.MapGet("/greetings", (HttpContext httpContext) =>
{
    var apiVersion = httpContext.GetRequestedApiVersion();
    return Results.Ok($"[{apiVersion}] Hello, World..! 🌏");
}).WithApiVersionSet(apiVersionSet).MapToApiVersion(1, 0);

app.MapGet("/greetings", (HttpContext httpContext) =>
{
    var apiVersion = httpContext.GetRequestedApiVersion();
    return Results.Ok($"[{apiVersion}] Hello, Universe..! 👾");
}).WithApiVersionSet(apiVersionSet).MapToApiVersion(2, 0);

// Define Minimal API Endpoints
app.MapGet("/success", () =>
{
    return Results.Ok(new { Message = "This endpoint works fine!" });
}).WithApiVersionSet(apiVersionSet).MapToApiVersion(2, 0);

app.MapGet("/error", () =>
{
    // throw new InvalidOperationException("This is a test exception.");
    Random rnd = new Random();
    int a = rnd.Next(1, 4);
    int b = rnd.Next(1, 4);
    return Results.Ok(new { Message = 1 / (a - b) });
}).WithApiVersionSet(apiVersionSet).MapToApiVersion(2, 0);

await app.RunAsync();