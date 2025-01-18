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
using System.Linq;
using Emzi0767.NetworkSelfService.Mikrotik.Exceptions;
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
            MikrotikThrowHelper.Throw_Argument(nameof(words), "Supplied word sequence is invalid.");

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

    internal void ThrowIfError()
    {
        if (this.Words.First() is not MikrotikReplyWord { IsError: true })
            return;

        var category = MikrotikApiErrorCategory.Unknown;
        var message = "";
        foreach (var word in this.Words)
        {
            if (word is not MikrotikAttributeWord attr)
                continue;

            switch (attr.Name)
            {
                case "category":
                    category = (MikrotikApiErrorCategory)int.Parse(attr.Value);
                    break;

                case "message":
                    message = attr.Value;
                    break;
            }
        }

        MikrotikThrowHelper.Throw_MikrotikApi(message, category);
    }

    /// <summary>
    /// Validates a sequence of API words in a sentence.
    /// </summary>
    /// <param name="words">Enumerable of words to validate.</param>
    /// <returns>Whether the sequence is valid.</returns>
    private static bool Validate(IEnumerable<IMikrotikWord> words)
        => words.Any()
        && (words.First() is MikrotikCommandWord || words.First() is MikrotikReplyWord)
        && words.Last() is MikrotikStopWord;
}
