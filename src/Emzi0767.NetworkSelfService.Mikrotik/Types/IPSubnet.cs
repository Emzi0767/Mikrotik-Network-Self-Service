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
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Primitives;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents information about an IP subnet, based on CIDR notation.
/// </summary>
public readonly struct IPSubnet : IParsable<IPSubnet>, IEquatable<IPSubnet>
{
    /// <summary>
    /// Gets the default (zero) CIDR subnet.
    /// </summary>
    public static IPSubnet Default { get; } = new IPSubnet(new IPAddress(0), 0);

    /// <summary>
    /// Gets the address of the subnet.
    /// </summary>
    public IPAddress Address { get; }

    /// <summary>
    /// Gets the CIDR notation for the mask of the subnet.
    /// </summary>
    public int CidrMask { get; }

    /// <summary>
    /// Gets the IP notation for the address mask.
    /// </summary>
    public IPAddress Mask { get; }

    /// <summary>
    /// Gets the broadcast address for the network.
    /// </summary>
    public IPAddress Broadcast { get; }

    /// <summary>
    /// Creates new IP subnet information.
    /// </summary>
    /// <param name="address">Address of the subnet.</param>
    /// <param name="mask">Numeric mask of the subnet.</param>
    public IPSubnet(IPAddress address, int mask)
    {
        var mask32 = ComputeMask32(mask);

        this.Address = GetNetworkBaseAddress(address, mask32);
        this.CidrMask = mask;
        this.Mask = ToAddress(mask32);
        this.Broadcast = GetNetworkBroadcastAddress(address, mask32);
    }

    /// <inheritdoc />
    public bool Equals(IPSubnet other)
        => this.Address is not null
        && this.Address.Equals(other.Address)
        && this.CidrMask == other.CidrMask;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is IPSubnet other
        && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(this.Address, this.CidrMask);

    /// <inheritdoc />
    public static IPSubnet Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Malformed CIDR string.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out IPSubnet result)
    {
        result = default;
        if (s is null)
            return false;

        var ss = new StringSegment(s);
        var (hasAddress, hasMask) = (false, false);
        var address = default(IPAddress);
        var mask = default(int);
        foreach (var segment in ss.Split([ '/' ]))
        {
            if (hasAddress && hasMask)
                return false;

            if (!hasAddress)
            {
                if (!(hasAddress = IPAddress.TryParse(segment, out address)))
                    return false;
            }
            else if (hasAddress && !(hasMask = int.TryParse(segment, provider, out mask)))
                return false;
        }

        result = new(address, mask);
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{this.Address}/{this.CidrMask}";

    private static uint ComputeMask32(int mask)
        => ~((1u << (32 - mask)) - 1u) & uint.MaxValue;

    private static IPAddress ToAddress(uint addr)
        => new IPAddress(BinaryPrimitives.ReverseEndianness(addr));

    private static uint ToAddress32(IPAddress addr)
        => BinaryPrimitives.ReverseEndianness((uint)addr.Address);

    private static IPAddress GetNetworkBaseAddress(IPAddress address, uint mask32)
    {
        if (address.AddressFamily != AddressFamily.InterNetwork)
            MikrotikThrowHelper.Throw_NotSupported("Only IPv4 addresses are supported.");

        var addr32 = ToAddress32(address);
        addr32 &= mask32;

        var ip = ToAddress(addr32);
        return ip;
    }

    private static IPAddress GetNetworkBroadcastAddress(IPAddress addr, uint mask32)
    {
        var addr32 = ToAddress32(addr);
        addr32 &= mask32;
        var bcast32 = uint.MaxValue & ~mask32;
        addr32 |= bcast32;
        return ToAddress(addr32);
    }

    public static bool operator ==(IPSubnet left, IPSubnet right)
        => left.Equals(right);

    public static bool operator !=(IPSubnet left, IPSubnet right)
        => !left.Equals(right);
}
