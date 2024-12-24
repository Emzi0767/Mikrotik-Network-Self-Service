using System.Runtime.Serialization;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Specifies the type of Mikrotik reply.
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
    [EnumMember(Value = "done")]
    Completed,

    /// <summary>
    /// Specifies a response indicating that a command failed.
    /// </summary>
    [EnumMember(Value = "trap")]
    Error,

    /// <summary>
    /// Specifies a partial response which contains data in response to a query.
    /// </summary>
    [EnumMember(Value = "re")]
    Data,

    /// <summary>
    /// Specifies an indication that a connection is being terminated.
    /// </summary>
    [EnumMember(Value = "fatal")]
    ConnectionTermination,
}
