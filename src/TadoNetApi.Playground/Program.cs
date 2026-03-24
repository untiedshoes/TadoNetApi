using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using System;
using System.Threading;

var services = new ServiceCollection();

// 1️⃣ Configure Tado login
var tadoConfig = new TadoApiConfig
{
    Username = "your-email@example.com",
    Password = "your-password",
    MaxRetries = 5,
    InitialRetryDelayMs = 1000
};

// 2️⃣ Register DI
services.AddSingleton(tadoConfig);
services.AddHttpClient<TadoHttpClient>();
services.AddSingleton<IHomeService, TadoHomeService>();
services.AddSingleton<IUserService, TadoUserService>();
services.AddSingleton<IDeviceService, TadoDeviceService>();
services.AddSingleton<IZoneService, TadoZoneService>();
services.AddSingleton<IWeatherService, TadoWeatherService>();
services.AddSingleton<IZoneOverlayService, TadoZoneOverlayService>();

var serviceProvider = services.BuildServiceProvider();

// 3️⃣ Resolve services
var httpClient = serviceProvider.GetRequiredService<TadoHttpClient>();
var homeService = serviceProvider.GetRequiredService<IHomeService>();
var userService = serviceProvider.GetRequiredService<IUserService>();
var deviceService = serviceProvider.GetRequiredService<IDeviceService>();
var zoneService = serviceProvider.GetRequiredService<IZoneService>();
var weatherService = serviceProvider.GetRequiredService<IWeatherService>();
var overlayService = serviceProvider.GetRequiredService<IZoneOverlayService>();

// 4️⃣ Cancellation token (timeout 15s)
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
var cancellationToken = cts.Token;

try
{
    // 🔑 Authenticate
    Console.WriteLine("Authenticating with Tado...");
    await httpClient.AuthenticateAsync(cancellationToken);
    Console.WriteLine("Authentication successful.\n");

    // 👤 Fetch user
    var user = await userService.GetUserAsync(cancellationToken);
    Console.WriteLine($"User: {user?.Name} ({user?.Email})\n");

    // 🏠 Fetch all homes
    var homes = await homeService.GetHomesAsync(cancellationToken);
    foreach (var home in homes)
    {
        Console.WriteLine($"Home: {home.Name} ({home.Id})");

        // 🌤 Fetch weather
        var weather = await weatherService.GetWeatherAsync(home.Id, cancellationToken);
        Console.WriteLine($"  Weather: {weather.Condition}, Temp: {weather.Temperature}°C, Humidity: {weather.Humidity}%");

        // 🔥 Fetch zones
        var zones = await zoneService.GetZonesAsync(home.Id, cancellationToken);
        foreach (var zone in zones)
        {
            Console.WriteLine($"  Zone: {zone.Name} (Temp: {zone.CurrentTemperature}°C, Target: {zone.TargetTemperature}°C, Humidity: {zone.Humidity}%)");

            // 💡 Fetch devices in zone
            var devices = await deviceService.GetDevicesAsync(home.Id, zone.Id, cancellationToken);
            foreach (var device in devices)
            {
                Console.WriteLine($"    Device: {device.Name} (ChildLock: {device.ChildLock})");
            }

            // 🌡 Get and set overlay
            var overlay = await overlayService.GetZoneOverlayAsync(home.Id, zone.Id, cancellationToken);
            Console.WriteLine($"    Overlay: {overlay.Type}, {overlay.TargetTemperature}°C");

            // Example: Set new temperature overlay
            await overlayService.SetZoneTemperatureAsync(home.Id, zone.Id, 21, cancellationToken);
            Console.WriteLine($"    Overlay set to 21°C");
        }

        Console.WriteLine(); // Blank line between homes
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation canceled (timeout).");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}