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
using Emzi0767.NetworkSelfService.Mikrotik.Expressions;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents a time range.
/// </summary>
public readonly struct MikrotikTimeRange : IParsable<MikrotikTimeRange>, ISpanParsable<MikrotikTimeRange>
{
    /// <summary>
    /// Gets the starting time of this range.
    /// </summary>
    public TimeSpan Start { get; }

    /// <summary>
    /// Gets the ending time of the range.
    /// </summary>
    public TimeSpan End { get; }

    /// <summary>
    /// Gets the weekdays this range is specified for.
    /// </summary>
    public MikrotikWeekday Weekdays { get; }

    /// <summary>
    /// Creates a new time range.
    /// </summary>
    /// <param name="start">Starting time the range is specified for.</param>
    /// <param name="end">Ending time the range is specified for.</param>
    /// <param name="weekdays">Weekdays the range is specified for.</param>
    public MikrotikTimeRange(TimeSpan start, TimeSpan end, MikrotikWeekday weekdays)
    {
        this.Start = start;
        this.End = end;
        this.Weekdays = weekdays;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var weekdays = this.Weekdays;
        var flags = Enum.GetValues<MikrotikWeekday>()
            .Where(x => x != MikrotikWeekday.None && weekdays.HasFlag(x))
            .Select(x => x.MapToSerialized());

        var allParams = Enumerable.Empty<string>()
            .Append(string.Join("-", [ this.Start.ToMikrotikString(), this.End.ToMikrotikString() ]))
            .Concat(flags);

        return string.Join(",", allParams);
    }

    /// <inheritdoc />
    public static MikrotikTimeRange Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid range supplied.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out MikrotikTimeRange result)
        => TryParse(s.AsSpan(), provider, out result);

    /// <inheritdoc />
    public static MikrotikTimeRange Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid range supplied.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out MikrotikTimeRange result)
    {
        var weekdays = MikrotikWeekday.None;
        result = default;
        var index = s.IndexOf(',');

        var times = index < 0 ? s : s[..index];
        s = s[(index + 1)..];
        var splitter = times.IndexOf('-');
        var sStart = times[..splitter];
        var sEnd = times[(splitter + 1)..];
        var oStart = MikrotikValueParser.Parse(typeof(TimeSpan), sStart, out var parseResult);
        if (parseResult != MikrotikValueParser.Result.Success)
            return false;

        var start = (TimeSpan)oStart;
        var oEnd = MikrotikValueParser.Parse(typeof(TimeSpan), sEnd, out parseResult);
        if (parseResult != MikrotikValueParser.Result.Success)
            return false;

        var end = (TimeSpan)oEnd;
        do
        {
            index = s.IndexOf(',');
            var sDay = index < 0 ? s : s[..index];
            if (index >= 0)
                s = s[(index + 1)..];

            weekdays |= EnumProxies.MapFromSerialized<MikrotikWeekday>(new string(sDay));
        }
        while (index >= 0);

        result = new(start, end, weekdays);
        return true;
    }
}
