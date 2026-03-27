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
            // Auth Service (typed HttpClient)
            // ----------------------------
            services.AddHttpClient<ITadoAuthService, TadoAuthService>();

            // ----------------------------
            // Handlers
            // ----------------------------
            services.AddTransient<AuthDelegatingHandler>();
            services.AddTransient<RetryDelegatingHandler>();

            // ----------------------------
            // Main API client
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