using System.Threading.Tasks;
using Xunit;

namespace TadoNetApi.Tests.Integration
{
    public class ZoneIntegrationTests
    {
        [Fact(Skip = "Requires live Tado credentials")]
        public async Task CanFetchZonesFromLiveApi()
        {
            // Arrange
            // TODO: Inject real TadoHttpClient with token

            // Act
            // TODO: Call GetZonesAsync

            // Assert
            // TODO: Validate results
        }
    }
}