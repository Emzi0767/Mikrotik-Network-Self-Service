using System;
using System.Collections.Generic;
using System.Linq;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API sentence.
/// </summary>
public readonly struct MikrotikSentence
{
    /// <summary>
    /// Gets all the words in this Mikrotik API sentence.
    /// </summary>
    public IEnumerable<IMikrotikWord> Words { get; }

    /// <summary>
    /// Creates a new Mikrotik API sentence from given words.
    /// </summary>
    /// <param name="words"></param>
    public MikrotikSentence(IEnumerable<IMikrotikWord> words)
    {
        if (words is null)
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(words), "Words enumerable cannot be null.");

        if (!Validate(words))
            MikrotikThrowHelper.Throw_ArgumentException(nameof(words), "Supplied word sequence is invalid.");

        this.Words = words;
    }

    /// <summary>
    /// Attempts to encode this word to the specified buffer.
    /// </summary>
    /// <param name="destination">Buffer to encode the word to.</param>
    /// <returns>Whether the operation succeeded.</returns>
    public bool TryEncode(IMemoryBuffer<byte> destination)
    {
        foreach (var word in this.Words)
        {
            if (!((long)word.Length).TryEncodeLength(destination))
                return false;
            
            if (!word.TryEncode(destination))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Validates a sequence of API words in a sentence.
    /// </summary>
    /// <param name="words">Enumerable of words to validate.</param>
    /// <returns>Whether the sequence is valid.</returns>
    private static bool Validate(IEnumerable<IMikrotikWord> words)
        => words.Any()
        && words.First() is MikrotikCommandWord
        && words.Last() is MikrotikStopWord;
}
