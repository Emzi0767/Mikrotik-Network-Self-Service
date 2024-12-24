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