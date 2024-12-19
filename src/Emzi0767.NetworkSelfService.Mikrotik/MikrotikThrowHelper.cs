using System;

namespace Emzi0767.NetworkSelfService.Mikrotik;

internal static class MikrotikThrowHelper
{
    public static void Throw_ArgumentNull(string arg, string msg)
        => throw new ArgumentNullException(arg, msg);

    public static void Throw_ArgumentException(string arg, string msg)
        => throw new ArgumentException(msg, arg);

    public static void Throw_OutOfRangeException(string arg, string msg)
        => throw new ArgumentOutOfRangeException(arg, msg);
}
