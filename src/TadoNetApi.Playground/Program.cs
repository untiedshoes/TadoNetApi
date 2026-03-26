using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new TadoApiConfig
        {
            Username = Environment.GetEnvironmentVariable("TADO_USERNAME") ?? "",
            Password = Environment.GetEnvironmentVariable("TADO_PASSWORD") ?? "",
            MaxRetries = 5,
            InitialRetryDelayMs = 1000
        };

        if (string.IsNullOrEmpty(config.Username) || string.IsNullOrEmpty(config.Password))
        {
            Console.WriteLine("❌ Missing Tado credentials.");
            return;
        }

        // ----------------------------
        // Setup DI and Logging
        // ----------------------------
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddTadoInfrastructure(config);

        var provider = services.BuildServiceProvider();

        var homeService = provider.GetRequiredService<IHomeService>();
        var userService = provider.GetRequiredService<IUserService>();
        var deviceService = provider.GetRequiredService<IDeviceService>();
        var zoneService = provider.GetRequiredService<IZoneService>();
        var weatherService = provider.GetRequiredService<IWeatherService>();
        var scheduleService = provider.GetRequiredService<IScheduleService>();

        var cancellationToken = CancellationToken.None;

        Console.WriteLine("🚀 Starting Tado Playground...");

        try
        {
            // ----------------------------
            // Retry-aware user fetch
            // ----------------------------
            var maxAttempts = 5;
            int attempt = 0;
            while (true)
            {
                attempt++;
                try
                {
                    var user = await userService.GetUserAsync(cancellationToken);
                    Console.WriteLine($"👤 User: {user?.Name} ({user?.Email})");
                    break;
                }
                catch (Exception ex) when (ex is System.Net.Http.HttpRequestException ||
                                           ex.Message.Contains("503") ||
                                           ex.Message.Contains("429"))
                {
                    Console.WriteLine($"⚠️ Attempt {attempt}/{maxAttempts} failed: {ex.Message}");
                    if (attempt >= maxAttempts)
                    {
                        Console.WriteLine("❌ Maximum retries reached. Cannot fetch user.");
                        throw;
                    }
                    int delay = config.InitialRetryDelayMs * attempt; // linear backoff
                    Console.WriteLine($"⏳ Waiting {delay}ms before retrying...");
                    await Task.Delay(delay, cancellationToken);
                }
            }

            // ----------------------------
            // Fetch homes
            // ----------------------------
            var homes = await homeService.GetHomesAsync(cancellationToken);
            foreach (var home in homes)
            {
                Console.WriteLine($"🏠 Home: {home.Name} (ID: {home.Id})");
            }

            Console.WriteLine("✅ Playground complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("⚠️ An error occurred:");
            Console.WriteLine(ex.Message);
        }
    }
}