// This file is part of Network Self-Service Project.
// Copyright © 2024-2025 Mateusz Brawański <Emzi0767>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Emzi0767.NetworkSelfService.Mikrotik.Exceptions;

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

    [DoesNotReturn]
    public static void Throw_RequestTerminatedEarly()
        => throw new OperationCanceledException();

    [DoesNotReturn]
    public static void Throw_SyncNotSupported()
        => Throw_NotSupported("Synchronous operations are not supported.");

    [DoesNotReturn]
    public static void Throw_NotSupported(string msg)
        => throw new NotSupportedException(msg);

    [DoesNotReturn]
    public static void Throw_InvalidEntityType(Type type)
        => throw new MikrotikEntityTypeException(type);

    [DoesNotReturn]
    public static void Throw_InvalidEntityType(Type type, string msg)
        => throw new MikrotikEntityTypeException(type, msg);
}
