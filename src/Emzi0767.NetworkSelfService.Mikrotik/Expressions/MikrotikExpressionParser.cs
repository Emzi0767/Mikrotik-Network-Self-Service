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
using System.Linq.Expressions;

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

internal sealed class MikrotikExpressionParser
{
    public static MikrotikExpressionParser Instance { get; } = new();

    public IEnumerable<IMikrotikWord> Parse(Expression expression, Type rootType)
    {
        ValidateRootType(rootType);
        return
        [
            ..this.ParseInternal(expression, typeof(IAsyncQueryable<>).MakeGenericType(rootType)),
            MikrotikStopWord.Instance,
        ];
    }

    private IEnumerable<IMikrotikWord> ParseInternal(Expression expression, Type queryableType)
    {
        switch (expression)
        {
            case null:
                yield break;

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
                var resultType = methodCall.GetTargetElementType();
                foreach (var word in this.ParseInternal(source, queryableType))
                    yield return word;

                foreach (var word in this.ParseTransformInternal(transformType, source, methodCall, resultType))
                    yield return word;

                break;
            }

            case ConstantExpression constant:
            {
                if (!queryableType.IsAssignableFrom(constant.Type))
                    MikrotikThrowHelper.Throw_NotSupported("API expressions need to be rooted in an instance of IAsyncQueryable<T>.");

                Debug.WriteLine("-- -- -- -- -- --");
                Debug.WriteLine("CONSTANT:     {0}", [ constant.Value ]);
                Debug.WriteLine("TYPE:         {0}", [ constant.GetTargetElementType() ]);
                // TODO: build command word
                yield break;
            }

            default:
            {
                Debug.WriteLine("-- -- -- -- -- --");
                Debug.WriteLine("UNSUPPORTED EXPRESSION");
                MikrotikThrowHelper.Throw_NotSupported($"'{expression.NodeType}' expressions are not supported.");
                yield break;
            }
        }
    }

    private IEnumerable<IMikrotikWord> ParseTransformInternal(
        MikrotikTransformType transformType,
        Expression source,
        MethodCallExpression transform,
        Type resultType
    )
    {
        switch (transformType)
        {
            case MikrotikTransformType.OfType:
                return this.ParseOfType(source, transform, resultType);

            case MikrotikTransformType.Where:
            {
                var filter = transform.Arguments[1];
                return this.ParseWhere(source, filter, resultType);
            }

            case MikrotikTransformType.Select:
            {
                var selector = transform.Arguments[1];
                return this.ParseSelect(source, selector, resultType);
            }

            case MikrotikTransformType.SelectMany:
            {
                var selector = transform.Arguments[1];
                var transformer = transform.Arguments.Count >= 3
                    ? transform.Arguments[2]
                    : null;

                return this.ParseSelectMany(source, selector, transformer, resultType);
            }
        }

        MikrotikThrowHelper.Throw_NotSupported($"'{transform.Method}' transforms are not supported.'");
        return null;
    }

    private IEnumerable<IMikrotikWord> ParseOfType(Expression source, MethodCallExpression transform, Type resultType)
    {
        var sourceType = source.GetTargetElementType();
        ValidateTypeTransform(sourceType, resultType);

        // TODO: create a query for ?=${TYPE_DISCRIM}=${TYPE_VALUE}
        MikrotikThrowHelper.Throw_NotSupported("OfType transforms are currently not supported.");
        yield break;
    }

    private IEnumerable<IMikrotikWord> ParseWhere(Expression source, Expression filter, Type resultType)
    {
        // TODO: create a query
        yield break;
    }

    private IEnumerable<IMikrotikWord> ParseSelect(Expression source, Expression selector, Type resultType)
    {
        // TODO: adjust .proplist
        // should only support (x => new { ... }) transforms
        yield break;
    }

    private IEnumerable<IMikrotikWord> ParseSelectMany(Expression source, Expression selector, Expression transformer, Type resultType)
    {
        // TODO: adjust .proplist to just one property
        // should only support (x => x.Property) transforms
        yield break;
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
