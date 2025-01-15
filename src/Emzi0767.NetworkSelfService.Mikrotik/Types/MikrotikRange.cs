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
using System.Globalization;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents a range of values.
/// </summary>
public readonly struct MikrotikRange : IEquatable<MikrotikRange>, IParsable<MikrotikRange>, ISpanParsable<MikrotikRange>
{
    /// <summary>
    /// Gets the start of the range.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the end of the range.
    /// </summary>
    public int End { get; }

    /// <summary>
    /// Creates a new range with given start and end.
    /// </summary>
    /// <param name="start">Start of the range.</param>
    /// <param name="end">End of the range.</param>
    public MikrotikRange(int start, int end)
    {
        this.Start = start;
        this.End = end;
    }

    /// <inheritdoc />
    public bool Equals(MikrotikRange other)
        => this.Start == other.Start
        && this.End == other.End;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is MikrotikRange other
        && this.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(this.Start, this.End);

    /// <inheritdoc />
    public override string ToString()
        => $"{this.Start}..{this.End}";

    /// <inheritdoc />
    public static MikrotikRange Parse(string s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid range supplied.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(string s, IFormatProvider provider, out MikrotikRange result)
        => TryParse(s.AsSpan(), provider, out result);

    /// <inheritdoc />
    public static MikrotikRange Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid range supplied.");
        return default;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out MikrotikRange result)
    {
        result = default;
        var sepIdx = s.IndexOf('.');
        if (!int.TryParse(s[..sepIdx], NumberStyles.Integer, provider, out var start))
            return false;

        s = s[sepIdx..];
        if (s[..2] is not "..")
            return false;

        s = s[2..];
        if (!int.TryParse(s, NumberStyles.Integer, provider, out var end))
            return false;

        result = new(start, end);
        return true;
    }
}
