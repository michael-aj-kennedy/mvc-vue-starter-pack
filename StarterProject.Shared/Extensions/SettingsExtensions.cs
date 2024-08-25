using Microsoft.Extensions.Configuration;

namespace StarterProject.Shared.Extensions;

public static class SettingsExtensions
{
    private const string _appSettingsFileExtension = ".json";
    private const string _appSettingsFileName = "AppSettings";

    public static IConfigurationBuilder UseConfiguration(this IConfigurationBuilder configurationBuilder) =>
        configurationBuilder
            .UseSettings();

    private static IConfigurationBuilder UseSettings(this IConfigurationBuilder config)
    {
        config.SetBasePath(Directory.GetCurrentDirectory());

        config.AddJsonFile($"{_appSettingsFileName}{_appSettingsFileExtension}", false, true);

        return config;
    }
}