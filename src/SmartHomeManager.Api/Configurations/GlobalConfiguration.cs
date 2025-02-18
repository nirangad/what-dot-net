using System;

namespace SmartHomeManager.Configurations;

public static class GlobalConfiguration
{
    public static readonly string APP_NAME = "SMART_HOME_MANAGER_";
    public static T? GetLocalSettings<T>(this WebApplicationBuilder builder, string sectionName) where T : class
    {
        return builder.Configuration.GetSection(sectionName).Get<T>();
    }

    public static T? GetSecretSettings<T>(this WebApplication app, string sectionName) where T : class
    {
        return app.Configuration.GetRequiredSection(sectionName).Get<T>();
    }
}
