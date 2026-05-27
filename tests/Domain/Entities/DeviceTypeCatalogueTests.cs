using TadoNetApi.Domain.Entities;
using Xunit;

namespace TadoNetApi.Tests.Domain.Entities;

/// <summary>
/// Unit tests for <see cref="DeviceTypeCatalogue"/>.
/// </summary>
public class DeviceTypeCatalogueTests
{
    /// <summary>
    /// GetFriendlyName returns null for blank values and unknown four-character codes.
    /// </summary>
    [Fact(DisplayName = "GetFriendlyName returns null for blank values and unknown four character codes")]
    public void GetFriendlyName_ReturnsNull_ForBlankValuesAndUnknownFourCharacterCodes()
    {
        Assert.Null(DeviceTypeCatalogue.GetFriendlyName(" "));
        Assert.Null(DeviceTypeCatalogue.GetFriendlyName("ZZ99"));
    }
}