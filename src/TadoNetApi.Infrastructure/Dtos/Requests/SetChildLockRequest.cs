namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Represents a request to set the child lock state of a device.
/// </summary>
public class SetChildLockRequest
{
    /// <summary>True to enable child lock, false to disable.</summary>
    public bool ChildLock { get; set; }
}