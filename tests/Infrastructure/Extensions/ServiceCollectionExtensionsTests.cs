using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;
using TadoNetApi.Infrastructure.Http;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Extensions;

/// <summary>
/// Unit tests for <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    /// <summary>
    /// AddTadoInfrastructure with explicit config registers expected core services.
    /// </summary>
    [Fact(DisplayName = "AddTadoInfrastructure with explicit config registers expected core services")]
    public void AddTadoInfrastructure_WithExplicitConfig_RegistersExpectedCoreServices()
    {
        var services = new ServiceCollection();
        var config = new TadoApiConfig { MaxRetries = 5 };

        services.AddLogging();
        services.AddTadoInfrastructure(config);

        Assert.Contains(services, d => d.ServiceType == typeof(TadoApiConfig));
        Assert.Contains(services, d => d.ServiceType == typeof(ITadoAuthService));
        Assert.Contains(services, d => d.ServiceType == typeof(ITadoHttpClient));
        Assert.Contains(services, d => d.ServiceType == typeof(IPublicTadoHttpClient));
        Assert.Contains(services, d => d.ServiceType == typeof(IHomeService));
        Assert.Contains(services, d => d.ServiceType == typeof(IUserService));
        Assert.Contains(services, d => d.ServiceType == typeof(IDeviceService));
        Assert.Contains(services, d => d.ServiceType == typeof(IZoneService));
        Assert.Contains(services, d => d.ServiceType == typeof(IWeatherService));
        Assert.Contains(services, d => d.ServiceType == typeof(HomeAppService));
        Assert.Contains(services, d => d.ServiceType == typeof(DeviceAppService));
        Assert.Contains(services, d => d.ServiceType == typeof(WeatherAppService));
    }

    /// <summary>
    /// AddTadoInfrastructure default overload registers a config singleton instance.
    /// </summary>
    [Fact(DisplayName = "AddTadoInfrastructure default overload registers config singleton")]
    public void AddTadoInfrastructure_DefaultOverload_RegistersConfigSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddTadoInfrastructure();

        using var provider = services.BuildServiceProvider();
        var configs = provider.GetServices<TadoApiConfig>().ToList();

        Assert.Single(configs);
        Assert.NotNull(configs[0]);
    }

    /// <summary>
    /// Registered HttpClient and service factories can be resolved and instantiated.
    /// </summary>
    [Fact(DisplayName = "AddTadoInfrastructure resolves HttpClient factory auth service and typed clients")]
    public void AddTadoInfrastructure_ResolvesHttpClientFactoryAuthServiceAndTypedClients()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTadoInfrastructure(new TadoApiConfig());

        using var provider = services.BuildServiceProvider();

        var httpFactory = provider.GetRequiredService<IHttpClientFactory>();
        var authClient = httpFactory.CreateClient("TadoAuth");
        var authService = provider.GetRequiredService<ITadoAuthService>();
        var publicClient = provider.GetRequiredService<IPublicTadoHttpClient>();
        var appService = provider.GetRequiredService<HomeAppService>();

        Assert.NotNull(httpFactory);
        Assert.NotNull(authClient);
        Assert.Equal(TimeSpan.FromSeconds(30), authClient.Timeout);
        Assert.NotNull(authService);
        Assert.NotNull(publicClient);
        Assert.NotNull(appService);
    }
}