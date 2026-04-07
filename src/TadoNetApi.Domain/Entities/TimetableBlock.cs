namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a scheduled block within a timetable.
/// </summary>
public class TimetableBlock
{
    /// <summary>
    /// The day type covered by the block.
    /// </summary>
    public string? DayType { get; set; }

    /// <summary>
    /// The block start time in 24-hour clock notation.
    /// </summary>
    public string? Start { get; set; }

    /// <summary>
    /// The block end time in 24-hour clock notation.
    /// </summary>
    public string? End { get; set; }

    /// <summary>
    /// Indicates whether the block should remain active even when the home is in AWAY mode.
    /// </summary>
    public bool? GeolocationOverride { get; set; }

    /// <summary>
    /// The setting applied during this timetable block.
    /// </summary>
    public Setting? Setting { get; set; }
}