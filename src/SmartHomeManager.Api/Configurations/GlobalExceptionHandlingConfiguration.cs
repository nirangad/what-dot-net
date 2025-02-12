using SmartHomeManager.Api.Middleware;

namespace SmartHomeManager.Configurations;

public static class GlobalExceptionHandlingConfiguration
{
    public static void UseGlobalExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
