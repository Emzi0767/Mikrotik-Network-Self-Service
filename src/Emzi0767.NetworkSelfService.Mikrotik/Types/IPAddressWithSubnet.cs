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
using System.Net;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents information about an IP address with a mask, in CIDR notation.
/// </summary>
public readonly struct IPAddressWithSubnet : IParsable<IPAddressWithSubnet>, ISpanParsable<IPAddressWithSubnet>, IEquatable<IPAddressWithSubnet>
{
    /// <summary>
    /// Gets the default (zero) CIDR address.
    /// </summary>
    public static IPAddressWithSubnet Default { get; } = new IPAddressWithSubnet(new IPAddress(0), 0);

    /// <summary>
    /// Gets the address of the subnet.
    /// </summary>
    public IPAddress Address { get; }

    /// <summary>
    /// Gets the CIDR notation for the mask of the subnet.
    /// </summary>
    public int CidrMask { get; }

    /// <summary>
    /// Creates a new address with a subnet mask.
    /// </summary>
    /// <param name="address">IP address.</param>
    /// <param name="cidrMask">Network mask for the address.</param>
    public IPAddressWithSubnet(IPAddress address, int cidrMask)
    {
        this.Address = address;
        this.CidrMask = cidrMask;
    }

    /// <summary>
    /// Gets the network information from this address.
    /// </summary>
    /// <returns>Subnet information.</returns>
    public IPSubnet AsSubnet()
        => this;

    /// <inheritdoc />
    public bool Equals(IPAddressWithSubnet other)
        => this.Address.Equals(other.Address)
        && this.CidrMask == other.CidrMask;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is IPAddressWithSubnet other
        && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(this.Address, this.CidrMask);

    /// <inheritdoc />
    public static IPAddressWithSubnet Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Malformed CIDR string.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out IPAddressWithSubnet result)
        => TryParse(s.AsSpan(), provider, out result);

    /// <inheritdoc />
    public static IPAddressWithSubnet Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Malformed CIDR string.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out IPAddressWithSubnet result)
    {
        result = default;
        var index = s.IndexOf('/');
        if (index == -1)
            return false;

        var ipSegment = s[..index];
        var maskSegment = s[(index + 1)..];
        if (!IPAddress.TryParse(ipSegment, out var address))
            return false;

        if (!int.TryParse(maskSegment, out var mask))
            return false;

        result = new(address, mask);
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
        => $"{this.Address}/{this.CidrMask}";

    public static bool operator ==(IPAddressWithSubnet left, IPAddressWithSubnet right)
        => left.Equals(right);

    public static bool operator !=(IPAddressWithSubnet left, IPAddressWithSubnet right)
        => !left.Equals(right);

    public static implicit operator IPSubnet(IPAddressWithSubnet value)
        => new IPSubnet(value.Address, value.CidrMask);
}
