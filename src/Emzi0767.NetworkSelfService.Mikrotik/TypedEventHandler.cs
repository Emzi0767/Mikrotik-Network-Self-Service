using System;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a typed event handler.
/// </summary>
/// <typeparam name="TSender">Type of the sender.</typeparam>
/// <typeparam name="TArgs">Type of event arguments.</typeparam>
public delegate void TypedEventHandler<TSender, TArgs>(TSender sender, TArgs e)
    where TArgs : EventArgs;
