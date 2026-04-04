namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the boiler flow-temperature optimisation settings for a home.
/// </summary>
public class FlowTemperatureOptimisation
{
    /// <summary>
    /// Indicates whether multiple boiler-control devices are present.
    /// </summary>
    public bool? HasMultipleBoilerControlDevices { get; set; }

    /// <summary>
    /// The configured maximum flow temperature.
    /// </summary>
    public int? MaxFlowTemperature { get; set; }

    /// <summary>
    /// Supported constraints for the maximum flow temperature.
    /// </summary>
    public FlowTemperatureOptimisationConstraints? MaxFlowTemperatureConstraints { get; set; }

    /// <summary>
    /// Auto-adaptation settings when supported.
    /// </summary>
    public FlowTemperatureOptimisationAutoAdaptation? AutoAdaptation { get; set; }

    /// <summary>
    /// Serial number of the OpenTherm device when available.
    /// </summary>
    public string? OpenThermDeviceSerialNumber { get; set; }
}

/// <summary>
/// Represents min/max constraints for flow temperature.
/// </summary>
public class FlowTemperatureOptimisationConstraints
{
    /// <summary>
    /// Minimum supported flow temperature.
    /// </summary>
    public int? Min { get; set; }

    /// <summary>
    /// Maximum supported flow temperature.
    /// </summary>
    public int? Max { get; set; }
}

/// <summary>
/// Represents auto-adaptation settings for boiler flow temperature.
/// </summary>
public class FlowTemperatureOptimisationAutoAdaptation
{
    /// <summary>
    /// Indicates whether auto-adaptation is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// The maximum flow temperature used by auto-adaptation when available.
    /// </summary>
    public int? MaxFlowTemperature { get; set; }
}