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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Represents a MAC address.
/// </summary>
public readonly partial struct MacAddress : IEquatable<MacAddress>, IParsable<MacAddress>, ISpanParsable<MacAddress>
{
    private static Regex MacRegex { get; } = MacRegexGen();

    private readonly byte[] _bytes;

    /// <summary>
    /// Creates a new MAC address from given byte array.
    /// </summary>
    /// <param name="bytes">Bytes comprising the MAC address. Must be exactly 6 items long.</param>
    public MacAddress(byte[] bytes)
    {
        if (bytes is null)
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(bytes), "Bytes cannot be null.");

        if (bytes.Length != 6)
            MikrotikThrowHelper.Throw_Argument(nameof(bytes), "MAC address must contain 6 bytes.");

        this._bytes = bytes;
    }

    /// <inheritdoc />
    public override string ToString()
        => string.Join(":", this._bytes.Select(x => x.ToString("X2", CultureInfo.InvariantCulture)));

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is MacAddress other
           && this == other;

    /// <inheritdoc />
    public bool Equals(MacAddress other)
        => this == other;

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(
            this._bytes[0],
            this._bytes[1],
            this._bytes[2],
            this._bytes[3],
            this._bytes[4],
            this._bytes[5]
        );

    /// <summary>
    /// Parses a MAC address from a <see cref="string"/> instance.
    /// </summary>
    /// <param name="input">String containing the address to parse.</param>
    /// <param name="formatProvider">Format provider for the parsing.</param>
    /// <returns>Parsed MAC address.</returns>
    public static MacAddress Parse(string input, IFormatProvider formatProvider)
    {
        if (TryParse(input, formatProvider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(input), "Invalid MAC address supplied.");
        return default;
    }

    /// <summary>
    /// Attempts to parse a MAC address from a <see cref="string"/> instance.
    /// </summary>
    /// <param name="input">String containing the address to parse.</param>
    /// <param name="formatProvider">Format provider for the parsing.</param>
    /// <param name="result">Parsed MAC address.</param>
    /// <returns>Whether the operation was a success.</returns>
    public static bool TryParse(string input, IFormatProvider formatProvider, out MacAddress result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var m = MacRegex.Match(input);
        if (!m.Success)
            return false;

        var sep = m.Groups["sep"].Success
            ? m.Groups["sep"].Value.ToCharArray()
            : null;

        var segments = sep is not null
            ? new StringSegment(m.Value).Split(sep)
            : Enumerable.Range(0, 6).Select(x => new StringSegment(m.Value, x * 2, 2));

        var bytes = segments.Select(x => byte.Parse(x, NumberStyles.HexNumber, formatProvider))
            .ToArray();

        result = new(bytes);
        return true;
    }

    /// <summary>
    /// Parses a MAC address from a <see cref="string"/> instance.
    /// </summary>
    /// <param name="s">String containing the address to parse.</param>
    /// <param name="provider">Format provider for the parsing.</param>
    /// <returns>Parsed MAC address.</returns>
    public static MacAddress Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        MikrotikThrowHelper.Throw_Argument(nameof(s), "Invalid MAC address supplied.");
        return default;
    }

    /// <summary>
    /// Attempts to parse a MAC address from a <see cref="string"/> instance.
    /// </summary>
    /// <param name="s">String containing the address to parse.</param>
    /// <param name="provider">Format provider for the parsing.</param>
    /// <param name="result">Parsed MAC address.</param>
    /// <returns>Whether the operation was a success.</returns>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out MacAddress result)
        => TryParse(new string(s), provider, out result);

    public static bool operator ==(MacAddress a, MacAddress b)
        => a._bytes.SequenceEqual(b._bytes);

    public static bool operator !=(MacAddress a, MacAddress b)
        => !a._bytes.SequenceEqual(b._bytes);

    [GeneratedRegex(@"^[0-9a-fA-F]{2}(?<sep>[:\-])?(?(sep)(?:[0-9a-fA-F]{2}\k<sep>){4}|(?:[0-9a-fA-F]{2}){4})[0-9a-fA-F]{2}$", RegexOptions.Compiled)]
    private static partial Regex MacRegexGen();
}
