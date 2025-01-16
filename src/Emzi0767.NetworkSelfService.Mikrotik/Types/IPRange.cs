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
/// Represents a range of IP addresses.
/// </summary>
public readonly struct IPRange : IParsable<IPRange>, ISpanParsable<IPRange>, IEquatable<IPRange>
{
    /// <summary>
    /// Gets the starting address of this range.
    /// </summary>
    public IPAddress Start { get; }

    /// <summary>
    /// Gets the end address of this range.
    /// </summary>
    public IPAddress End { get; }

    /// <summary>
    /// Creates a new IP range.
    /// </summary>
    /// <param name="start">Starting address of the range.</param>
    /// <param name="end">Ending address of the range.</param>
    public IPRange(IPAddress start, IPAddress end)
    {
        this.Start = start;
        this.End = end;
    }

    /// <summary>
    /// Checks whether a given address is contained in this range.
    /// </summary>
    /// <param name="address">Address to check.</param>
    /// <returns>Whether this range contains the address.</returns>
    public bool Contains(IPAddress address)
        => address.ToNumeric() >= this.Start.ToNumeric()
        && address.ToNumeric() <= this.End.ToNumeric();

    /// <summary>
    /// Checks whether this range contains the given subnet.
    /// </summary>
    /// <param name="subnet">Subnet to check.</param>
    /// <returns>Whether this range contains the subnet.</returns>
    public bool Contains(IPSubnet subnet)
        => subnet.Address.ToNumeric() >= this.Start.ToNumeric()
        && subnet.Broadcast.ToNumeric() <= this.End.ToNumeric();

    /// <summary>
    /// Checks whether this range contains the given address.
    /// </summary>
    /// <param name="addressWithSubnet">Address to check.</param>
    /// <returns>Whether this range contains the address.</returns>
    public bool Contains(IPAddressWithSubnet addressWithSubnet)
        => this.Contains(addressWithSubnet.Address)
        && this.Contains(addressWithSubnet.AsSubnet());

    /// <summary>
    /// Checks whether this range contains another range.
    /// </summary>
    /// <param name="range">Range to check.</param>
    /// <returns>Whether this range contains the range.</returns>
    public bool Contains(IPRange range)
        => range.Start.ToNumeric() >= this.Start.ToNumeric()
        && range.End.ToNumeric() <= this.End.ToNumeric();

    /// <inheritdoc />
    public bool Equals(IPRange other)
        => this.Start.Equals(other.Start)
        && this.End.Equals(other.End);

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is IPRange other
        && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(this.Start, this.End);

    /// <inheritdoc />
    public override string ToString()
        => $"{this.Start}-{this.End}";

    /// <inheritdoc />
    public static IPRange Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Malformed CIDR string.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out IPRange result)
        => TryParse(s.AsSpan(), provider, out result);

    /// <inheritdoc />
    public static IPRange Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Malformed CIDR string.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out IPRange result)
    {
        result = default;
        if (IPSubnet.TryParse(s, provider, out var subnet))
        {
            result = subnet;
            return true;
        }

        var sepIdx = s.IndexOf('-');
        var sStart = s[..sepIdx];
        var sEnd = s[(sepIdx + 1)..];
        if (!IPAddress.TryParse(sStart, out var start) || !IPAddress.TryParse(sEnd, out var end))
            return false;

        result = new IPRange(start, end);
        return true;
    }
}
