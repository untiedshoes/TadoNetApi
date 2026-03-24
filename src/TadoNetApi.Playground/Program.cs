using System;
using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Infrastructure.Config;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // ----------------------------
        // Tado API Configuration
        // ----------------------------
        var config = new TadoApiConfig
        {
            Username = "YOUR_TADO_EMAIL",
            Password = "YOUR_TADO_PASSWORD",
            MaxRetries = 5,
            InitialRetryDelayMs = 1000
        };

        // ----------------------------
        // Dependency Injection Setup
        // ----------------------------
        var services = new ServiceCollection();

        // Register the configuration
        services.AddSingleton(config);

        // Register HttpClient for Tado API
        services.AddHttpClient<TadoHttpClient>();

        // Register all domain services
        services.AddSingleton<IHomeService, TadoHomeService>();
        services.AddSingleton<IUserService, TadoUserService>();
        services.AddSingleton<IDeviceService, TadoDeviceService>();
        services.AddSingleton<IZoneService, TadoZoneService>();
        services.AddSingleton<IWeatherService, TadoWeatherService>();
        services.AddSingleton<IScheduleService, TadoScheduleService>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // ----------------------------
        // Resolve services
        // ----------------------------
        var homeService = serviceProvider.GetRequiredService<IHomeService>();
        var userService = serviceProvider.GetRequiredService<IUserService>();
        var deviceService = serviceProvider.GetRequiredService<IDeviceService>();
        var zoneService = serviceProvider.GetRequiredService<IZoneService>();
        var weatherService = serviceProvider.GetRequiredService<IWeatherService>();
        var scheduleService = serviceProvider.GetRequiredService<IScheduleService>();

        var cancellationToken = new CancellationToken();

        // ----------------------------
        // Playground Example Usage
        // ----------------------------

        // Fetch Tado user info
        Console.WriteLine("Fetching Tado User from API...");
        var user = await userService.GetUserAsync(cancellationToken);
        Console.WriteLine($"User: {user?.Name} ({user?.Email})\n");

        // Fetch homes
        Console.WriteLine("Fetching all homes...");
        var homes = await homeService.GetHomesAsync(cancellationToken);
        foreach (var home in homes)
        {
            Console.WriteLine($"Home: {home.Name} ({home.Id})");

            // Fetch zones in the home
            Console.WriteLine("Fetching zones for this home...");
            var zones = await zoneService.GetZonesAsync(home.Id, cancellationToken);
            foreach (var zone in zones)
            {
                Console.WriteLine($"  Zone: {zone.Name} (Current Temp: {zone.CurrentTemperature}°C)");

                // Fetch devices in the zone
                Console.WriteLine("  Fetching devices in this zone...");
                var devices = await deviceService.GetDevicesAsync(home.Id, zone.Id, cancellationToken);
                foreach (var device in devices)
                {
                    Console.WriteLine($"    Device: {device.Name} (ChildLock: {device.ChildLock})");
                }

                // Fetch weather
                Console.WriteLine("  Fetching weather info...");
                var weather = await weatherService.GetWeatherAsync(home.Id, cancellationToken);
                Console.WriteLine($"    Weather: {weather?.Temperature}°C, {weather?.Condition}");

                // Fetch schedules
                Console.WriteLine("  Fetching schedules...");
                var schedules = await scheduleService.GetZoneScheduleAsync(home.Id, zone.Id, cancellationToken);
                Console.WriteLine("    Zone Schedules:");
                foreach (var entry in schedules)
                {
                    Console.WriteLine($"      {entry.Name}: {entry.TargetTemperature}°C from {entry.StartTime:t} to {entry.EndTime:t} (Active: {entry.IsActive})");
                }

                // Fetch active program
                Console.WriteLine("  Fetching currently active program...");
                var program = await scheduleService.GetZoneProgramAsync(home.Id, zone.Id, cancellationToken);
                Console.WriteLine($"    Active Program: {program}\n");
            }
        }

        Console.WriteLine("Playground complete! All data fetched successfully.");
    }
}