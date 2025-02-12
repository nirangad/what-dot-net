using Asp.Versioning;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;

namespace SmartHomeManager.Configurations;

public static class ApiVersioningConfiguration
{
    public static ApiVersionSet PrepareApiVersionSet(this WebApplication app)
    {
        return app.NewApiVersionSet()
        .HasApiVersion(1, 0)
        .HasApiVersion(2, 0)
        .ReportApiVersions() // Add version info in response headers
        .Build();
    }

    public static void UseApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true; // Use default version if none is specified
            options.DefaultApiVersion = new ApiVersion(1, 0); // Default API version is 1.0
            options.ReportApiVersions = true; // Add version info in response headers

            // Read API version from header
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });
    }

}
