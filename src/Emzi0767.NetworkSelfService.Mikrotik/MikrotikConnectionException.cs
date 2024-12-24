using System;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Indicates that a problem occured when connecting to a router.
/// </summary>
public sealed class MikrotikConnectionException : Exception
{
    /// <summary>
    /// Creates a new connection exception with specified message.
    /// </summary>
    /// <param name="message">Message describing the problem.</param>
    public MikrotikConnectionException(string message)
        : base(message)
    { }
}
