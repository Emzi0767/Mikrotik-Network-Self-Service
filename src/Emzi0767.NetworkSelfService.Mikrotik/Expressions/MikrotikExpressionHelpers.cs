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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

/// <summary>
/// Contains various helpers for <see cref="Expression"/> instances.
/// </summary>
public static class MikrotikExpressionHelpers
{
    private static MethodInfo MikrotikQueryableCreateQuery { get; } = typeof(MikrotikQueryable)
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .First(x => x.Name == nameof(IQueryProvider.CreateQuery) && x.IsGenericMethod);

    private const string TransformSelect = nameof(Queryable.Select);
    private static ISet<MethodInfo> QueryableSelects { get; } = typeof(Queryable)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(x => x.Name == TransformSelect)
        .ToHashSet();

    private const string TransformSelectMany = nameof(Queryable.SelectMany);
    private static ISet<MethodInfo> QueryableSelectManys { get; } = typeof(Queryable)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(x => x.Name == TransformSelectMany)
        .ToHashSet();

    private const string TransformWhere = nameof(Queryable.Where);
    private static ISet<MethodInfo> QueryableWheres { get; } = typeof(Queryable)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(x => x.Name == TransformWhere)
        .ToHashSet();

    private const string TransformOfType = nameof(Queryable.OfType);
    private static ISet<MethodInfo> QueryableOfTypes { get; } = typeof(Queryable)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .Where(x => x.Name == TransformOfType)
        .ToHashSet();

    private static ISet<string> SupportedTransformNames { get; } = new HashSet<string>([ TransformSelect, TransformSelectMany, TransformWhere ]);
    private static ISet<MethodInfo> SupportedTransforms { get; } =
        new HashSet<MethodInfo>([
            ..QueryableSelects,
            ..QueryableSelectManys,
            ..QueryableWheres,
            ..QueryableOfTypes,
        ]);

    /// <summary>
    /// Attempts to discover the target element type of the given query.
    /// </summary>
    /// <param name="expression">Expression to discover the type of.</param>
    /// <returns>Discovered type.</returns>
    public static Type GetTargetElementType(this Expression expression)
    {
        if (expression is null)
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(expression), "Expression cannot be null.");

        switch (expression)
        {
            case ConstantExpression @const:
                return @const.Type.Simplify();

            case ParameterExpression param:
                return param.Type;

            case MemberExpression member:
                return member.Type;

            case LambdaExpression lambda:
                return lambda.ReturnType;

            case MethodCallExpression call:
            {
                var method = call.Method;
                return method.IsGenericMethod && SupportedTransforms.Contains(method.GetGenericMethodDefinition())
                    ? method.GetGenericArguments().Last()
                    : method.ReturnType;
            }
        }

        MikrotikThrowHelper.Throw_NotSupported($"{expression.NodeType} is not supported.");
        return null;
    }

    /// <summary>
    /// Simplifies <see cref="IAsyncQueryable{T}"/> types to just their generic parameter.
    /// </summary>
    /// <param name="type">Type to simplify.</param>
    /// <returns>Simplified type.</returns>
    public static Type Simplify(this Type type)
        => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>))
        ? type.GetGenericArguments().First()
        : type;

    /// <summary>
    /// Attempts to instantiate a generic version of non-generic
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MethodInfo CreateQueryGeneric(this Expression expression)
    {
        var tElement = expression.GetTargetElementType();
        return MikrotikQueryableCreateQuery.MakeGenericMethod(tElement);
    }

    /// <summary>
    /// Gets whether the method call expression is a transform supported by the Mikrotik API. Note that this does not
    /// validate the inner expression.
    /// </summary>
    /// <param name="expression">Expression to validate.</param>
    /// <returns>Whether the transform is supported.</returns>
    public static bool IsSupportedTransform(this MethodCallExpression expression)
        => expression.Method.IsGenericMethod && SupportedTransforms.Contains(expression.Method.GetGenericMethodDefinition());

    internal static MikrotikTransformType GetTransformType(this MethodCallExpression expression)
        => Enum.Parse<MikrotikTransformType>(expression.Method.Name, false);

    internal static bool IsEnumerableType(this Type t, out Type tItem)
    {
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            tItem = t.GetGenericArguments().First();
            return true;
        }

        foreach (var iface in t.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                tItem = iface.GetGenericArguments().First();
                return true;
            }
        }

        tItem = null;
        return false;
    }

    internal static bool IsEnumerableType<T>(out Type tItem)
        => typeof(T).IsEnumerableType(out tItem);

    internal static bool IsSpanParsable(this Type t)
        => t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISpanParsable<>));

    internal static bool IsSpanParsable<T>(this Type t)
        => typeof(T).IsSpanParsable();
}
