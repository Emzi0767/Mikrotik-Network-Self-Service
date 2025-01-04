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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API reply word.
/// </summary>
public sealed class MikrotikReplyWord : IMikrotikWord
{
    /// <summary>
    /// Gets the prefix for all replies.
    /// </summary>
    public const string Prefix = "!";

    private static readonly IReadOnlyDictionary<MikrotikReplyWordType, string> _codebook;
    private static readonly IReadOnlyDictionary<string, MikrotikReplyWordType> _decodebook;

    /// <summary>
    /// Gets the type of this reply.
    /// </summary>
    public MikrotikReplyWordType Type { get; }
    
    /// <summary>
    /// Gets whether this sentence is the final one in a response.
    /// </summary>
    public bool IsFinal
        => this.Type is MikrotikReplyWordType.Completed or MikrotikReplyWordType.Error;

    /// <inheritdoc />
    public int Length { get; }

    static MikrotikReplyWord()
    {
        _codebook = typeof(MikrotikReplyWordType)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(x => new { v = (MikrotikReplyWordType)x.GetValue(null), a = x.GetCustomAttribute<EnumMemberAttribute>() })
            .Where(x => x.a is not null)
            .ToDictionary(x => x.v, x => x.a.Value);

        _decodebook = _codebook.ToDictionary(x => x.Value, x => x.Key, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a new Mikrotik API reply word.
    /// </summary>
    /// <param name="type">Type of the reply.</param>
    public MikrotikReplyWord(MikrotikReplyWordType type)
    {
        this.Type = type;
        this.Length = ComputeLength(this.Type);
    }

    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
        => throw new NotSupportedException();

    /// <summary>
    /// Attempts to determine the reply type from its encoded form.
    /// </summary>
    /// <param name="encoded">Encoded form of the reply type.</param>
    /// <returns>The determined reply type.</returns>
    public static MikrotikReplyWordType GetReplyType(string encoded)
        => _decodebook.TryGetValue(encoded, out var type)
            ? type
            : MikrotikReplyWordType.Unknown;

    private static int ComputeLength(MikrotikReplyWordType type)
    {
        var prefixLen = Prefix.ComputeEncodedLength();
        if (_codebook.TryGetValue(type, out var encoded))
            return prefixLen + encoded.ComputeEncodedLength();

        MikrotikThrowHelper.Throw_OutOfRange(nameof(type), "Specified reply type is invalid.");
        return -1;
    }
}
