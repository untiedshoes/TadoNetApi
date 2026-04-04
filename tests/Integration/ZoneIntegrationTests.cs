using System.Threading.Tasks;
using TadoNetApi.Application.Services;
using TadoNetApi.Infrastructure.Config;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TadoNetApi.Tests.Integration
{
    /// <summary>
    /// Integration tests for TadoNetApi.
    /// Requires valid credentials via environment variables.
    /// </summary>
    public class TadoIntegrationTests
    {
        private readonly ServiceProvider _provider;

        public TadoIntegrationTests()
        {
            var services = new ServiceCollection();
            var config = new TadoApiConfig
            {
                Username = System.Environment.GetEnvironmentVariable("TADO_USERNAME") ?? "",
                Password = System.Environment.GetEnvironmentVariable("TADO_PASSWORD") ?? "",
                MaxRetries = 3
            };

            services.AddTransient<UserAppService>();
            services.AddTransient<HomeAppService>();
            services.AddTransient<ZoneAppService>();
            services.AddTransient<DeviceAppService>();

            _provider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Basic smoke test: Can retrieve user and homes.
        /// </summary>
        [Fact(Skip = "Requires real credentials")]
        public async Task CanRetrieveUserAndHomes()
        {
            var userService = _provider.GetRequiredService<UserAppService>();
            var homeService = _provider.GetRequiredService<HomeAppService>();

            var user = await userService.GetMeAsync(CancellationToken.None);
            Assert.NotNull(user);
            Assert.NotEmpty(user.Homes);

            var home = await homeService.GetHomeAsync((int)user.Homes[0].Id!, CancellationToken.None);
            Assert.NotNull(home);
        }
    }
}