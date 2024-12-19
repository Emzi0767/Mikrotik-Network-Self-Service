using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API command word.
/// </summary>
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
            MikrotikThrowHelper.Throw_ArgumentException(nameof(parts), "The parts collection must contain at least one item.");
            return;
        }

        this.Parts = ImmutableArray.ToImmutableArray(parts);
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
