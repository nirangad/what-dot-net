using Serilog;

namespace SmartHomeManager.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // Optional: Add Serilog for structured logging
        builder.Host.UseSerilog((context, config) =>
        {
            config.WriteTo.Console()
                  .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
                  .Enrich.FromLogContext()
                  .Enrich.WithMachineName()
                  .Enrich.WithEnvironmentName();
        });
    }
}
