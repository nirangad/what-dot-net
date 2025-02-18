using Asp.Versioning.Conventions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SmartHomeManager.Configurations;
using SmartHomeManager.Configurations.Settings;

var builder = WebApplication.CreateBuilder(args);

// Setting up caching service
builder.UseRedis();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add API versioning
builder.UseApiVersioning();

// Global Logging Configuration
builder.ConfigureLogging();

// Add AWS Secrets Manager
// Add AWS Systems Manager Parameter Store to Configuration
builder.UseAwsSecretManager();

AwsSettings? awsSettings = builder.GetLocalSettings<AwsSettings>(AwsConfiguration.AWS_SETTINGS);
Console.WriteLine($"AWS Settings fetched from Secret Manager :\n {Newtonsoft.Json.JsonConvert.SerializeObject(awsSettings)}");

var app = builder.Build();

// API Version Set
var apiVersionSet = app.PrepareApiVersionSet();

// Configure the HTTP request pipeline.
app.UseScalar();

// Global Error Handling Middleware
app.UseGlobalExceptionHandling();

// Global Logging Middleware with Serilog
app.UseSerilog();

// Redirects HTTP traffic to HTTPS
// app.UseHttpsRedirection();

app.MapGet("/test-config", async (IDistributedCache cache) =>
{
    string key = "Test_Key";

    // Read the cached value
    string? cachedTime = await cache.GetStringAsync(key);

    // Write a test value
    Random rnd = new Random();
    string newTime = DateTime.Now.ToString("h:mm:ss tt");

    await cache.SetStringAsync(key, newTime, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });

    return Results.Ok(new
    {
        CachedValue = cachedTime ?? "[Cache Miss!]",
        NewValue = newTime
    });
});

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
    Random rnd = new Random();
    int a = rnd.Next(1, 4);
    int b = rnd.Next(1, 4);
    return Results.Ok(new { Message = 1 / (a - b) });
}).WithApiVersionSet(apiVersionSet).MapToApiVersion(2, 0);

await app.RunAsync();