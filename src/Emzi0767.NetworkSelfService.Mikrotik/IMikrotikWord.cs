using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API word.
/// </summary>
public interface IMikrotikWord
{
    /// <summary>
    /// Gets the total encoded length of this word.
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Encodes the given word to the destination buffer.
    /// </summary>
    /// <param name="destination">Destination buffer to encode to.</param>
    /// <returns>Whether the operation was successful.</returns>
    bool TryEncode(IMemoryBuffer<byte> destination);
}
