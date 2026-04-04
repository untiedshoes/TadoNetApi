namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a heating circuit configured for a home.
/// </summary>
public class HeatingCircuit
{
    /// <summary>
    /// The heating circuit number.
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// The full serial number of the device controlling the circuit.
    /// </summary>
    public string? DriverSerialNo { get; set; }

    /// <summary>
    /// The short serial number of the device controlling the circuit.
    /// </summary>
    public string? DriverShortSerialNo { get; set; }
}