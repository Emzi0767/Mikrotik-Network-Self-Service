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
using System.Text;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents a route, as configured in a DHCP lease.
/// </summary>
public readonly struct MikrotikDhcpRoute : IParsable<MikrotikDhcpRoute>, ISpanParsable<MikrotikDhcpRoute>
{
    /// <summary>
    /// Gets the destination subnet the route is for.
    /// </summary>
    public Optional<IPSubnet> DestinationSubnet { get; }

    /// <summary>
    /// Gets the gateway address for the route.
    /// </summary>
    public Optional<IPAddress> GatewayAddress { get; }

    /// <summary>
    /// Gets the distance for this route.
    /// </summary>
    public Optional<int> Distance { get; }

    /// <summary>
    /// Creates a new route, as configured in DHCP.
    /// </summary>
    /// <param name="destinationSubnet">Destination subnet the route is for.</param>
    /// <param name="gatewayAddress">Address of the gateway for the route.</param>
    /// <param name="distance">Distance (cost) for this route.</param>
    public MikrotikDhcpRoute(
        Optional<IPSubnet> destinationSubnet,
        Optional<IPAddress> gatewayAddress,
        Optional<int> distance
    )
    {
        this.DestinationSubnet = destinationSubnet;
        this.GatewayAddress = gatewayAddress;
        this.Distance = distance;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (this.DestinationSubnet.HasValue)
            sb.Append(this.DestinationSubnet.Value);

        if (this.GatewayAddress.HasValue)
        {
            if (this.DestinationSubnet.HasValue)
                sb.Append(' ');

            sb.Append(this.GatewayAddress.Value);
        }

        if (this.Distance.HasValue)
        {
            if (this.DestinationSubnet.HasValue || this.GatewayAddress.HasValue)
                sb.Append(' ');

            sb.Append(this.Distance.Value);
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public static MikrotikDhcpRoute Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid route specification.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out MikrotikDhcpRoute result)
        => TryParse(s.AsSpan(), provider, out result);

    /// <inheritdoc />
    public static MikrotikDhcpRoute Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid route specification.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out MikrotikDhcpRoute result)
    {
        var subnet = Optional<IPSubnet>.Default;
        var gateway = Optional<IPAddress>.Default;
        var distance = Optional<int>.Default;
        result = default;
        if (IPSubnet.TryParse(s, provider, out var subnetValue))
        {
            subnet = subnetValue;
            if (!_advance(ref s))
            {
                result = new(subnet, gateway, distance);
                return true;
            }
        }

        if (IPAddress.TryParse(s, out var gatewayValue))
        {
            gateway = gatewayValue;
            if (!_advance(ref s))
            {
                result = new(subnet, gateway, distance);
                return true;
            }
        }

        if (int.TryParse(s, out var distanceValue))
            distance = distanceValue;

        result = new MikrotikDhcpRoute(subnet, gateway, distance);
        return true;

        static bool _advance(ref ReadOnlySpan<char> _s)
        {
            var spIdx = _s.IndexOf(' ');
            if (spIdx == -1)
                return false;

            _s = _s[(spIdx + 1)..];
            return true;
        }
    }
}
