using System;
using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Application.Services;

namespace TadoNetApi.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTadoInfrastructure(
            this IServiceCollection services)
            => services.AddTadoInfrastructure(new TadoApiConfig());

        public static IServiceCollection AddTadoInfrastructure(
            this IServiceCollection services,
            TadoApiConfig config)
        {
            // ----------------------------
            // Config
            // ----------------------------
            services.AddSingleton(config);

            // ----------------------------
            // HttpClient for AUTH (named client)
            // ----------------------------
            services.AddHttpClient("TadoAuth", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // ----------------------------
            // Auth Service (Singleton IMPORTANT)
            // ----------------------------
            services.AddSingleton<ITadoAuthService>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<TadoAuthService>>();

                return new TadoAuthService(factory, logger);
            });

            // ----------------------------
            // Handlers
            // ----------------------------
            services.AddTransient<AuthDelegatingHandler>();
            services.AddTransient<RetryDelegatingHandler>();

            // ----------------------------
            // Main API HttpClient
            // ----------------------------
            services.AddHttpClient<ITadoHttpClient, TadoHttpClient>(client =>
            {
                client.BaseAddress = new Uri(TadoApiEndpoints.ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddHttpMessageHandler<RetryDelegatingHandler>();

            services.AddHttpClient<IPublicTadoHttpClient, TadoHttpClient>(client =>
            {
                client.BaseAddress = new Uri(TadoApiEndpoints.ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<RetryDelegatingHandler>();

            // ----------------------------
            // Domain services
            // ----------------------------
            services.AddTransient<IHomeService, TadoHomeService>();
            services.AddTransient<IUserService, TadoUserService>();
            services.AddTransient<IDeviceService, TadoDeviceService>();
            services.AddTransient<IZoneService, TadoZoneService>();
            services.AddTransient<IWeatherService, TadoWeatherService>();
            services.AddTransient<IBridgeService, TadoBridgeService>();
            services.AddTransient<IBoilerByBridgeService, TadoBoilerByBridgeService>();

            // ----------------------------
            // Application services
            // ---------------------------- 
            services.AddTransient<HomeAppService>();
            services.AddTransient<UserAppService>();
            services.AddTransient<ZoneAppService>();
            services.AddTransient<DeviceAppService>();
            services.AddTransient<WeatherAppService>();
            services.AddTransient<BridgeAppService>();
            services.AddTransient<BoilerByBridgeAppService>();

            return services;
        }
    }
}