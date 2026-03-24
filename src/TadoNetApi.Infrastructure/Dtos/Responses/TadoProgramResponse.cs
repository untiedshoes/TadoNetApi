namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the currently active program for a zone returned by the Tado API.
/// </summary>
public class TadoProgramResponse
{
    public string Name { get; set; } = string.Empty;
}