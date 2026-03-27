using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;
using TadoNetApi.Infrastructure.Auth;

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
            Console.WriteLine("❌ Missing Tado credentials in environment variables.");
            return;
        }

        // Setup DI and logging
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTadoInfrastructure(config);

        var provider = services.BuildServiceProvider();

        // Resolve services
        var authService = provider.GetRequiredService<ITadoAuthService>();
        var userService = provider.GetRequiredService<IUserService>();
        var homeService = provider.GetRequiredService<IHomeService>();

        var cancellationToken = CancellationToken.None;

        Console.WriteLine("🚀 Starting Tado Playground...");

        try
        {
            // 1️⃣ Start device authorization
            Console.WriteLine("🔑 Requesting device authorisation...");
            var deviceCodeInfo = await authService.StartDeviceAuthorisationAsync(cancellationToken);

            // Prefer verification_uri_complete if available, fallback to verification_uri
            var verificationUri = !string.IsNullOrEmpty(deviceCodeInfo.VerificationUriComplete)
                                  ? deviceCodeInfo.VerificationUriComplete
                                  : deviceCodeInfo.VerificationUri;

            Console.WriteLine($"📋 Please visit {verificationUri} and enter the code: {deviceCodeInfo.UserCode}");
            Console.WriteLine("⏳ Waiting for user authorisation...");

            // 2️⃣ Poll token endpoint until user approves, respecting Tado's interval & expiration
            var token = await authService.WaitForDeviceTokenAsync(
                deviceCodeInfo.DeviceCode,
                intervalSeconds: deviceCodeInfo.Interval,
                maxWaitSeconds: deviceCodeInfo.ExpiresIn,
                cancellationToken: cancellationToken);

            Console.WriteLine("✅ Device authorised successfully!");

            // 3️⃣ Call API using authenticated token
            var user = await userService.GetUserAsync(cancellationToken);
            Console.WriteLine($"👤 User: {user?.Name} ({user?.Email})");

            var homes = await homeService.GetHomesAsync(cancellationToken);
            foreach (var home in homes)
            {
                Console.WriteLine($"🏠 Home: {home.Name} (ID: {home.Id})");
            }

            Console.WriteLine("🎉 Playground complete!");
        }
        catch (TimeoutException)
        {
            Console.WriteLine("⚠️ Device authorisation timed out. Make sure you enter the code in the browser promptly.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("⚠️ An error occurred:");
            Console.WriteLine(ex.Message);
        }
    }
}