namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Specifies the type of a Mikrotik reply.
/// </summary>
public enum MikrotikReplyWordType
{
    /// <summary>
    /// Specifies an unknown reply word type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Specifies a response indicating that a command has been executed successfully.
    /// </summary>
    [MikrotikReplyText("done")]
    Completed,

    /// <summary>
    /// Specifies a response indicating that a command failed.
    /// </summary>
    [MikrotikReplyText("trap")]
    Error,

    /// <summary>
    /// Specifies a partial response which contains data in response to a query.
    /// </summary>
    [MikrotikReplyText("re")]
    Data,

    /// <summary>
    /// Specifies an indication that a connection is being terminated.
    /// </summary>
    [MikrotikReplyText("fatal")]
    ConnectionTermination,
}
