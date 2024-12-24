using Emzi0767.Utilities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents arguments for <see cref="MikrotikClient.SentenceReceived"/>.
/// </summary>
public sealed class MikrotikSentenceReceivedEventArgs : AsyncEventArgs
{
    /// <summary>
    /// Gets the sentence that was received.
    /// </summary>
    public MikrotikSentence Sentence { get; init; }
}