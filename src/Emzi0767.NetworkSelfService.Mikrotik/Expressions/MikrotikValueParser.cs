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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

public static class MikrotikValueParser
{
    private static ConcurrentDictionary<Type, ParseDelegate> _converters = new();
    private static MethodInfo _parseParsable = ((Delegate)ParseParsable<int>).Method.GetGenericMethodDefinition();
    private static ParseDelegate _parseGeneric = ParseGeneric;

    private static ConcurrentDictionary<Type, ParseListDelegate> _listConverters = new();
    private static MethodInfo _parseParsableList = ((Delegate)ParseParsableList<int>).Method.GetGenericMethodDefinition();
    private static ParseListDelegate _parseGenericList = ParseGenericList;

    public static object Parse(Type type, StringSegment serialized, out Result result)
    {
        if (type != typeof(string) && type.IsEnumerableType(out var tItem))
        {
            if (_listConverters.TryGetValue(tItem, out var listParse))
                return listParse(tItem, serialized, out result);

            if (tItem.IsSpanParsable())
            {
                listParse = _parseParsableList.MakeGenericMethod(tItem)
                    .CreateDelegate<ParseListDelegate>();
            }
            else
            {
                listParse = _parseGenericList;
            }

            _listConverters[tItem] = listParse;
            return listParse(tItem, serialized, out result);
        }

        if (type == typeof(string))
        {
            result = Result.Success;
            return serialized.ToString();
        }
        else if (type == typeof(bool))
        {
            result = serialized == "yes" || serialized == "no" || serialized == "true" || serialized == "false"
                ? Result.Success
                : Result.Failure;

            return serialized == "yes" || serialized == "true";
        }

        if (_converters.TryGetValue(type, out var parse))
            parse(type, serialized, out result);

        if (type.IsSpanParsable())
        {
            parse = _parseParsable.MakeGenericMethod(type)
                .CreateDelegate<ParseDelegate>();
        }
        else
        {
            parse = _parseGeneric;
        }

        _converters[type] = parse;
        return parse(type, serialized, out result);
    }

    private static object ParseParsable<T>(Type type, StringSegment serialized, out Result result)
        where T : ISpanParsable<T>
    {
        var val = ParseParsableInner<T>(type, serialized, out result);
        return result == Result.Success
            ? val
            : null;
    }

    private static T ParseParsableInner<T>(Type type, StringSegment serialized, out Result result)
        where T : ISpanParsable<T>
    {
        result = Result.Failure;
        if (T.TryParse(serialized, CultureInfo.InvariantCulture, out var parsed))
        {
            result = Result.Success;
            return parsed;
        }

        return default;
    }

    private static object ParseGeneric(Type type, StringSegment serialized, out Result result)
    {
        result = Result.Failure;
        MikrotikThrowHelper.Throw_NotSupported("This type of conversion is not supported.");
        return null;
    }

    private static object ParseParsableList<T>(Type itemType, StringSegment serialized, out Result result)
        where T : ISpanParsable<T>
    {
        if (serialized.Length == 0 && itemType != typeof(string))
        {
            result = Result.Success;
            return Array.Empty<T>();
        }

        var quote = serialized.IndexOf('"');
        var separator = serialized.IndexOf(',');
        if (separator != -1)
        {
            var obj = ParseParsableInner<T>(itemType, serialized, out result);
            return result == Result.Success
                ? new[] { obj }
                : null;
        }

        var items = new List<T>();
        while (serialized.Length > 0)
        {
            if (separator == -1)
            {
                items.Add(ParseParsableInner<T>(itemType, serialized, out result));
                if (result != Result.Success)
                    return null;
            }

            if (quote != -1 && separator > quote)
            {
                quote = serialized.IndexOf('"', quote + 1);
                separator = serialized.IndexOf(',', quote + 1);
            }

            var sub = serialized.Subsegment(0, separator);
            if (quote != -1)
                sub = serialized.Subsegment(1, serialized.Length - 2);

            items.Add(ParseParsableInner<T>(itemType, sub, out result));
            if (result != Result.Success)
                return null;

            serialized = serialized.Subsegment(separator + 1);
            quote = serialized.IndexOf('"');
            separator = serialized.IndexOf(',');
        }

        result = Result.Success;
        return items;
    }

    private static IEnumerable<object> ParseGenericList(Type itemType, StringSegment serialized, out Result result)
    {
        result = Result.Failure;
        MikrotikThrowHelper.Throw_NotSupported("This type of conversion is not supported.");
        return null;
    }

    private delegate object ParseDelegate(Type type, StringSegment serialized, out Result result);
    private delegate object ParseListDelegate(Type itemType, StringSegment serialized, out Result result);

    public enum Result
    {
        Success,
        Failure,
    }
}
