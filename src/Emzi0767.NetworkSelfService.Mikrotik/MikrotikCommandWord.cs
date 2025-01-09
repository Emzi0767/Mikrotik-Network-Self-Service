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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API command word.
/// </summary>
[DebuggerDisplay("CMD: '{Parts}'")]
public sealed class MikrotikCommandWord : IMikrotikWord
{
    /// <summary>
    /// Gets the separator for command parts.
    /// </summary>
    public const string PartSeparator = "/";

    /// <summary>
    /// Gets the part of this command.
    /// </summary>
    public IEnumerable<string> Parts { get; }

    /// <inheritdoc />
    public int Length { get; }

    /// <summary>
    /// Creates a new Mikrotik API command word.
    /// </summary>
    /// <param name="parts">Command parts to encode.</param>
    public MikrotikCommandWord(IEnumerable<string> parts)
    {
        if (parts is null)
        {
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(parts), "The parts collection cannot be null.");
            return;
        }

        if (!parts.Any())
        {
            MikrotikThrowHelper.Throw_Argument(nameof(parts), "The parts collection must contain at least one item.");
            return;
        }

        this.Parts = parts.ToImmutableArray();
        this.Length = ComputeLength(this.Parts);
    }

    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
    {
        foreach (var part in this.Parts)
        {
            if (!PartSeparator.TryEncodeTo(destination))
                return false;

            if (!part.TryEncodeTo(destination))
                return false;
        }

        return true;
    }

    private static int ComputeLength(IEnumerable<string> parts)
    {
        var sepLen = PartSeparator.ComputeEncodedLength();
        return parts.Select(x => x.ComputeEncodedLength() + sepLen).Sum();
    }
}
