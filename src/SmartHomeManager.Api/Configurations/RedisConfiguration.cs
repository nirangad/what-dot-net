using System;
using System.Threading.Tasks;

namespace SmartHomeManager.Configurations;

public static class RedisConfiguration
{
    public static void UseRedis(this WebApplicationBuilder builder)
    {
        string redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
        // Redis Configuration
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = GlobalConfiguration.APP_NAME;
        });
    }
}
