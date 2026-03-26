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
            services.AddSingleton(config);

            // Auth service and HTTP client
            services.AddHttpClient<ITadoAuthService, TadoAuthService>();
            services.AddSingleton<ITadoAuthService, TadoAuthService>();

            // Delegating handlers
            services.AddTransient<AuthDelegatingHandler>();
            services.AddTransient<RetryDelegatingHandler>();

            // Typed HttpClient with auth + retry
            services.AddHttpClient<ITadoHttpClient, TadoHttpClient>(client =>
            {
                client.BaseAddress = new Uri(TadoApiEndpoints.ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            .AddHttpMessageHandler<RetryDelegatingHandler>();

            // Domain services
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