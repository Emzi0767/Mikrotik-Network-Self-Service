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
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents Mikrotik rate limit information.
/// </summary>
public readonly struct MikrotikDhcpRateLimit : IParsable<MikrotikDhcpRateLimit>
{
    /// <summary>
    /// Gets the regular receive rate, in bytes per second.
    /// </summary>
    public long ReceiveRate { get; }

    /// <summary>
    /// Gets the regular transmit rate, in bytes per second.
    /// </summary>
    public Optional<long> TransmitRate { get; }

    /// <summary>
    /// Gets the burst receive rate, in bytes per second.
    /// </summary>
    public Optional<long> ReceiveBurstRate { get; }

    /// <summary>
    /// Gets the burst transmit rate, in bytes per second.
    /// </summary>
    public Optional<long> TransmitBurstRate { get; }

    /// <summary>
    /// Gets the burst receive threshold, in bytes per second.
    /// </summary>
    public Optional<long> ReceiveBurstThreshold { get; }

    /// <summary>
    /// Gets the burst transmit threshold, in bytes per second.
    /// </summary>
    public Optional<long> TransmitBurstThreshold { get; }

    /// <summary>
    /// Gets the time over which average receive data rate is calculated.
    /// </summary>
    public Optional<TimeSpan> ReceiveBurstTime { get; }

    /// <summary>
    /// Gets the time over which average transmit data rate is calculated.
    /// </summary>
    public Optional<TimeSpan> TransmitBurstTime { get; }

    /// <summary>
    /// Creates new DHCP rate limit info.
    /// </summary>
    /// <param name="receiveRate">Nominal allowed receive rate.</param>
    /// <param name="transmitRate">Nominal allowed transmit rate. If not specified, <paramref name="receiveRate"/> is used.</param>
    /// <param name="receiveBurstRate">Allowed burst receive rate.</param>
    /// <param name="transmitBurstRate">Allowed burst transmit rate. If not specified, <param name="receiveBurstRate"/> is used.</param>
    /// <param name="receiveBurstThreshold">Bandwidth threshold which determines if receive burst is available.</param>
    /// <param name="transmitBurstThreshold">Bandwidth threshold which determines if transmit burst is available. If not specified, <paramref name="transmitBurstThreshold"/> is used.</param>
    /// <param name="receiveBurstTime">Time over which average receive rate is calculated.</param>
    /// <param name="transmitBurstTime">Time over which average transmit rate is calculated. If not specified, <paramref name="receiveBurstTime"/> is used.</param>
    public MikrotikDhcpRateLimit(
        long receiveRate,
        Optional<long> transmitRate = default,
        Optional<long> receiveBurstRate = default,
        Optional<long> transmitBurstRate = default,
        Optional<long> receiveBurstThreshold = default,
        Optional<long> transmitBurstThreshold = default,
        Optional<TimeSpan> receiveBurstTime = default,
        Optional<TimeSpan> transmitBurstTime = default
    )
    {
        if (transmitBurstRate.HasValue && !receiveBurstRate.HasValue)
            MikrotikThrowHelper.Throw_Argument(nameof(transmitBurstRate), "Receive burst rate must be specified if transmit burst rate is specified.");

        if (transmitBurstThreshold.HasValue && !receiveBurstThreshold.HasValue)
            MikrotikThrowHelper.Throw_Argument(nameof(transmitBurstThreshold), "Receive burst threshold must be specified if transmit burst threshold is specified.");

        if (transmitBurstTime.HasValue && !receiveBurstTime.HasValue)
            MikrotikThrowHelper.Throw_Argument(nameof(transmitBurstTime), "Receive burst time must be specified if transmit burst time is specified.");

        if (receiveBurstTime.HasValue && !receiveBurstThreshold.HasValue)
            MikrotikThrowHelper.Throw_Argument(nameof(receiveBurstTime), "Receive burst threshold must be specified if receive burst time is specified.");

        if (receiveBurstThreshold.HasValue && !receiveBurstRate.HasValue)
            MikrotikThrowHelper.Throw_Argument(nameof(receiveBurstThreshold), "Receive burst rate must be specified if receive burst threshold is specified.");

        this.ReceiveRate = receiveRate;
        this.TransmitRate = transmitRate;
        this.ReceiveBurstRate = receiveBurstRate;
        this.TransmitBurstRate = transmitBurstRate;
        this.ReceiveBurstThreshold = receiveBurstThreshold;
        this.TransmitBurstThreshold = transmitBurstThreshold;
        this.ReceiveBurstTime = receiveBurstTime;
        this.TransmitBurstTime = transmitBurstTime;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.ReceiveRate);

        if (this.TransmitRate.HasValue)
            sb.Append('/').Append(this.TransmitRate.Value);

        if (!this.ReceiveBurstRate.HasValue)
            return sb.ToString();

        sb.Append(' ').Append(this.ReceiveBurstRate.Value);
        if (this.TransmitBurstRate.HasValue)
            sb.Append('/').Append(this.TransmitBurstRate.Value);

        if (!this.ReceiveBurstThreshold.HasValue)
            return sb.ToString();

        sb.Append(' ').Append(this.ReceiveBurstThreshold.Value);
        if (this.TransmitBurstThreshold.HasValue)
            sb.Append('/').Append(this.TransmitBurstThreshold.Value);

        if (!this.ReceiveBurstTime.HasValue)
            return sb.ToString();

        sb.Append(' ').Append(Math.Ceiling(this.ReceiveBurstTime.Value.TotalSeconds));
        if (this.TransmitBurstTime.HasValue)
            sb.Append('/').Append(Math.Ceiling(this.TransmitBurstTime.Value.TotalSeconds));

        return sb.ToString();
    }

    /// <inheritdoc />
    public static MikrotikDhcpRateLimit Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Malformed rate limit.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out MikrotikDhcpRateLimit result)
    {
        result = default;
        if (s is null)
            return false;

        var ss = new StringSegment(s);
        var rateRx = 0L;
        var rateTx = Optional<long>.Default;
        var rateBurstRx = Optional<long>.Default;
        var rateBurstTx = Optional<long>.Default;
        var thresholdRx = Optional<long>.Default;
        var thresholdTx = Optional<long>.Default;
        var timeRx = Optional<TimeSpan>.Default;
        var timeTx = Optional<TimeSpan>.Default;

        foreach (var (x, i) in ss.Split([' ']).Select(static (x, i) => (x, i)))
        {
            if (i > 3)
                return false;

            var (hasRx, hasTx) = (false, false);
            foreach (var y in x.Split(['/']))
            {
                if (hasRx && hasTx)
                    return false;

                if (!long.TryParse(y, provider, out var parsed))
                    return false;

                switch ((i, hasRx))
                {
                    case (0, false):
                        rateRx = parsed;
                        hasRx = true;
                        break;

                    case (0, true):
                        rateTx = parsed;
                        hasTx = true;
                        break;

                    case (1, false):
                        rateBurstRx = parsed;
                        hasRx = true;
                        break;

                    case (1, true):
                        rateBurstTx = parsed;
                        hasTx = true;
                        break;

                    case (2, false):
                        thresholdRx = parsed;
                        hasRx = true;
                        break;

                    case (2, true):
                        thresholdTx = parsed;
                        hasTx = true;
                        break;

                    case (3, false):
                        timeRx = TimeSpan.FromSeconds(parsed);
                        hasRx = true;
                        break;

                    case (3, true):
                        timeTx = TimeSpan.FromSeconds(parsed);
                        hasTx = true;
                        break;

                    default: return false;
                }
            }
        }

        try
        {
            result = new(
                rateRx,
                rateTx,
                rateBurstRx,
                rateBurstTx,
                thresholdRx,
                thresholdTx,
                timeRx,
                timeTx
            );
            return true;
        }
        catch { return false; }
    }
}
