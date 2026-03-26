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

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
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
            var user = await userService.GetUserAsync(cancellationToken);
            Console.WriteLine($"👤 User: {user?.Name} ({user?.Email})");

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