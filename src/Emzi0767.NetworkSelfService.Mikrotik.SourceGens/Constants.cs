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
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Emzi0767.NetworkSelfService.Mikrotik.SourceGens;

/// <summary>
/// Contains static and constant values for the generator.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the name of the attribute which marks entities to generate proxies for.
    /// </summary>
    public const string GenerateAttributeName = "GenerateMikrotikEntityMetadataAttribute";

    /// <summary>
    /// Gets the name of the interface which all proxies implement.
    /// </summary>
    public const string ProxyInterfaceName = "IMikrotikEntityProxy";

    /// <summary>
    /// Gets the name of the implementation of an entity proxy.
    /// </summary>
    public const string ProxyImplementationName = "MikrotikEntityProxy";

    /// <summary>
    /// Gets the name of the interface the proxy getter/setters implement.
    /// </summary>
    public const string ProxyGetterSetterInterfaceName = "IMikrotikEntityProxyGetterSetter";

    /// <summary>
    /// Gets the name of the proxy getter/setter implementation.
    /// </summary>
    public const string ProxyGetterSetterImplementationName = "MikrotikEntityProxyGetterSetter";

    /// <summary>
    /// Gets the name of the entity proxy static class.
    /// </summary>
    public const string EntityProxiesClassName = "EntityProxies";

    /// <summary>
    /// Gets the namespace the generated code is in.
    /// </summary>
    public static string[] GeneratedNamespace { get; } = ["Emzi0767", "NetworkSelfService", "Mikrotik"];

    /// <summary>
    /// Gets the full qualified name of the generate attribute.
    /// </summary>
    public static string GenerateAttributeQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + GenerateAttributeName;

    /// <summary>
    /// Gets the full qualified name of the proxy interface.
    /// </summary>
    public static string ProxyInterfaceQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + ProxyInterfaceName;

    /// <summary>
    /// Gets the full qualified name of the proxy implementation.
    /// </summary>
    public static string ProxyImplementationQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + ProxyImplementationName;

    /// <summary>
    /// Gets the full qualified name of the proxy getter/setter interface.
    /// </summary>
    public static string ProxyGetterSetterInterfaceQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + ProxyGetterSetterInterfaceName;

    /// <summary>
    /// Gets the full qualified name of the proxy getter/setter implementation.
    /// </summary>
    public static string ProxyGetterSetterImplementationQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + ProxyGetterSetterImplementationName;

    /// <summary>
    /// Gets the full qualified name of the entity proxy static class.
    /// </summary>
    public static string EntityProxiesClassQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + EntityProxiesClassName;

    /// <summary>
    /// Gets the style for fully qualified type names without global prefix.
    /// </summary>
    public static SymbolDisplayFormat QualifiedTypeName { get; } = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    /// <summary>
    /// Converts a given syntax node to a source text.
    /// </summary>
    /// <param name="syntax">Syntax node to convert.</param>
    /// <returns>Created source text.</returns>
    public static SourceText ToSourceText(this CSharpSyntaxNode syntax)
        => SourceText.From(syntax.ToFullString(), Encoding.UTF8);

    /// <summary>
    /// Creates a qualified name syntax from a given dotted qualified name.
    /// </summary>
    /// <param name="dottedQualifiedName">Qualified name, where identifiers are separated with dots.</param>
    /// <returns>Constructed qualified name syntax instance.</returns>
    public static NameSyntax CreateQualifiedName(string dottedQualifiedName)
    {
        var syntax = default(NameSyntax);
        foreach (var component in dottedQualifiedName.Split('.'))
            syntax = syntax is null
                ? SyntaxFactory.IdentifierName(component)
                : SyntaxFactory.QualifiedName(syntax, SyntaxFactory.IdentifierName(component));

        return syntax;
    }

    /// <summary>
    /// Creates a qualifier from a given dotted qualified name.
    /// </summary>
    /// <param name="dottedQualifiedName">Qualified name, where identifiers are separated with dots.</param>
    /// <returns>Constructed qualifier.</returns>
    public static string CreateQualifier(string dottedQualifiedName)
        => string.Join("", dottedQualifiedName.Split('.'));

    // using System;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // [AttributeUsage(AttributeTargets.Class)]
    // public sealed class GenerateMikrotikEntityMetadataAttribute : Attribute
    // { }
    /// <summary>
    /// Gets the [GenerateMikrotikEntityMetadata] attribute source code. This is a marker attribute, which marks a type for
    /// introspection and proxy generation.
    /// </summary>
    public static CompilationUnitSyntax GenerateMikrotikEntityMetadataAttribute = SyntaxFactory.CompilationUnit()
        .WithUsings(
            SyntaxFactory.SingletonList(
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.IdentifierName("System"))))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.FileScopedNamespaceDeclaration(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName(GeneratedNamespace[0]),
                                SyntaxFactory.IdentifierName(GeneratedNamespace[1])),
                            SyntaxFactory.IdentifierName(GeneratedNamespace[2])))
                    .WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.ClassDeclaration(GenerateAttributeName)
                                .WithAttributeLists(
                                    SyntaxFactory.SingletonList(
                                        SyntaxFactory.AttributeList(
                                            SyntaxFactory.SingletonSeparatedList(
                                                SyntaxFactory.Attribute(
                                                        SyntaxFactory.IdentifierName("AttributeUsage"))
                                                    .WithArgumentList(
                                                        SyntaxFactory.AttributeArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.AttributeArgument(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName("AttributeTargets"),
                                                                        SyntaxFactory.IdentifierName("Class"))))))))))
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.SealedKeyword) }))
                                .WithBaseList(
                                    SyntaxFactory.BaseList(
                                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                                            SyntaxFactory.SimpleBaseType(
                                                SyntaxFactory.IdentifierName("Attribute")))))))))
        .NormalizeWhitespace();

    // using System;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal static partial class EntityProxies
    // { }
    /// <summary>
    /// Gets the EntityProxies static class source code. This is a partial class, to which all the proxy getters will be
    /// attached.
    /// </summary>
    public static CompilationUnitSyntax EntityProxiesClass = SyntaxFactory.CompilationUnit()
        .WithUsings(
            SyntaxFactory.SingletonList(
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.IdentifierName("System"))))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.FileScopedNamespaceDeclaration(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName(GeneratedNamespace[0]),
                                SyntaxFactory.IdentifierName(GeneratedNamespace[1])),
                            SyntaxFactory.IdentifierName(GeneratedNamespace[2])))
                    .WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.ClassDeclaration(EntityProxiesClassName)
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        new[] { SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword) }))))))
        .NormalizeWhitespace();

    // using System;
    // using System.Collections.Generic;
    // using System.Linq.Expressions;
    // using System.Reflection;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal static partial class EntityProxies
    // {
    //     private static readonly IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<ENTITY>> _propertiesQUALIFIER = new Dictionary<string, IMikrotikEntityProxyGetterSetter<ENTITY>>()
    //     {
    //         ["PROPNAME"] = new MikrotikEntityProxyGetterSetter<ENTITY, PROPTYPE>(static x => x.PROPNAME, static (x, v) => x.PROPNAME = v),
    //     };
    //
    //     private static readonly IReadOnlyDictionary<string, string> _unmapQUALIFIER = new Dictionary<string, string>()
    //     {
    //         ["PROPNAME"] = "PROPMEMBER",
    //     };
    //
    //     private static readonly IReadOnlyDictionary<string, string> _mapQUALIFIER = new Dictionary<string, string>()
    //     {
    //         ["PROPMEMBER"] = "PROPNAME",
    //     };
    //
    //     public static IMikrotikEntityProxy GetProxy(this ENTITY entity)
    //         => new MikrotikEntityProxy<ENTITY>(entity, _propertiesQUALIFIER);
    //
    //     public static string MapToSerialized<T>(this ENTITY entity, Expression<Func<ENTITY, T>> prop)
    //         => prop.Body is MemberExpression { Member: PropertyInfo property }
    //         ? (_mapQUALIFIER.TryGetValue(property.Name, out var serialized)
    //             ? serialized
    //             : null)
    //         : null;
    //
    //     public static string MapFromSerialized(this ENTITY entity, string serialized)
    //         => _unmapQUALIFIER.TryGetValue(serialized, out var name)
    //         ? name
    //         : null;
    // }
    /// <summary>
    /// Generates an entity proxy for a given entity.
    /// </summary>
    /// <param name="metadata">Metadata of the entity to generate a proxy for.</param>
    /// <returns>Generated entity proxy.</returns>
    public static CompilationUnitSyntax GenerateEntityProxyStatic(EntityMetadata metadata)
        => SyntaxFactory.CompilationUnit()
            .WithUsings(
                SyntaxFactory.List(
                [
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.IdentifierName("System")),
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName("System"),
                                SyntaxFactory.IdentifierName("Collections")),
                            SyntaxFactory.IdentifierName("Generic"))),
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName("System"),
                                SyntaxFactory.IdentifierName("Linq")),
                            SyntaxFactory.IdentifierName("Expressions"))),
                    SyntaxFactory.UsingDirective(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("System"),
                            SyntaxFactory.IdentifierName("Reflection")))
                ]))
            .WithMembers(
                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                    SyntaxFactory.FileScopedNamespaceDeclaration(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.QualifiedName(
                                    SyntaxFactory.IdentifierName(GeneratedNamespace[0]),
                                    SyntaxFactory.IdentifierName(GeneratedNamespace[1])),
                                SyntaxFactory.IdentifierName(GeneratedNamespace[2])))
                        .WithMembers(
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                SyntaxFactory.ClassDeclaration(EntityProxiesClassName)
                                    .WithModifiers(
                                        SyntaxFactory.TokenList(
                                            new[] { SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword) }))
                                    .WithMembers(
                                        SyntaxFactory.List<MemberDeclarationSyntax>(
                                        [
                                            SyntaxFactory.FieldDeclaration(
                                                    SyntaxFactory.VariableDeclaration(
                                                            SyntaxFactory.GenericName(
                                                                    SyntaxFactory.Identifier("IReadOnlyDictionary"))
                                                                .WithTypeArgumentList(
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.PredefinedType(
                                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.GenericName(
                                                                                        SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                                                    .WithTypeArgumentList(
                                                                                        SyntaxFactory.TypeArgumentList(
                                                                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                                SyntaxFactory.IdentifierName(metadata.QualifiedName))))
                                                                            }))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_properties" + CreateQualifier(metadata.QualifiedName)))
                                                                    .WithInitializer(
                                                                        SyntaxFactory.EqualsValueClause(
                                                                            SyntaxFactory.ObjectCreationExpression(
                                                                                    SyntaxFactory.GenericName(
                                                                                            SyntaxFactory.Identifier("Dictionary"))
                                                                                        .WithTypeArgumentList(
                                                                                            SyntaxFactory.TypeArgumentList(
                                                                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                                    new SyntaxNodeOrToken[]
                                                                                                    {
                                                                                                        SyntaxFactory.PredefinedType(
                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.GenericName(
                                                                                                                SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                                                                            .WithTypeArgumentList(
                                                                                                                SyntaxFactory.TypeArgumentList(
                                                                                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                                                        SyntaxFactory.IdentifierName(metadata.QualifiedName))))
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadata.Members.SelectMany(x => GenerateEntityPropertyProxyStatic(metadata, x))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[]
                                                        {
                                                            SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                        })),
                                            SyntaxFactory.FieldDeclaration(
                                                    SyntaxFactory.VariableDeclaration(
                                                            SyntaxFactory.GenericName(
                                                                    SyntaxFactory.Identifier("IReadOnlyDictionary"))
                                                                .WithTypeArgumentList(
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.PredefinedType(
                                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.PredefinedType(
                                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword))
                                                                            }))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_unmap" + CreateQualifier(metadata.QualifiedName)))
                                                                    .WithInitializer(
                                                                        SyntaxFactory.EqualsValueClause(
                                                                            SyntaxFactory.ObjectCreationExpression(
                                                                                    SyntaxFactory.GenericName(
                                                                                            SyntaxFactory.Identifier("Dictionary"))
                                                                                        .WithTypeArgumentList(
                                                                                            SyntaxFactory.TypeArgumentList(
                                                                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                                    new SyntaxNodeOrToken[]
                                                                                                    {
                                                                                                        SyntaxFactory.PredefinedType(
                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.PredefinedType(
                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword))
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadata.Members.SelectMany(x => GenerateUnmap(x))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[]
                                                        {
                                                            SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                        })),
                                            SyntaxFactory.FieldDeclaration(
                                                    SyntaxFactory.VariableDeclaration(
                                                            SyntaxFactory.GenericName(
                                                                    SyntaxFactory.Identifier("IReadOnlyDictionary"))
                                                                .WithTypeArgumentList(
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.PredefinedType(
                                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.PredefinedType(
                                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword))
                                                                            }))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_map" + CreateQualifier(metadata.QualifiedName)))
                                                                    .WithInitializer(
                                                                        SyntaxFactory.EqualsValueClause(
                                                                            SyntaxFactory.ObjectCreationExpression(
                                                                                    SyntaxFactory.GenericName(
                                                                                            SyntaxFactory.Identifier("Dictionary"))
                                                                                        .WithTypeArgumentList(
                                                                                            SyntaxFactory.TypeArgumentList(
                                                                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                                    new SyntaxNodeOrToken[]
                                                                                                    {
                                                                                                        SyntaxFactory.PredefinedType(
                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.PredefinedType(
                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword))
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadata.Members.SelectMany(x => GenerateMap(x))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[]
                                                        {
                                                            SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                        })),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.IdentifierName(ProxyInterfaceName),
                                                    SyntaxFactory.Identifier("GetProxy"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword) }))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("entity"))
                                                                .WithModifiers(
                                                                    SyntaxFactory.TokenList(
                                                                        SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName(metadata.QualifiedName)))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ObjectCreationExpression(
                                                                SyntaxFactory.GenericName(
                                                                        SyntaxFactory.Identifier(ProxyImplementationName))
                                                                    .WithTypeArgumentList(
                                                                        SyntaxFactory.TypeArgumentList(
                                                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                SyntaxFactory.IdentifierName(metadata.QualifiedName)))))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("entity")),
                                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("_properties" + CreateQualifier(metadata.QualifiedName)))
                                                                        })))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword) }))
                                                .WithTypeParameterList(
                                                    SyntaxFactory.TypeParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.TypeParameter(
                                                                SyntaxFactory.Identifier("T")))))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("entity"))
                                                                    .WithModifiers(
                                                                        SyntaxFactory.TokenList(
                                                                            SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName(metadata.QualifiedName)),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("prop"))
                                                                    .WithType(
                                                                        SyntaxFactory.GenericName(
                                                                                SyntaxFactory.Identifier("Expression"))
                                                                            .WithTypeArgumentList(
                                                                                SyntaxFactory.TypeArgumentList(
                                                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                        SyntaxFactory.GenericName(
                                                                                                SyntaxFactory.Identifier("Func"))
                                                                                            .WithTypeArgumentList(
                                                                                                SyntaxFactory.TypeArgumentList(
                                                                                                    SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                                        new SyntaxNodeOrToken[]
                                                                                                        {
                                                                                                            SyntaxFactory.IdentifierName(metadata.QualifiedName),
                                                                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName("T")
                                                                                                        })))))))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ConditionalExpression(
                                                            SyntaxFactory.IsPatternExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("prop"),
                                                                    SyntaxFactory.IdentifierName("Body")),
                                                                SyntaxFactory.RecursivePattern()
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("MemberExpression"))
                                                                    .WithPropertyPatternClause(
                                                                        SyntaxFactory.PropertyPatternClause(
                                                                            SyntaxFactory.SingletonSeparatedList(
                                                                                SyntaxFactory.Subpattern(
                                                                                        SyntaxFactory.DeclarationPattern(
                                                                                            SyntaxFactory.IdentifierName("PropertyInfo"),
                                                                                            SyntaxFactory.SingleVariableDesignation(
                                                                                                SyntaxFactory.Identifier(
                                                                                                    SyntaxFactory.TriviaList(),
                                                                                                    SyntaxKind.PropertyKeyword,
                                                                                                    "property",
                                                                                                    "property",
                                                                                                    SyntaxFactory.TriviaList()))))
                                                                                    .WithNameColon(
                                                                                        SyntaxFactory.NameColon(
                                                                                            SyntaxFactory.IdentifierName("Member"))))))),
                                                            SyntaxFactory.ParenthesizedExpression(
                                                                SyntaxFactory.ConditionalExpression(
                                                                    SyntaxFactory.InvocationExpression(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.IdentifierName("_map" + CreateQualifier(metadata.QualifiedName)),
                                                                                SyntaxFactory.IdentifierName("TryGetValue")))
                                                                        .WithArgumentList(
                                                                            SyntaxFactory.ArgumentList(
                                                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                                    new SyntaxNodeOrToken[]
                                                                                    {
                                                                                        SyntaxFactory.Argument(
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.IdentifierName(
                                                                                                    SyntaxFactory.Identifier(
                                                                                                        SyntaxFactory.TriviaList(),
                                                                                                        SyntaxKind.PropertyKeyword,
                                                                                                        "property",
                                                                                                        "property",
                                                                                                        SyntaxFactory.TriviaList())),
                                                                                                SyntaxFactory.IdentifierName("Name"))),
                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                                SyntaxFactory.DeclarationExpression(
                                                                                                    SyntaxFactory.IdentifierName(
                                                                                                        SyntaxFactory.Identifier(
                                                                                                            SyntaxFactory.TriviaList(),
                                                                                                            SyntaxKind.VarKeyword,
                                                                                                            "var",
                                                                                                            "var",
                                                                                                            SyntaxFactory.TriviaList())),
                                                                                                    SyntaxFactory.SingleVariableDesignation(
                                                                                                        SyntaxFactory.Identifier("serialized"))))
                                                                                            .WithRefOrOutKeyword(
                                                                                                SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                                    }))),
                                                                    SyntaxFactory.IdentifierName("serialized"),
                                                                    SyntaxFactory.LiteralExpression(
                                                                        SyntaxKind.NullLiteralExpression))),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapFromSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword) }))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("entity"))
                                                                    .WithModifiers(
                                                                        SyntaxFactory.TokenList(
                                                                            SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName(metadata.QualifiedName)),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("serialized"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ConditionalExpression(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName("_unmap" + CreateQualifier(metadata.QualifiedName)),
                                                                        SyntaxFactory.IdentifierName("TryGetValue")))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.IdentifierName("serialized")),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                        SyntaxFactory.DeclarationExpression(
                                                                                            SyntaxFactory.IdentifierName(
                                                                                                SyntaxFactory.Identifier(
                                                                                                    SyntaxFactory.TriviaList(),
                                                                                                    SyntaxKind.VarKeyword,
                                                                                                    "var",
                                                                                                    "var",
                                                                                                    SyntaxFactory.TriviaList())),
                                                                                            SyntaxFactory.SingleVariableDesignation(
                                                                                                SyntaxFactory.Identifier("name"))))
                                                                                    .WithRefOrOutKeyword(
                                                                                        SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                            }))),
                                                            SyntaxFactory.IdentifierName("name"),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                        ]))))))
            .NormalizeWhitespace();

    // ["PROPNAME"] = new MikrotikEntityProxyGetterSetter<ENTITY, PROPTYPE>(static x => x.PROPNAME, static (x, v) => x.PROPNAME = v),
    /// <summary>
    /// Generates dictionary entry for entity member proxy data.
    /// </summary>
    /// <param name="entity">Entity to generate for.</param>
    /// <param name="member">Member to generate.</param>
    /// <returns>Generated dictionary entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateEntityPropertyProxyStatic(in EntityMetadata entity, in EntityMemberMetadata member)
        =>
        [
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.ImplicitElementAccess()
                    .WithArgumentList(
                        SyntaxFactory.BracketedArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(member.SerializedName)))))),
                SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier(ProxyGetterSetterImplementationName))
                            .WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.SeparatedList<TypeSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            SyntaxFactory.IdentifierName(entity.QualifiedName), SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName(member.Type)
                                        }))))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.SimpleLambdaExpression(
                                                SyntaxFactory.Parameter(
                                                    SyntaxFactory.Identifier("x")))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                            .WithExpressionBody(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName("x"),
                                                    SyntaxFactory.IdentifierName(member.Name)))),
                                    SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                        SyntaxFactory.ParenthesizedLambdaExpression()
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("x")),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("v"))
                                                        })))
                                            .WithExpressionBody(
                                                SyntaxFactory.AssignmentExpression(
                                                    SyntaxKind.SimpleAssignmentExpression,
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName("x"),
                                                        SyntaxFactory.IdentifierName(member.Name)),
                                                    SyntaxFactory.IdentifierName("v"))))
                                })))),
            SyntaxFactory.Token(SyntaxKind.CommaToken)
        ];

    // ["PROPNAME"] = "PROPMEMBER",
    /// <summary>
    /// Generates dictionary entry serialized -> member name map.
    /// </summary>
    /// <param name="member">Member to generate.</param>
    /// <returns>Generated dictionary entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateUnmap(in EntityMemberMetadata member)
        =>
        [
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.ImplicitElementAccess()
                    .WithArgumentList(
                        SyntaxFactory.BracketedArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(member.SerializedName)))))),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(member.Name))),
            SyntaxFactory.Token(SyntaxKind.CommaToken)
        ];

    // ["PROPMEMBER"] = "PROPNAME",
    /// <summary>
    /// Generates dictionary entry member name -> serialized map.
    /// </summary>
    /// <param name="member">Member to generate.</param>
    /// <returns>Generated dictionary entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateMap(in EntityMemberMetadata member)
        =>
        [
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.ImplicitElementAccess()
                    .WithArgumentList(
                        SyntaxFactory.BracketedArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(member.Name)))))),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(member.SerializedName))),
            SyntaxFactory.Token(SyntaxKind.CommaToken)
        ];

    // using System;
    // using System.Collections.Generic;
    // using Emzi0767.NetworkSelfService.Mikrotik.Entities;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal interface IMikrotikEntityProxyGetterSetter<T>
    //     where T : IMikrotikEntity
    // {
    //     object Get(T @this);
    //     void Set(T @this, object value);
    // }
    //
    // internal readonly struct MikrotikEntityProxyGetterSetter<T, TProp> : IMikrotikEntityProxyGetterSetter<T>
    //     where T : IMikrotikEntity
    // {
    //     public Func<T, TProp> Getter { get; }
    //     public Action<T, TProp> Setter { get; }
    //     public MikrotikEntityProxyGetterSetter(Func<T, TProp> getter, Action<T, TProp> setter)
    //     {
    //         this.Getter = getter;
    //         this.Setter = setter;
    //     }
    //     object IMikrotikEntityProxyGetterSetter<T>.Get(T @this) => this.Getter(@this);
    //     void IMikrotikEntityProxyGetterSetter<T>.Set(T @this, object value) => this.Setter(@this, (TProp)value);
    // }
    //
    // internal interface IMikrotikEntityProxy
    // {
    //     object Get(string name);
    //     void Set(string name, object value);
    // }
    //
    // internal readonly struct MikrotikEntityProxy<T> : IMikrotikEntityProxy
    //     where T : IMikrotikEntity
    // {
    //     private T This { get; }
    //     private IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<T>> PropertyProxies { get; }
    //     public MikrotikEntityProxy(T @this, IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<T>> propertyProxies)
    //     {
    //         this.This = @this;
    //         this.PropertyProxies = propertyProxies;
    //     }
    //     public object GetProperty(string name)
    //     {
    //         return this.PropertyProxies.TryGetValue(name, out var proxy)
    //             ? proxy.Get(this.This)
    //             : null;
    //     }
    //     public void SetProperty(string name, object value)
    //     {
    //         if (this.PropertyProxies.TryGetValue(name, out var proxy))
    //             proxy.Set(this.This, value);
    //     }
    //     object IMikrotikEntityProxy.Get(string name) => this.GetProperty(name);
    //     void IMikrotikEntityProxy.Set(string name, object value) => this.SetProperty(name, value);
    // }
    /// <summary>
    /// Gets the IMikrotikEntityProxy implementation source code. This is an interface which all proxies have to implement.
    /// </summary>
    public static CompilationUnitSyntax MikrotikEntityProxyImplementation = SyntaxFactory.CompilationUnit()
        .WithUsings(
            SyntaxFactory.List(
            [
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.IdentifierName("System")),
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("System"),
                            SyntaxFactory.IdentifierName("Collections")),
                        SyntaxFactory.IdentifierName("Generic"))),
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName(GeneratedNamespace[0]),
                                SyntaxFactory.IdentifierName(GeneratedNamespace[1])),
                            SyntaxFactory.IdentifierName(GeneratedNamespace[2])),
                        SyntaxFactory.IdentifierName("Entities")))
            ]))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.FileScopedNamespaceDeclaration(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName(GeneratedNamespace[0]),
                                SyntaxFactory.IdentifierName(GeneratedNamespace[1])),
                            SyntaxFactory.IdentifierName(GeneratedNamespace[2])))
                    .WithMembers(
                        SyntaxFactory.List<MemberDeclarationSyntax>(
                        [
                            SyntaxFactory.InterfaceDeclaration(ProxyGetterSetterInterfaceName)
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        SyntaxFactory.Token(SyntaxKind.InternalKeyword)))
                                .WithTypeParameterList(
                                    SyntaxFactory.TypeParameterList(
                                        SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.TypeParameter(
                                                SyntaxFactory.Identifier("T")))))
                                .WithConstraintClauses(
                                    SyntaxFactory.SingletonList(
                                        SyntaxFactory.TypeParameterConstraintClause(
                                                SyntaxFactory.IdentifierName("T"))
                                            .WithConstraints(
                                                SyntaxFactory.SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                                    SyntaxFactory.TypeConstraint(
                                                        SyntaxFactory.IdentifierName("IMikrotikEntity"))))))
                                .WithMembers(
                                    SyntaxFactory.List<MemberDeclarationSyntax>(
                                    [
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                SyntaxFactory.Identifier("Get"))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("@this"))
                                                            .WithType(
                                                                SyntaxFactory.IdentifierName("T")))))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                                SyntaxFactory.Identifier("Set"))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("@this"))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName("T")),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("value"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                        })))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                    ])),
                            SyntaxFactory.StructDeclaration(ProxyGetterSetterImplementationName)
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        new[] { SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword) }))
                                .WithTypeParameterList(
                                    SyntaxFactory.TypeParameterList(
                                        SyntaxFactory.SeparatedList<TypeParameterSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                SyntaxFactory.TypeParameter(
                                                    SyntaxFactory.Identifier("T")),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeParameter(
                                                    SyntaxFactory.Identifier("TProp"))
                                            })))
                                .WithBaseList(
                                    SyntaxFactory.BaseList(
                                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                                            SyntaxFactory.SimpleBaseType(
                                                SyntaxFactory.GenericName(
                                                        SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                SyntaxFactory.IdentifierName("T"))))))))
                                .WithConstraintClauses(
                                    SyntaxFactory.SingletonList(
                                        SyntaxFactory.TypeParameterConstraintClause(
                                                SyntaxFactory.IdentifierName("T"))
                                            .WithConstraints(
                                                SyntaxFactory.SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                                    SyntaxFactory.TypeConstraint(
                                                        SyntaxFactory.IdentifierName("IMikrotikEntity"))))))
                                .WithMembers(
                                    SyntaxFactory.List<MemberDeclarationSyntax>(
                                    [
                                        SyntaxFactory.PropertyDeclaration(
                                                SyntaxFactory.GenericName(
                                                        SyntaxFactory.Identifier("Func"))
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.IdentifierName("T"), SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName("TProp")
                                                                }))),
                                                SyntaxFactory.Identifier("Getter"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithAccessorList(
                                                SyntaxFactory.AccessorList(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.AccessorDeclaration(
                                                                SyntaxKind.GetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))))),
                                        SyntaxFactory.PropertyDeclaration(
                                                SyntaxFactory.GenericName(
                                                        SyntaxFactory.Identifier("Action"))
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.IdentifierName("T"), SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName("TProp")
                                                                }))),
                                                SyntaxFactory.Identifier("Setter"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithAccessorList(
                                                SyntaxFactory.AccessorList(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.AccessorDeclaration(
                                                                SyntaxKind.GetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))))),
                                        SyntaxFactory.ConstructorDeclaration(
                                                SyntaxFactory.Identifier(ProxyGetterSetterImplementationName))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("getter"))
                                                                .WithType(
                                                                    SyntaxFactory.GenericName(
                                                                            SyntaxFactory.Identifier("Func"))
                                                                        .WithTypeArgumentList(
                                                                            SyntaxFactory.TypeArgumentList(
                                                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                    new SyntaxNodeOrToken[]
                                                                                    {
                                                                                        SyntaxFactory.IdentifierName("T"), SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                                        SyntaxFactory.IdentifierName("TProp")
                                                                                    })))),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("setter"))
                                                                .WithType(
                                                                    SyntaxFactory.GenericName(
                                                                            SyntaxFactory.Identifier("Action"))
                                                                        .WithTypeArgumentList(
                                                                            SyntaxFactory.TypeArgumentList(
                                                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                    new SyntaxNodeOrToken[]
                                                                                    {
                                                                                        SyntaxFactory.IdentifierName("T"), SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                                        SyntaxFactory.IdentifierName("TProp")
                                                                                    }))))
                                                        })))
                                            .WithBody(
                                                SyntaxFactory.Block(
                                                    SyntaxFactory.ExpressionStatement(
                                                        SyntaxFactory.AssignmentExpression(
                                                            SyntaxKind.SimpleAssignmentExpression,
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("Getter")),
                                                            SyntaxFactory.IdentifierName("getter"))),
                                                    SyntaxFactory.ExpressionStatement(
                                                        SyntaxFactory.AssignmentExpression(
                                                            SyntaxKind.SimpleAssignmentExpression,
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("Setter")),
                                                            SyntaxFactory.IdentifierName("setter"))))),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                SyntaxFactory.Identifier("Get"))
                                            .WithExplicitInterfaceSpecifier(
                                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.IdentifierName("T"))))))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("@this"))
                                                            .WithType(
                                                                SyntaxFactory.IdentifierName("T")))))
                                            .WithExpressionBody(
                                                SyntaxFactory.ArrowExpressionClause(
                                                    SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("Getter")))
                                                        .WithArgumentList(
                                                            SyntaxFactory.ArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList(
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.IdentifierName("@this")))))))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                                SyntaxFactory.Identifier("Set"))
                                            .WithExplicitInterfaceSpecifier(
                                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.IdentifierName("T"))))))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("@this"))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName("T")),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("value"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                        })))
                                            .WithExpressionBody(
                                                SyntaxFactory.ArrowExpressionClause(
                                                    SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("Setter")))
                                                        .WithArgumentList(
                                                            SyntaxFactory.ArgumentList(
                                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.IdentifierName("@this")),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                            SyntaxFactory.CastExpression(
                                                                                SyntaxFactory.IdentifierName("TProp"),
                                                                                SyntaxFactory.IdentifierName("value")))
                                                                    })))))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                    ])),
                            SyntaxFactory.InterfaceDeclaration(ProxyInterfaceName)
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        SyntaxFactory.Token(SyntaxKind.InternalKeyword)))
                                .WithMembers(
                                    SyntaxFactory.List<MemberDeclarationSyntax>(
                                    [
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                SyntaxFactory.Identifier("Get"))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("name"))
                                                            .WithType(
                                                                SyntaxFactory.PredefinedType(
                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                                SyntaxFactory.Identifier("Set"))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("name"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("value"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                        })))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                    ])),
                            SyntaxFactory.StructDeclaration(ProxyImplementationName)
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        new[] { SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword) }))
                                .WithTypeParameterList(
                                    SyntaxFactory.TypeParameterList(
                                        SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.TypeParameter(
                                                SyntaxFactory.Identifier("T")))))
                                .WithBaseList(
                                    SyntaxFactory.BaseList(
                                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                                            SyntaxFactory.SimpleBaseType(
                                                SyntaxFactory.IdentifierName(ProxyInterfaceName)))))
                                .WithConstraintClauses(
                                    SyntaxFactory.SingletonList(
                                        SyntaxFactory.TypeParameterConstraintClause(
                                                SyntaxFactory.IdentifierName("T"))
                                            .WithConstraints(
                                                SyntaxFactory.SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                                    SyntaxFactory.TypeConstraint(
                                                        SyntaxFactory.IdentifierName("IMikrotikEntity"))))))
                                .WithMembers(
                                    SyntaxFactory.List<MemberDeclarationSyntax>(
                                    [
                                        SyntaxFactory.PropertyDeclaration(
                                                SyntaxFactory.IdentifierName("T"),
                                                SyntaxFactory.Identifier("This"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                                            .WithAccessorList(
                                                SyntaxFactory.AccessorList(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.AccessorDeclaration(
                                                                SyntaxKind.GetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))))),
                                        SyntaxFactory.PropertyDeclaration(
                                                SyntaxFactory.GenericName(
                                                        SyntaxFactory.Identifier("IReadOnlyDictionary"))
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.GenericName(
                                                                            SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                                        .WithTypeArgumentList(
                                                                            SyntaxFactory.TypeArgumentList(
                                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                    SyntaxFactory.IdentifierName("T"))))
                                                                }))),
                                                SyntaxFactory.Identifier("PropertyProxies"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                                            .WithAccessorList(
                                                SyntaxFactory.AccessorList(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.AccessorDeclaration(
                                                                SyntaxKind.GetAccessorDeclaration)
                                                            .WithSemicolonToken(
                                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))))),
                                        SyntaxFactory.ConstructorDeclaration(
                                                SyntaxFactory.Identifier(ProxyImplementationName))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("@this"))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName("T")),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("propertyProxies"))
                                                                .WithType(
                                                                    SyntaxFactory.GenericName(
                                                                            SyntaxFactory.Identifier("IReadOnlyDictionary"))
                                                                        .WithTypeArgumentList(
                                                                            SyntaxFactory.TypeArgumentList(
                                                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                                    new SyntaxNodeOrToken[]
                                                                                    {
                                                                                        SyntaxFactory.PredefinedType(
                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.GenericName(
                                                                                                SyntaxFactory.Identifier(ProxyGetterSetterInterfaceName))
                                                                                            .WithTypeArgumentList(
                                                                                                SyntaxFactory.TypeArgumentList(
                                                                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                                        SyntaxFactory.IdentifierName("T"))))
                                                                                    }))))
                                                        })))
                                            .WithBody(
                                                SyntaxFactory.Block(
                                                    SyntaxFactory.ExpressionStatement(
                                                        SyntaxFactory.AssignmentExpression(
                                                            SyntaxKind.SimpleAssignmentExpression,
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("This")),
                                                            SyntaxFactory.IdentifierName("@this"))),
                                                    SyntaxFactory.ExpressionStatement(
                                                        SyntaxFactory.AssignmentExpression(
                                                            SyntaxKind.SimpleAssignmentExpression,
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("PropertyProxies")),
                                                            SyntaxFactory.IdentifierName("propertyProxies"))))),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                SyntaxFactory.Identifier("GetProperty"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("name"))
                                                            .WithType(
                                                                SyntaxFactory.PredefinedType(
                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                                            .WithBody(
                                                SyntaxFactory.Block(
                                                    SyntaxFactory.SingletonList<StatementSyntax>(
                                                        SyntaxFactory.ReturnStatement(
                                                            SyntaxFactory.ConditionalExpression(
                                                                SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.ThisExpression(),
                                                                                SyntaxFactory.IdentifierName("PropertyProxies")),
                                                                            SyntaxFactory.IdentifierName("TryGetValue")))
                                                                    .WithArgumentList(
                                                                        SyntaxFactory.ArgumentList(
                                                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]
                                                                                {
                                                                                    SyntaxFactory.Argument(
                                                                                        SyntaxFactory.IdentifierName("name")),
                                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                            SyntaxFactory.DeclarationExpression(
                                                                                                SyntaxFactory.IdentifierName(
                                                                                                    SyntaxFactory.Identifier(
                                                                                                        SyntaxFactory.TriviaList(),
                                                                                                        SyntaxKind.VarKeyword,
                                                                                                        "var",
                                                                                                        "var",
                                                                                                        SyntaxFactory.TriviaList())),
                                                                                                SyntaxFactory.SingleVariableDesignation(
                                                                                                    SyntaxFactory.Identifier("proxy"))))
                                                                                        .WithRefOrOutKeyword(
                                                                                            SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                                }))),
                                                                SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName("proxy"),
                                                                            SyntaxFactory.IdentifierName("Get")))
                                                                    .WithArgumentList(
                                                                        SyntaxFactory.ArgumentList(
                                                                            SyntaxFactory.SingletonSeparatedList(
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.MemberAccessExpression(
                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                        SyntaxFactory.ThisExpression(),
                                                                                        SyntaxFactory.IdentifierName("This")))))),
                                                                SyntaxFactory.LiteralExpression(
                                                                    SyntaxKind.NullLiteralExpression)))))),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                                SyntaxFactory.Identifier("SetProperty"))
                                            .WithModifiers(
                                                SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("name"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("value"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                        })))
                                            .WithBody(
                                                SyntaxFactory.Block(
                                                    SyntaxFactory.SingletonList<StatementSyntax>(
                                                        SyntaxFactory.IfStatement(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.ThisExpression(),
                                                                            SyntaxFactory.IdentifierName("PropertyProxies")),
                                                                        SyntaxFactory.IdentifierName("TryGetValue")))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.IdentifierName("name")),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                        SyntaxFactory.DeclarationExpression(
                                                                                            SyntaxFactory.IdentifierName(
                                                                                                SyntaxFactory.Identifier(
                                                                                                    SyntaxFactory.TriviaList(),
                                                                                                    SyntaxKind.VarKeyword,
                                                                                                    "var",
                                                                                                    "var",
                                                                                                    SyntaxFactory.TriviaList())),
                                                                                            SyntaxFactory.SingleVariableDesignation(
                                                                                                SyntaxFactory.Identifier("proxy"))))
                                                                                    .WithRefOrOutKeyword(
                                                                                        SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                            }))),
                                                            SyntaxFactory.ExpressionStatement(
                                                                SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName("proxy"),
                                                                            SyntaxFactory.IdentifierName("Set")))
                                                                    .WithArgumentList(
                                                                        SyntaxFactory.ArgumentList(
                                                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]
                                                                                {
                                                                                    SyntaxFactory.Argument(
                                                                                        SyntaxFactory.MemberAccessExpression(
                                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                                            SyntaxFactory.ThisExpression(),
                                                                                            SyntaxFactory.IdentifierName("This"))),
                                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                        SyntaxFactory.IdentifierName("value"))
                                                                                })))))))),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                SyntaxFactory.Identifier("Get"))
                                            .WithExplicitInterfaceSpecifier(
                                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                                    SyntaxFactory.IdentifierName(ProxyInterfaceName)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Parameter(
                                                                SyntaxFactory.Identifier("name"))
                                                            .WithType(
                                                                SyntaxFactory.PredefinedType(
                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                                            .WithExpressionBody(
                                                SyntaxFactory.ArrowExpressionClause(
                                                    SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("GetProperty")))
                                                        .WithArgumentList(
                                                            SyntaxFactory.ArgumentList(
                                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.IdentifierName("name"))
                                                                    })))))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                        SyntaxFactory.MethodDeclaration(
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                                SyntaxFactory.Identifier("Set"))
                                            .WithExplicitInterfaceSpecifier(
                                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                                    SyntaxFactory.IdentifierName(ProxyInterfaceName)))
                                            .WithParameterList(
                                                SyntaxFactory.ParameterList(
                                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("name"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("value"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                        })))
                                            .WithExpressionBody(
                                                SyntaxFactory.ArrowExpressionClause(
                                                    SyntaxFactory.InvocationExpression(
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.ThisExpression(),
                                                                SyntaxFactory.IdentifierName("SetProperty")))
                                                        .WithArgumentList(
                                                            SyntaxFactory.ArgumentList(
                                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.IdentifierName("name")),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                            SyntaxFactory.IdentifierName("value"))
                                                                    })))))
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                    ]))
                        ]))))
        .NormalizeWhitespace();
}
