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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Emzi0767.NetworkSelfService.Mikrotik.Entities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Contains various helpers for <see cref="Expression"/> instances.
/// </summary>
public static class MikrotikExpressionHelpers
{
    private static MethodInfo QueryableSelect { get; } = typeof(Queryable)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(x => x.Name == nameof(Queryable.Select) && x.GetParameters().Length == 2);
    
    private static MethodInfo MikrotikQueryableCreateQuery { get; } = typeof(MikrotikQueryable)
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .First(x => x.Name == nameof(IQueryProvider.CreateQuery) && !x.IsGenericMethod);

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
            case ParameterExpression param:
                return param.Type;
            
            case MemberExpression member:
                return member.Type;
            
            case LambdaExpression lambda:
                return lambda.ReturnType;

            case MethodCallExpression call:
            {
                var method = call.Method;
                if (method.IsGenericMethod && method.GetGenericMethodDefinition() == QueryableSelect)
                    return method.GetGenericArguments()[1];
                
                return method.ReturnType;
            }
        }
            
        MikrotikThrowHelper.Throw_NotSupported($"{expression.NodeType} is not supported.");
        return null;
    }

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
}