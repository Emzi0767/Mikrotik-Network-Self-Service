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
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;

namespace Emzi0767.NetworkSelfService.Mikrotik.SourceGens;

[Generator]
public sealed class EntityProxyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            hintName: $"{Constants.GenerateAttributeName}.g.cs",
            Constants.GenerateMikrotikEntityMetadataAttribute.ToSourceText()
        ));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            hintName: $"{Constants.GenerateEnumAttributeName}.g.cs",
            Constants.GenerateEnumMetadataAttribute.ToSourceText()
        ));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            hintName: $"{Constants.GenerateObjectAttributeName}.g.cs",
            Constants.GenerateObjectMetadataAttribute.ToSourceText()
        ));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            hintName: $"{Constants.ProxyInterfaceName}.g.cs",
            Constants.MikrotikEntityProxyImplementation.ToSourceText()
        ));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            hintName: $"{Constants.ObjectProxyInterfaceName}.g.cs",
            Constants.ObjectProxyImplementation.ToSourceText()
        ));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            hintName: $"{Constants.EntityProxiesClassName}.g.cs",
            Constants.EntityProxiesClass.ToSourceText()
        ));

        var entityMetadata = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                Constants.GenerateAttributeQualifiedName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetEntityMetadata(ctx.SemanticModel, ctx.TargetNode)
            )
            .Where(static m => m.IsInitialized);

        var bulkMetadata = entityMetadata.Collect();

        var enumMetadata = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                Constants.GenerateEnumAttributeQualifiedName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetEnumMetadata(ctx.SemanticModel, ctx.TargetNode)
            )
            .Where(static m => m.IsInitialized);

        var bulkEnumMetadata = enumMetadata.Collect();

        var objectMetadata = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                Constants.GenerateObjectAttributeQualifiedName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetObjectMetadata(ctx.SemanticModel, ctx.TargetNode)
            )
            .Where(static m => m.IsInitialized);

        var bulkObjectMetadata = objectMetadata.Collect();

        context.RegisterSourceOutput(entityMetadata, static (ctx, metadata) => Execute(metadata, ctx));
        context.RegisterSourceOutput(bulkMetadata, static (ctx, metadatas) => ExecuteBulk(metadatas, ctx));
        context.RegisterSourceOutput(enumMetadata, static (ctx, metadata) => ExecuteEnum(metadata, ctx));
        context.RegisterSourceOutput(bulkEnumMetadata, static (ctx, metadatas) => ExecuteEnumBulk(metadatas, ctx));
        context.RegisterSourceOutput(objectMetadata, static (ctx, metadata) => ExecuteObject(metadata, ctx));
        context.RegisterSourceOutput(bulkObjectMetadata, static (ctx, metadatas) => ExecuteObjectBulk(metadatas, ctx));
    }

    private static EntityMetadata GetEntityMetadata(SemanticModel model, SyntaxNode root)
    {
        if (model.GetDeclaredSymbol(root) is not INamedTypeSymbol symbol)
            return default;

        var entityName = symbol.Name;
        var qualifiedName = symbol.ToDisplayString(Constants.QualifiedTypeName);
        var members = symbol.GetMembers();
        var memberMetadata = ImmutableArray<EntityMemberMetadata>.Empty;
        var path = default(string[]);

        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass is null || attr.AttributeClass.ToDisplayString(Constants.QualifiedTypeName) != Constants.EntityAttributeQualifiedName)
                continue;

            var @const = attr.ConstructorArguments.First();
            if (@const.Kind != TypedConstantKind.Array)
                return default;

            path = @const.Values.Select(x => x.Value as string)
                .ToArray();
        }

        if (path is null or { Length: 0 })
            return default;

        foreach (var member in members)
        {
            if (member is not IPropertySymbol { SetMethod.DeclaredAccessibility: Accessibility.Internal, GetMethod.DeclaredAccessibility: Accessibility.Public } property)
                continue;

            var serializedName = default(string);
            var isReadOnly = false;
            foreach (var attr in member.GetAttributes())
            {
                if (attr.AttributeClass.ToDisplayString(Constants.QualifiedTypeName) == typeof(DataMemberAttribute).FullName)
                    serializedName = attr.NamedArguments.FirstOrDefault(x => x.Key == nameof(DataMemberAttribute.Name)).Value.Value as string;

                else if (attr.AttributeClass.ToDisplayString(Constants.QualifiedTypeName) == Constants.ReadonlyAttributeQualifiedName)
                    isReadOnly = true;
            }

            if (string.IsNullOrWhiteSpace(serializedName))
                continue;

            var type = property.Type;
            var typeName = type.ToDisplayString(Constants.QualifiedTypeName);

            memberMetadata = memberMetadata.Add(new(member.Name, typeName, serializedName, isReadOnly));
        }

        return new(entityName, qualifiedName, memberMetadata, [..path]);
    }

    private static EnumMetadata GetEnumMetadata(SemanticModel model, SyntaxNode root)
    {
        if (model.GetDeclaredSymbol(root) is not INamedTypeSymbol symbol)
            return default;

        var entityName = symbol.Name;
        var qualifiedName = symbol.ToDisplayString(Constants.QualifiedTypeName);
        var members = symbol.GetMembers();
        var mappings = new Dictionary<string, string>();

        foreach (var member in members)
        {
            if (member is not IFieldSymbol field || field.ConstantValue is null)
                continue;

            var attr = member.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass is not null
                    && x.AttributeClass.ToDisplayString(Constants.QualifiedTypeName) == typeof(DataMemberAttribute).FullName);

            if (attr is null)
                continue;

            var serializedName = attr.NamedArguments.FirstOrDefault(x => x.Key == nameof(DataMemberAttribute.Name)).Value.Value as string;
            if (string.IsNullOrWhiteSpace(serializedName))
                continue;

            mappings.Add(field.Name, serializedName);
        }

        return new(entityName, qualifiedName, mappings);
    }

    private static ObjectMetadata GetObjectMetadata(SemanticModel model, SyntaxNode root)
    {
        if (model.GetDeclaredSymbol(root) is not INamedTypeSymbol symbol)
            return default;

        var entityName = symbol.Name;
        var qualifiedName = symbol.ToDisplayString(Constants.QualifiedTypeName);
        var members = symbol.GetMembers();
        var memberMetadata = ImmutableArray<ObjectMemberMetadata>.Empty;

        foreach (var member in members)
        {
            if (member is not IPropertySymbol { GetMethod: not null } property)
                continue;

            var serializedName = default(string);
            foreach (var attr in member.GetAttributes())
            {
                if (attr.AttributeClass.ToDisplayString(Constants.QualifiedTypeName) == typeof(DataMemberAttribute).FullName)
                    serializedName = attr.NamedArguments.FirstOrDefault(x => x.Key == nameof(DataMemberAttribute.Name)).Value.Value as string;
            }

            if (string.IsNullOrWhiteSpace(serializedName))
                continue;

            var type = property.Type;
            var typeName = type.ToDisplayString(Constants.QualifiedTypeName);

            memberMetadata = memberMetadata.Add(new(member.Name, typeName, serializedName));
        }

        return new(entityName, qualifiedName, memberMetadata);
    }

    private static void Execute(in EntityMetadata metadata, SourceProductionContext context)
    {
        if (!metadata.IsInitialized)
            return;

        context.AddSource(
            $"{Constants.EntityProxiesClassName}.{metadata.QualifiedName}.g.cs",
            Constants.GenerateEntityProxyStatic(metadata).ToSourceText()
        );
    }

    private static void ExecuteEnum(in EnumMetadata metadata, SourceProductionContext context)
    {
        if (!metadata.IsInitialized)
            return;

        context.AddSource(
            $"{Constants.EnumProxiesClassName}.{metadata.QualifiedName}.g.cs",
            Constants.GenerateEnumProxyStatic(metadata).ToSourceText()
        );
    }

    private static void ExecuteObject(in ObjectMetadata metadata, SourceProductionContext context)
    {
        if (!metadata.IsInitialized)
            return;

        context.AddSource(
            $"{Constants.ObjectProxiesClassName}.{metadata.QualifiedName}.g.cs",
            Constants.GenerateObjectProxyStatic(metadata).ToSourceText()
        );
    }

    private static void ExecuteBulk(in ImmutableArray<EntityMetadata> metadatas, SourceProductionContext context)
    {
        context.AddSource(
            $"{Constants.EntityProxiesClassName}.pathmap.g.cs",
            Constants.GenerateEntityPathMapStatic(metadatas).ToSourceText()
        );
    }

    private static void ExecuteEnumBulk(in ImmutableArray<EnumMetadata> metadatas, SourceProductionContext context)
    {
        context.AddSource(
            $"{Constants.EnumProxiesClassName}.map.g.cs",
            Constants.GenerateEnumMapStatic(metadatas).ToSourceText()
        );
    }

    private static void ExecuteObjectBulk(in ImmutableArray<ObjectMetadata> metadatas, SourceProductionContext context)
    {
        context.AddSource(
            $"{Constants.ObjectProxiesClassName}.pathmap.g.cs",
            Constants.GenerateObjectMapStatic(metadatas).ToSourceText()
        );
    }
}
