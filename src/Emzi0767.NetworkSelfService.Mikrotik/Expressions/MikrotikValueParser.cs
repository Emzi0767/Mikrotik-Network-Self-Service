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
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

// it's copy-paste
// honestly just couldn't bother at this point

public static partial class MikrotikValueParser
{
    private static Regex _timeSpanRegex = TimeSpanRegex();

    private static ConcurrentDictionary<Type, ParseDelegate> _converters = new();
    private static MethodInfo _parseParsable = ((Delegate)ParseParsable<int>).Method.GetGenericMethodDefinition();
    private static ParseDelegate _parseTimeSpan = ParseTimeSpan;
    private static MethodInfo _parseEnum = ((Delegate)ParseEnum<Result>).Method.GetGenericMethodDefinition();
    private static MethodInfo _parseNullableParsable = ((Delegate)ParseNullableParsable<int>).Method.GetGenericMethodDefinition();
    private static ParseDelegate _parseNullableTimeSpan = ParseNullableTimeSpan;
    private static MethodInfo _parseNullableEnum = ((Delegate)ParseNullableEnum<Result>).Method.GetGenericMethodDefinition();
    private static ParseDelegate _parseNullableGeneric = ParseNullableGeneric;
    private static ParseDelegate _parseGeneric = ParseGeneric;

    private static ConcurrentDictionary<Type, ParseListDelegate> _listConverters = new();
    private static MethodInfo _parseParsableList = ((Delegate)ParseParsableList<int>).Method.GetGenericMethodDefinition();
    private static ParseListDelegate _parseTimeSpanList = ParseTimeSpanList;
    private static MethodInfo _parseEnumList = ((Delegate)ParseEnumList<Result>).Method.GetGenericMethodDefinition();
    private static ParseListDelegate _parseGenericList = ParseGenericList;

    public static object Parse(Type type, ReadOnlySpan<char> serialized, out Result result)
    {
        if (type != typeof(string) && type.IsEnumerableType(out var tItem))
        {
            if (_listConverters.TryGetValue(tItem, out var listParse))
                return listParse(tItem, serialized, out result);

            if (tItem.IsEnum)
            {
                listParse = _parseEnumList.MakeGenericMethod(tItem)
                    .CreateDelegate<ParseListDelegate>();
            }
            else if (type == typeof(TimeSpan))
            {
                listParse = _parseTimeSpanList;
            }
            else if (tItem.IsSpanParsable())
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
            result = serialized is "yes" or "no" or "true" or "false"
                ? Result.Success
                : Result.Failure;

            return serialized is "yes" or "true";
        }

        if (_converters.TryGetValue(type, out var parse))
            parse(type, serialized, out result);

        if (type.IsNullable(out var tNullable))
        {
            if (tNullable.IsEnum)
            {
                parse = _parseNullableEnum.MakeGenericMethod(tNullable)
                    .CreateDelegate<ParseDelegate>();
            }
            else if (tNullable == typeof(TimeSpan))
            {
                parse = _parseNullableTimeSpan;
            }
            else if (tNullable.IsSpanParsable())
            {
                parse = _parseNullableParsable.MakeGenericMethod(tNullable)
                    .CreateDelegate<ParseDelegate>();
            }
            else
            {
                parse = _parseNullableGeneric;
            }
        }
        else if (type.IsEnum)
        {
            parse = _parseEnum.MakeGenericMethod(type)
                .CreateDelegate<ParseDelegate>();
        }
        else if (type == typeof(TimeSpan))
        {
            parse = _parseTimeSpan;
        }
        else if (type.IsSpanParsable())
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

    private static object ParseParsable<T>(Type type, ReadOnlySpan<char> serialized, out Result result)
        where T : ISpanParsable<T>
    {
        var val = ParseParsableInner<T>(type, serialized, out result);
        return result == Result.Success
            ? val
            : null;
    }

    private static T ParseParsableInner<T>(Type type, ReadOnlySpan<char> serialized, out Result result)
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

    private static object ParseTimeSpan(Type type, ReadOnlySpan<char> serialized, out Result result)
    {
        var val = ParseTimeSpanInner(type, serialized, out result);
        return result == Result.Success
            ? val
            : null;
    }

    private static TimeSpan ParseTimeSpanInner(Type type, ReadOnlySpan<char> serialized, out Result result)
    {
        var m = _timeSpanRegex.Match(serialized.ToString());
        result = m.Success
            ? Result.Success
            : Result.Failure;

        var days = m.Groups.TryGetValue("days", out var sDays) && sDays.Success
            ? int.Parse(sDays.Value, CultureInfo.InvariantCulture)
            : 0;

        var hours = m.Groups.TryGetValue("hours", out var sHours) && sHours.Success
            ? int.Parse(sHours.Value, CultureInfo.InvariantCulture)
            : 0;

        var minutes = m.Groups.TryGetValue("minutes", out var sMinutes) && sMinutes.Success
            ? int.Parse(sMinutes.Value, CultureInfo.InvariantCulture)
            : 0;

        var seconds = m.Groups.TryGetValue("seconds", out var sSeconds) && sSeconds.Success
            ? int.Parse(sSeconds.Value, CultureInfo.InvariantCulture)
            : 0;

        return new(days, hours, minutes, seconds);
    }

    private static object ParseEnum<T>(Type type, ReadOnlySpan<char> serialized, out Result result)
        where T : Enum
    {
        var val = ParseEnumInner<T>(type, serialized, out result);
        return result == Result.Success
            ? val
            : null;
    }

    private static T ParseEnumInner<T>(Type type, ReadOnlySpan<char> serialized, out Result result)
        where T : Enum
    {
        var parsed = EnumProxies.MapFromSerialized<T>(serialized.ToString());
        result = parsed is not null
            ? Result.Success
            : Result.Failure;

        return parsed;
    }

    private static object ParseNullableParsable<T>(Type type, ReadOnlySpan<char> serialized, out Result result)
        where T : ISpanParsable<T>
    {
        if (serialized.Length == 0)
        {
            result = Result.Success;
            return null;
        }

        return ParseParsable<T>(type, serialized, out result);
    }

    private static object ParseNullableTimeSpan(Type type, ReadOnlySpan<char> serialized, out Result result)
    {
        if (serialized.Length == 0)
        {
            result = Result.Success;
            return null;
        }

        return ParseTimeSpan(type, serialized, out result);
    }

    private static object ParseNullableEnum<T>(Type type, ReadOnlySpan<char> serialized, out Result result)
        where T : Enum
    {
        if (serialized.Length == 0)
        {
            result = Result.Success;
            return null;
        }

        return ParseEnum<T>(type, serialized, out result);
    }

    private static object ParseNullableGeneric(Type type, ReadOnlySpan<char> serialized, out Result result)
    {
        if (serialized.Length == 0)
        {
            result = Result.Success;
            return null;
        }

        return ParseGeneric(type, serialized, out result);
    }

    private static object ParseGeneric(Type type, ReadOnlySpan<char> serialized, out Result result)
    {
        result = Result.Failure;
        MikrotikThrowHelper.Throw_NotSupported("This type of conversion is not supported.");
        return null;
    }

    private static object ParseParsableList<T>(Type itemType, ReadOnlySpan<char> serialized, out Result result)
        where T : ISpanParsable<T>
    {
        if (serialized.Length == 0 && itemType != typeof(string))
        {
            result = Result.Success;
            return Array.Empty<T>();
        }

        var quote = serialized.IndexOf('"');
        var separator = serialized.IndexOf(',');
        if (separator == -1)
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

                break;
            }

            if (quote != -1 && separator > quote)
            {
                quote = serialized.NextIndexOf('"', quote + 1);
                separator = serialized.NextIndexOf(',', quote + 1);
            }

            var sub = serialized[..separator];
            if (quote != -1)
                sub = serialized.Slice(1, serialized.Length - 2);

            items.Add(ParseParsableInner<T>(itemType, sub, out result));
            if (result != Result.Success)
                return null;

            serialized = serialized[(separator + 1)..];
            quote = serialized.IndexOf('"');
            separator = serialized.IndexOf(',');
        }

        result = Result.Success;
        return items;
    }

    private static object ParseTimeSpanList(Type itemType, ReadOnlySpan<char> serialized, out Result result)
    {
        if (serialized.Length == 0 && itemType != typeof(string))
        {
            result = Result.Success;
            return Array.Empty<TimeSpan>();
        }

        var quote = serialized.IndexOf('"');
        var separator = serialized.IndexOf(',');
        if (separator == -1)
        {
            var obj = ParseTimeSpanInner(itemType, serialized, out result);
            return result == Result.Success
                ? new[] { obj }
                : null;
        }

        var items = new List<TimeSpan>();
        while (serialized.Length > 0)
        {
            if (separator == -1)
            {
                items.Add(ParseTimeSpanInner(itemType, serialized, out result));
                if (result != Result.Success)
                    return null;

                break;
            }

            if (quote != -1 && separator > quote)
            {
                quote = serialized.NextIndexOf('"', quote + 1);
                separator = serialized.NextIndexOf(',', quote + 1);
            }

            var sub = serialized[..separator];
            if (quote != -1)
                sub = serialized.Slice(1, serialized.Length - 2);

            items.Add(ParseTimeSpanInner(itemType, sub, out result));
            if (result != Result.Success)
                return null;

            serialized = serialized[(separator + 1)..];
            quote = serialized.IndexOf('"');
            separator = serialized.IndexOf(',');
        }

        result = Result.Success;
        return items;
    }

    private static object ParseEnumList<T>(Type itemType, ReadOnlySpan<char> serialized, out Result result)
        where T : Enum
    {
        if (serialized.Length == 0)
        {
            result = Result.Success;
            return Array.Empty<T>();
        }

        var quote = serialized.IndexOf('"');
        var separator = serialized.IndexOf(',');
        if (separator == -1)
        {
            var obj = ParseEnumInner<T>(itemType, serialized, out result);
            return result == Result.Success
                ? new[] { obj }
                : null;
        }

        var items = new List<T>();
        while (serialized.Length > 0)
        {
            if (separator == -1)
            {
                items.Add(ParseEnumInner<T>(itemType, serialized, out result));
                if (result != Result.Success)
                    return null;

                break;
            }

            if (quote != -1 && separator > quote)
            {
                quote = serialized.NextIndexOf('"', quote + 1);
                separator = serialized.NextIndexOf(',', quote + 1);
            }

            var sub = serialized[..separator];
            if (quote != -1)
                sub = serialized.Slice(1, serialized.Length - 2);

            items.Add(ParseEnumInner<T>(itemType, sub, out result));
            if (result != Result.Success)
                return null;

            serialized = serialized[(separator + 1)..];
            quote = serialized.IndexOf('"');
            separator = serialized.IndexOf(',');
        }

        result = Result.Success;
        return items;
    }

    private static IEnumerable<object> ParseGenericList(Type itemType, ReadOnlySpan<char> serialized, out Result result)
    {
        result = Result.Failure;
        MikrotikThrowHelper.Throw_NotSupported("This type of conversion is not supported.");
        return null;
    }

    private delegate object ParseDelegate(Type type, ReadOnlySpan<char> serialized, out Result result);

    private delegate object ParseListDelegate(Type itemType, ReadOnlySpan<char> serialized, out Result result);

    public enum Result
    {
        Success,
        Failure,
    }

    [GeneratedRegex(@"^(?:(?:(?<days>[0-9]+)d)?(?:(?(days)\s+)(?:(?:(?<hours>[0-9]{1,2})?:)?(?<minutes>[0-9]{1,2}):)?(?<seconds>[0-9]{1,2}))?|(?:(?<hours>[0-9]{1,2})h)?(?:(?<minutes>[0-9]{1,2})m)?(?:(?<seconds>[0-9]{1,2})s)?)$", RegexOptions.Compiled)]
    private static partial Regex TimeSpanRegex();
}
