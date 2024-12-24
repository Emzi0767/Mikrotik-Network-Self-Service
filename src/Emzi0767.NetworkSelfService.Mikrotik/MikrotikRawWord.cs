using System;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a raw Mikrotik API word.
/// </summary>
public sealed class MikrotikRawWord : IMikrotikWord
{
    /// <summary>
    /// Gets the value of this word.
    /// </summary>
    public string Value { get; }
    
    /// <inheritdoc />
    public int Length { get; }

    /// <summary>
    /// Creates a new raw word.
    /// </summary>
    /// <param name="value">Value of this word.</param>
    public MikrotikRawWord(string value)
    {
        this.Value = value;
        this.Length = value.ComputeEncodedLength();
    }
    
    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
        => throw new NotImplementedException();
}