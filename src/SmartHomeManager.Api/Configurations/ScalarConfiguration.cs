using Scalar.AspNetCore;

namespace SmartHomeManager.Configurations;

public static class ScalarConfiguration
{
    public static void UseScalar(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
}
