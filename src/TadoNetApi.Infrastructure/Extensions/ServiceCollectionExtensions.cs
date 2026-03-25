using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Http;

namespace TadoNetApi.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTadoInfrastructure(
            this IServiceCollection services,
            TadoApiConfig config)
        {
            // Config
            services.AddSingleton(config);

            // Auth
            services.AddSingleton<TadoAuthService>();

            // Handlers
            services.AddTransient<AuthDelegatingHandler>();

            // HttpClient
            services.AddHttpClient<ITadoHttpClient, TadoHttpClient>(client =>
            {
                client.BaseAddress = new Uri(TadoApiEndpoints.ApiBaseUrl);
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>();

            return services;
        }
    }
}