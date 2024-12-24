using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Emzi0767.NetworkSelfService.Mikrotik;

internal static class MikrotikThrowHelper
{
    [DoesNotReturn]
    public static void Throw_ArgumentNull(string arg, string msg)
        => throw new ArgumentNullException(arg, msg);

    [DoesNotReturn]
    public static void Throw_Argument(string arg, string msg)
        => throw new ArgumentException(msg, arg);

    [DoesNotReturn]
    public static void Throw_OutOfRange(string arg, string msg)
        => throw new ArgumentOutOfRangeException(arg, msg);

    [DoesNotReturn]
    public static void Throw_InvalidOperation(string msg)
        => throw new InvalidOperationException(msg);

    [DoesNotReturn]
    public static void Throw_InvalidData(string msg)
        => throw new InvalidDataException(msg);
    
    [DoesNotReturn]
    public static void Throw_Connection(string msg)
        => throw new MikrotikConnectionException(msg);
}
