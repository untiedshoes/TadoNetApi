using System;
using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;

namespace TadoNetApi.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
                var cfg = sp.GetRequiredService<TadoApiConfig>();

                return new TadoAuthService(factory, cfg, logger);
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

            // ----------------------------
            // Domain services
            // ----------------------------
            services.AddTransient<IHomeService, TadoHomeService>();
            services.AddTransient<IUserService, TadoUserService>();
            services.AddTransient<IDeviceService, TadoDeviceService>();
            services.AddTransient<IZoneService, TadoZoneService>();
            services.AddTransient<IWeatherService, TadoWeatherService>();
            services.AddTransient<IScheduleService, TadoScheduleService>();

            return services;
        }
    }
}