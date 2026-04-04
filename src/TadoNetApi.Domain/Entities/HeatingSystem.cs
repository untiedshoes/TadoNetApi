namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the heating system configuration for a home.
/// </summary>
public class HeatingSystem
{
    /// <summary>
    /// Boiler details for the home.
    /// </summary>
    public Boiler? Boiler { get; set; }

    /// <summary>
    /// Underfloor heating details for the home.
    /// </summary>
    public UnderfloorHeating? UnderfloorHeating { get; set; }
}

/// <summary>
/// Represents the boiler configuration for a home.
/// </summary>
public class Boiler
{
    /// <summary>
    /// Indicates whether a boiler is present.
    /// </summary>
    public bool? Present { get; set; }

    /// <summary>
    /// Tado-specific boiler identifier.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Indicates whether the boiler was identified.
    /// </summary>
    public bool? Found { get; set; }
}

/// <summary>
/// Represents underfloor heating configuration for a home.
/// </summary>
public class UnderfloorHeating
{
    /// <summary>
    /// Indicates whether underfloor heating is present.
    /// </summary>
    public bool? Present { get; set; }
}