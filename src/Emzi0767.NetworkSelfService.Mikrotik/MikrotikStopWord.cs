using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Gets the Mikrotik API stop (empty) word.
/// </summary>
public sealed class MikrotikStopWord : IMikrotikWord
{
    /// <summary>
    /// Gets the instance of this word.
    /// </summary>
    public static MikrotikStopWord Instance { get; } = new();

    /// <inheritdoc />
    public int Length
        => 0;

    private MikrotikStopWord()
    { }

    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
        => true;
}
