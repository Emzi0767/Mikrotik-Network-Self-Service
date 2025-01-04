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

namespace Emzi0767.NetworkSelfService.Mikrotik;

internal static class MikrotikWordParser
{
    public static bool TryParseWord(this string decodedWord, out IMikrotikWord word)
    {
        if (string.IsNullOrEmpty(decodedWord))
        {
            word = MikrotikStopWord.Instance;
            return true;
        }

        if (decodedWord.StartsWith(MikrotikReplyWord.Prefix))
        {
            var response = decodedWord.Substring(MikrotikReplyWord.Prefix.Length);
            var responseType = MikrotikReplyWord.GetReplyType(response);
            word = new MikrotikReplyWord(responseType);
            return true;
        }
        else if (decodedWord.StartsWith(MikrotikAttributeWord.Prefix))
        {
            var kv = decodedWord.Substring(MikrotikAttributeWord.Prefix.Length).Split(MikrotikAttributeWord.Separator, 2, StringSplitOptions.None);
            var k = kv[0];
            var v = kv[1];
            word = new MikrotikAttributeWord(k, v);
            return true;
        }
        else if (decodedWord.StartsWith(MikrotikSentenceAttributeWord.Prefix))
        {
            var kv = decodedWord.Substring(MikrotikSentenceAttributeWord.Prefix.Length).Split(MikrotikSentenceAttributeWord.Separator, 2, StringSplitOptions.None);
            var k = kv[0];
            var v = kv[1];
            word = new MikrotikSentenceAttributeWord(k, v);
            return true;
        }
        
        word = new MikrotikRawWord(decodedWord);
        return true;
    }
}