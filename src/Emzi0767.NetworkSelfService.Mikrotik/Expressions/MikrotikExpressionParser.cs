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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

internal sealed class MikrotikExpressionParser
{
    public static MikrotikExpressionParser Instance { get; } = new();

    public MikrotikExpression Parse(Expression expression, Type rootType)
    {
        ValidateRootType(rootType);

        var state = new MikrotikExpressionParserState(rootType);
        state.IncludedPropertyNames.AddRange(EntityProxies.GetProperties(rootType));

        this.ParseInternal(expression, ref state);
        this.AddPropertyList(ref state);
        state.Words.Add(MikrotikStopWord.Instance);

        var properties = state.IncludedPropertyNames;
        var inflater = (state.AnonymousConstructor is not null, state.IsSelectMany) switch
        {
            (false, false) => new MikrotikEntityInflater { ObjectType = state.ResultType } as IMikrotikInflater,
            (true, false) => new MikrotikAnonymousInflater
            {
                ObjectType = state.ResultType,
                Constructor = state.AnonymousConstructor,
                SerializedProperties = state.IncludedPropertyNames.Select(x => EntityProxies.MapToSerialized(rootType, x))
                    .ToArray(),
            },
            (false, true) => new MikrotikPassthroughInflater
            {
                ObjectType = state.ResultType,
                SerializedProperty = EntityProxies.MapToSerialized(rootType, properties.Single()),
            },
            _ => null,
        };

        if (inflater is null)
            MikrotikThrowHelper.Throw_InvalidOperation("Invalid expression state reached.");

        var expr = new MikrotikExpression
        {
            Sentence = new(state.Words),
            Inflater = inflater,
            UnpackEnumerable = state.IsSelectMany,
        };

        return expr;
    }

    private void ParseInternal(Expression expression, ref MikrotikExpressionParserState state)
    {
        switch (expression)
        {
            case null:
                return;

            case MethodCallExpression methodCall:
            {
                Debug.WriteLine("-- -- -- -- -- --");
                Debug.WriteLine("METHOD CALL:  {0}", [ methodCall.Method ]);
                Debug.WriteLine("IS SUPPORTED: {0}", [ methodCall.IsSupportedTransform() ]);
                Debug.WriteLine("CHAINED FROM: {0}", [ methodCall.Arguments[0] ]);
                Debug.WriteLine("RESULT TYPE:  {0}", [ methodCall.GetTargetElementType() ]);

                if (!methodCall.IsSupportedTransform())
                    MikrotikThrowHelper.Throw_NotSupported($"'{methodCall.Method}' transforms are not supported.'");

                var transformType = methodCall.GetTransformType();
                var source = methodCall.Arguments[0];
                state.ResultType = methodCall.GetTargetElementType();
                this.ParseInternal(source, ref state);
                this.ParseTransformInternal(transformType, source, methodCall, ref state);
                return;
            }

            case ConstantExpression constant:
            {
                if (!state.QueryableType.IsAssignableFrom(constant.Type))
                    MikrotikThrowHelper.Throw_NotSupported("API expressions need to be rooted in an instance of IAsyncQueryable<T>.");

                Debug.WriteLine("-- -- -- -- -- --");
                Debug.WriteLine("CONSTANT:     {0}", [ constant.Value ]);
                Debug.WriteLine("TYPE:         {0}", [ constant.GetTargetElementType() ]);

                state.Words.Add(
                    new MikrotikCommandWord([
                        ..EntityProxies.GetPath(state.RootType),
                        "print",
                    ])
                );
                return;
            }

            default:
            {
                Debug.WriteLine("-- -- -- -- -- --");
                Debug.WriteLine("UNSUPPORTED EXPRESSION");
                MikrotikThrowHelper.Throw_NotSupported($"'{expression.NodeType}' expressions are not supported.");
                return;
            }
        }
    }

    private void ParseTransformInternal(
        MikrotikTransformType transformType,
        Expression source,
        MethodCallExpression transform,
        ref MikrotikExpressionParserState state
    )
    {
        if (state.IsSelectMany)
        {
            MikrotikThrowHelper.Throw_NotSupported("Nested SelectMany is not supported.");
            return;
        }

        switch (transformType)
        {
            case MikrotikTransformType.OfType:
                this.ParseOfType(source, transform, ref state);
                return;

            case MikrotikTransformType.Where:
            {
                var filter = transform.Arguments[1];
                this.ParseWhere(source, filter, ref state);
                return;
            }

            case MikrotikTransformType.Select:
            {
                var selector = transform.Arguments[1];
                this.ParseSelect(source, selector, ref state);
                return;
            }

            case MikrotikTransformType.SelectMany:
            {
                var selector = transform.Arguments[1];
                var transformer = transform.Arguments.Count >= 3
                    ? transform.Arguments[2]
                    : null;

                this.ParseSelectMany(source, selector, transformer, ref state);
                return;
            }
        }

        MikrotikThrowHelper.Throw_NotSupported($"'{transform.Method}' transforms are not supported.'");
    }

    private void ParseOfType(Expression source, MethodCallExpression transform, ref MikrotikExpressionParserState state)
    {
        var sourceType = source.GetTargetElementType();
        ValidateTypeTransform(sourceType, state.ResultType);

        state.ResultType = sourceType;
        state.IncludedPropertyNames.Clear();
        // TODO: add all member names
        // TODO: create a query for ?=${TYPE_DISCRIM}=${TYPE_VALUE}
        MikrotikThrowHelper.Throw_NotSupported("OfType transforms are currently not supported.");
    }

    private void ParseWhere(Expression source, Expression filter, ref MikrotikExpressionParserState state)
    {
        // TODO: create a query
        return;
    }

    private void ParseSelect(Expression source, Expression selector, ref MikrotikExpressionParserState state)
    {
        if (selector is not UnaryExpression { NodeType: ExpressionType.Quote, Operand: LambdaExpression { Body: NewExpression { Members: not null } anon } })
        {
            MikrotikThrowHelper.Throw_NotSupported("Only basic anonymous object transforms are supported.");
            return;
        }

        var anonMap = state.AnonymousPropertyMap;

        state.ResultType = source.GetTargetElementType();
        state.AnonymousConstructor = anon.Constructor;
        state.AnonymousPropertyMap = new Dictionary<string, string>();
        state.IncludedPropertyNames.Clear();
        foreach (var (member, arg) in anon.Members.Zip(anon.Arguments))
        {
            if (arg is not MemberExpression { Member: PropertyInfo prop, Expression: ParameterExpression })
            {
                MikrotikThrowHelper.Throw_NotSupported("Only basic anonymous object transforms are supported.");
                return;
            }

            var propName = prop.Name;
            if (anonMap is not null && !anonMap.TryGetValue(propName, out propName))
            {
                MikrotikThrowHelper.Throw_InvalidOperation("Invalid property specified for Select transform.");
                return;
            }

            state.IncludedPropertyNames.Add(propName);
            state.AnonymousPropertyMap[member.Name] = propName;
        }
    }

    private void ParseSelectMany(Expression source, Expression selector, Expression transformer, ref MikrotikExpressionParserState state)
    {
        if (selector is not UnaryExpression { NodeType: ExpressionType.Quote, Operand: LambdaExpression { Body: MemberExpression { Member: PropertyInfo prop, Expression: ParameterExpression } } })
        {
            MikrotikThrowHelper.Throw_NotSupported("Only a single property can be directly selected for a SelectMany transform.");
            return;
        }

        if (transformer is not null)
        {
            MikrotikThrowHelper.Throw_NotSupported("Transforming SelectMany is not supported.");
            return;
        }

        var propType = prop.PropertyType;
        if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            state.ResultType = propType;
        }
        else
        {
            var elementType = prop.PropertyType.GetInterfaces()
                .First(x => x.IsGenericType && typeof(IEnumerable<>) == x.GetGenericTypeDefinition())
                .GetGenericArguments().First();

            state.ResultType = typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        state.IsSelectMany = true;
        state.IncludedPropertyNames.Clear();
        var propName = prop.Name;
        if (state.AnonymousPropertyMap is not null && !state.AnonymousPropertyMap.TryGetValue(propName, out propName))
        {
            MikrotikThrowHelper.Throw_InvalidOperation("Invalid property specified for SelectMany transform.");
            return;
        }

        state.IncludedPropertyNames.Add(propName);
        state.AnonymousConstructor = null;
        state.AnonymousPropertyMap = null;
    }

    private void AddPropertyList(ref MikrotikExpressionParserState state)
    {
        var rootType = state.RootType;
        var properties = state.IncludedPropertyNames
            .Select(x => EntityProxies.MapToSerialized(rootType, x));

        var propertiesValue = string.Join(",", properties);
        var word = new MikrotikAttributeWord(MikrotikAttributeWord.KeyPropertyList, propertiesValue);
        state.Words.Add(word);
    }

    private static void ValidateRootType(Type rootType)
    {
        if (!rootType.IsMikrotikEntity())
            MikrotikThrowHelper.Throw_InvalidEntityType(rootType);
    }

    private static void ValidateTypeTransform(Type priorType, Type transformType)
    {
        if (!transformType.IsMikrotikEntitySpecializationOf(priorType))
            MikrotikThrowHelper.Throw_InvalidEntityType(transformType, "Specified transformation uses an unsupported or invalid type relation. Ensure the target type is assignable to the source type.");
    }
}
