using Microsoft.Extensions.Configuration;
using MySpot.Infrastructure;

namespace MySpot.Tests.Integration;

public sealed class OptionsProvider
{
    private readonly IConfigurationRoot _configuration = GetConfigurationRoot();
    
    private static IConfigurationRoot GetConfigurationRoot()
        => new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", true)
            .AddEnvironmentVariables()
            .Build();


    public T Get<T>(string sectionName) where T : class, new()
        => _configuration.GetOptions<T>(sectionName);
}