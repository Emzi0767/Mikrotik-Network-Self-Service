using System;
using Emzi0767.Utilities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents arguments for <see cref="MikrotikClient.Exception"/> event.
/// </summary>
public sealed class MikrotikExceptionEventArgs : EventArgs
{
    /// <summary>
    /// Gets the exception that occured.
    /// </summary>
    public Exception Exception { get; init; }
    
    /// <summary>
    /// Gets the event in which the problem occured.
    /// </summary>
    public AsyncEvent Event { get; init; }
    
    /// <summary>
    /// Gets the arguments passed to the event.
    /// </summary>
    public AsyncEventArgs EventArgs { get; init; }
    
    /// <summary>
    /// Gets the handler in which the exception occured.
    /// </summary>
    public AsyncEventHandler<MikrotikClient, AsyncEventArgs> Handler { get; init; }
}
