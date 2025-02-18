using Amazon;
using Amazon.Extensions.NETCore.Setup;
using SmartHomeManager.Configurations.Settings;

namespace SmartHomeManager.Configurations;

public static class AwsConfiguration
{
    public readonly static string AWS_SETTINGS = "AwsSettings";
    public static void UseAwsSecretManager(this WebApplicationBuilder builder)
    {
        // AWS Account: 305493630724
        AwsSettings? awsSettings = GetAwsSettings(builder);
        builder.Configuration.AddSystemsManager(
            awsSettings?.AwsSecretManagerKey,
            new AWSOptions
            {
                Region = RegionEndpoint.GetBySystemName(awsSettings?.AwsRegion),
            }
        );
    }

    private static AwsSettings? GetAwsSettings(WebApplicationBuilder builder)
    {
        return builder.GetLocalSettings<AwsSettings>(AWS_SETTINGS);
    }
}