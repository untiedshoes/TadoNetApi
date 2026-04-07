namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a supported timetable type for a zone schedule.
/// </summary>
public class TimetableType
{
    /// <summary>
    /// The numeric timetable type identifier.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The symbolic timetable type, such as ONE_DAY, THREE_DAY, or SEVEN_DAY.
    /// </summary>
    public string? Type { get; set; }
}