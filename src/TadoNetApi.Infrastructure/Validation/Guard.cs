namespace TadoNetApi.Infrastructure.Validation;

/// <summary>
/// Provides shared argument validation helpers for infrastructure services.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Ensures the provided identifier is a positive integer.
    /// </summary>
    public static void PositiveId(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, "Identifier must be a positive integer.");
    }
}