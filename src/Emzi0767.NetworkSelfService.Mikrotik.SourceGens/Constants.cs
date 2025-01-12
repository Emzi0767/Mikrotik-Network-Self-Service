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

// for every $1 donated, I plant another syntax tree

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
    /// Gets the name of the attribute which marks enums to generate proxies for.
    /// </summary>
    public const string GenerateEnumAttributeName = "GenerateEnumMetadataAttribute";

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
    /// Gets the name of the MikrotikEntity attribute.
    /// </summary>
    public const string EntityAttributeQualifiedName = "Emzi0767.NetworkSelfService.Mikrotik.Attributes.MikrotikEntityAttribute";

    /// <summary>
    /// Gets the name of the entity proxy static class.
    /// </summary>
    public const string EntityProxiesClassName = "EntityProxies";

    /// <summary>
    /// Gets the name of the enum proxy static class.
    /// </summary>
    public const string EnumProxiesClassName = "EnumProxies";

    /// <summary>
    /// Gets the namespace the generated code is in.
    /// </summary>
    public static string[] GeneratedNamespace { get; } = ["Emzi0767", "NetworkSelfService", "Mikrotik"];

    /// <summary>
    /// Gets the full qualified name of the generate attribute.
    /// </summary>
    public static string GenerateAttributeQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + GenerateAttributeName;

    /// <summary>
    /// Gets the full qualified name of the generate enum attribute.
    /// </summary>
    public static string GenerateEnumAttributeQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + GenerateEnumAttributeName;

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
    /// Gets the full qualified name of the entity proxy static class.
    /// </summary>
    public static string EnumProxiesClassQualifiedName { get; } = string.Join(".", GeneratedNamespace) + "." + EnumProxiesClassName;

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
                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.SealedKeyword)))
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
    // [AttributeUsage(AttributeTargets.Enum)]
    // public sealed class GenerateEnumMetadataAttribute : Attribute
    // { }
    /// <summary>
    /// Gets the [GenerateEnumMetadata] attribute source code. This is a marker attribute, which marks a type for
    /// introspection and proxy generation.
    /// </summary>
    public static CompilationUnitSyntax GenerateEnumMetadataAttribute = SyntaxFactory.CompilationUnit()
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
                            SyntaxFactory.ClassDeclaration("GenerateEnumMetadataAttribute")
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
                                                                        SyntaxFactory.IdentifierName("Enum"))))))))))
                                .WithModifiers(
                                    SyntaxFactory.TokenList(
                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.SealedKeyword)]))
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
                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                        SyntaxFactory.Token(SyntaxKind.PartialKeyword)))))))
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
    //     private static readonly IReadOnlyDictionary<string, IMikrotikEntityProxyGetterSetter<ENTITY>> _proxiesQUALIFIER = new Dictionary<string, IMikrotikEntityProxyGetterSetter<ENTITY>>()
    //     {
    //         ["PROPMEMBER"] = new MikrotikEntityProxyGetterSetter<ENTITY, PROPTYPE>(static x => x.PROPMEMBER, static (x, v) => x.PROPMEMBER = v),
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
    //     private static readonly IReadOnlyDictionary<string, Type> _typesQUALIFIER = new Dictionary<string, Type>()
    //     {
    //         ["PROPMEMBER"] = typeof(PROPTYPE),
    //     };
    //
    //     private static readonly string[] _pathQUALIFIER = [ "PATH1", ];
    //
    //     private static readonly string[] _propertiesQUALIFIER = [ "PROPMEMBER", ];
    //
    //     public static IMikrotikEntityProxy GetProxy(this ENTITY entity)
    //         => new MikrotikEntityProxy<ENTITY>(entity, _proxiesQUALIFIER);
    //
    //     public static string MapToSerialized<T>(this ENTITY entity, Expression<Func<ENTITY, T>> prop)
    //         => prop.Body is MemberExpression member
    //         ? entity.MapToSerialized(member)
    //         : null;
    //
    //     public static string MapToSerialized(this ENTITY entity, MemberExpression member)
    //         => member is { Member: PropertyInfo property }
    //         ? (_mapQUALIFIER.TryGetValue(property.Name, out var serialized)
    //             ? serialized
    //             : null)
    //         : null;
    //
    //     public static string MapFromSerialized(this ENTITY entity, string serialized)
    //         => _unmapQUALIFIER.TryGetValue(serialized, out var name)
    //         ? name
    //         : null;
    //
    //     public static Type GetPropertyType(this ENTITY entity, string name)
    //         => _typesQUALIFIER.TryGetValue(name, out var type)
    //         ? type
    //         : null;
    //
    //     public static IEnumerable<string> GetPath(this ENTITY entity)
    //         => _pathQUALIFIER;
    //
    //     public static IEnumerable<string> GetProperties(this ENTITY entity)
    //         => _propertiesQUALIFIER;
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
                                        SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                            SyntaxFactory.Token(SyntaxKind.PartialKeyword)))
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
                                                                        SyntaxFactory.Identifier("_proxies" + CreateQualifier(metadata.QualifiedName)))
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
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword))),
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
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword))),
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
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword))),
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
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName("Type")
                                                                            }))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_types" + CreateQualifier(metadata.QualifiedName)))
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
                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName("Type")
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadata.Members.SelectMany(x => GenerateTypeMap(x))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.FieldDeclaration(
                                                    SyntaxFactory.VariableDeclaration(
                                                            SyntaxFactory.ArrayType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                                .WithRankSpecifiers(
                                                                    SyntaxFactory.SingletonList(
                                                                        SyntaxFactory.ArrayRankSpecifier(
                                                                            SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                                                SyntaxFactory.OmittedArraySizeExpression())))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_path" + CreateQualifier(metadata.QualifiedName)))
                                                                    .WithInitializer(
                                                                        SyntaxFactory.EqualsValueClause(
                                                                            SyntaxFactory.CollectionExpression(
                                                                                SyntaxFactory.SeparatedList<CollectionElementSyntax>(
                                                                                    metadata.Path.SelectMany(GenerateArrayStringEntry))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                    [
                                                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                    ])),
                                            SyntaxFactory.FieldDeclaration(
                                                    SyntaxFactory.VariableDeclaration(
                                                            SyntaxFactory.ArrayType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                                .WithRankSpecifiers(
                                                                    SyntaxFactory.SingletonList(
                                                                        SyntaxFactory.ArrayRankSpecifier(
                                                                            SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                                                SyntaxFactory.OmittedArraySizeExpression())))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_properties" + CreateQualifier(metadata.QualifiedName)))
                                                                    .WithInitializer(
                                                                        SyntaxFactory.EqualsValueClause(
                                                                            SyntaxFactory.CollectionExpression(
                                                                                SyntaxFactory.SeparatedList<CollectionElementSyntax>(
                                                                                    metadata.Members.SelectMany(x => GenerateArrayStringEntry(x.Name)))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                    [
                                                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                    ])),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.IdentifierName(ProxyInterfaceName),
                                                    SyntaxFactory.Identifier("GetProxy"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
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
                                                                                SyntaxFactory.IdentifierName("_proxies" + CreateQualifier(metadata.QualifiedName)))
                                                                        })))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
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
                                                                SyntaxFactory.DeclarationPattern(
                                                                    SyntaxFactory.IdentifierName("MemberExpression"),
                                                                    SyntaxFactory.SingleVariableDesignation(
                                                                        SyntaxFactory.Identifier("member")))),
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName("entity"),
                                                                        SyntaxFactory.IdentifierName("MapToSerialized")))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SingletonSeparatedList(
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("member"))))),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
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
                                                                        SyntaxFactory.Identifier("member"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("MemberExpression"))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ConditionalExpression(
                                                            SyntaxFactory.IsPatternExpression(
                                                                SyntaxFactory.IdentifierName("member"),
                                                                SyntaxFactory.RecursivePattern()
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
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
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
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.IdentifierName("Type"),
                                                    SyntaxFactory.Identifier("GetPropertyType"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
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
                                                                        SyntaxFactory.Identifier("name"))
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
                                                                        SyntaxFactory.IdentifierName("_types" + CreateQualifier(metadata.QualifiedName)),
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
                                                                                                SyntaxFactory.Identifier(
                                                                                                    SyntaxFactory.TriviaList(),
                                                                                                    SyntaxKind.TypeKeyword,
                                                                                                    "type",
                                                                                                    "type",
                                                                                                    SyntaxFactory.TriviaList()))))
                                                                                    .WithRefOrOutKeyword(
                                                                                        SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                            }))),
                                                            SyntaxFactory.IdentifierName(
                                                                SyntaxFactory.Identifier(
                                                                    SyntaxFactory.TriviaList(),
                                                                    SyntaxKind.TypeKeyword,
                                                                    "type",
                                                                    "type",
                                                                    SyntaxFactory.TriviaList())),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("IEnumerable"))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))),
                                                    SyntaxFactory.Identifier("GetPath"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
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
                                                        SyntaxFactory.IdentifierName("_path" + CreateQualifier(metadata.QualifiedName))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("IEnumerable"))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))),
                                                    SyntaxFactory.Identifier("GetProperties"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
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
                                                        SyntaxFactory.IdentifierName("_properties" + CreateQualifier(metadata.QualifiedName))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
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
                                        SyntaxFactory.Literal(member.Name)))))),
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

    // ["PROPMEMBER"] = typeof(PROPTYPE),
    /// <summary>
    /// Generates dictionary entry member name -> type map.
    /// </summary>
    /// <param name="member">Member to generate.</param>
    /// <returns>Generated dictionary entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateTypeMap(in EntityMemberMetadata member)
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
                SyntaxFactory.TypeOfExpression(
                    SyntaxFactory.IdentifierName(member.Type))),
            SyntaxFactory.Token(SyntaxKind.CommaToken)
        ];

    // "PATH",
    // or
    // "PROPMEMBER",
    /// <summary>
    /// Generates array entry for path element.
    /// </summary>
    /// <param name="element">Element to generate.</param>
    /// <returns>Generated array entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateArrayStringEntry(string element)
        =>
        [
            SyntaxFactory.ExpressionElement(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(element))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];

    // using System;
    // using System.Collections.Generic;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal static partial class EnumProxies
    // {
    //     private static readonly IReadOnlyDictionary<string, ENUM> _unmapQUALIFIER = new Dictionary<string, ENUM>()
    //     {
    //         ["SERIALIZED"] = ENUM.VALUE,
    //     };
    //
    //     private static readonly IReadOnlyDictionary<ENUM, string> _mapQUALIFIER = new Dictionary<ENUM, string>()
    //     {
    //         [ENUM.VALUE] = "SERIALIZED",
    //     };
    //
    //     public static string MapToSerialized(this ENUM @enum)
    //         => _mapQUALIFIER.TryGetValue(@enum, out var serialized)
    //         ? serialized
    //         : null;
    // }
    /// <summary>
    /// Generates an enum proxy for a given enum.
    /// </summary>
    /// <param name="metadata">Metadata of the enum.</param>
    /// <returns>Generated enum proxy.</returns>
    public static CompilationUnitSyntax GenerateEnumProxyStatic(EnumMetadata metadata)
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
                            SyntaxFactory.IdentifierName("Generic")))
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
                                SyntaxFactory.ClassDeclaration(EnumProxiesClassName)
                                    .WithModifiers(
                                        SyntaxFactory.TokenList(
                                            [SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword)]))
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
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName(metadata.QualifiedName)
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
                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.IdentifierName(metadata.QualifiedName)
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadata.MemberMappings.SelectMany(x => GenerateEnumProxyUnmap(metadata, x.Key, x.Value))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                    [
                                                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                    ])),
                                            SyntaxFactory.FieldDeclaration(
                                                    SyntaxFactory.VariableDeclaration(
                                                            SyntaxFactory.GenericName(
                                                                    SyntaxFactory.Identifier("IReadOnlyDictionary"))
                                                                .WithTypeArgumentList(
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.IdentifierName(metadata.QualifiedName), SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                                SyntaxFactory.PredefinedType(
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
                                                                                                        SyntaxFactory.IdentifierName(metadata.QualifiedName),
                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.PredefinedType(
                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword))
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadata.MemberMappings.SelectMany(x => GenerateEnumProxyMap(metadata, x.Key, x.Value))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                    [
                                                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                    ])),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("@enum"))
                                                                .WithModifiers(
                                                                    SyntaxFactory.TokenList(
                                                                        SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName(metadata.QualifiedName)))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
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
                                                                                    SyntaxFactory.IdentifierName("@enum")),
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
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                        ]))))))
            .NormalizeWhitespace();

    // ["SERIALIZED"] = ENUM.VALUE,
    /// <summary>
    /// Generates an enum unmap dictionary entry.
    /// </summary>
    /// <param name="metadata">Enum metadata.</param>
    /// <param name="member">Name of the member to generate.</param>
    /// <param name="serialized">Serialized name of the member.</param>
    /// <returns>Generates entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateEnumProxyUnmap(in EnumMetadata metadata, string member, string serialized)
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
                                        SyntaxFactory.Literal(serialized)))))),
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(metadata.QualifiedName),
                    SyntaxFactory.IdentifierName(member))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];

    // [ENUM.VALUE] = "SERIALIZED",
    /// <summary>
    /// Generates an enum map dictionary entry.
    /// </summary>
    /// <param name="metadata">Enum metadata.</param>
    /// <param name="member">Name of the member to generate.</param>
    /// <param name="serialized">Serialized name of the member.</param>
    /// <returns>Generates entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateEnumProxyMap(in EnumMetadata metadata, string member, string serialized)
        =>
        [
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.ImplicitElementAccess()
                    .WithArgumentList(
                        SyntaxFactory.BracketedArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(metadata.QualifiedName),
                                        SyntaxFactory.IdentifierName(member)))))),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(serialized))),
            SyntaxFactory.Token(SyntaxKind.CommaToken)
        ];

    // using System;
    // using System.Collections.Generic;
    // using Emzi0767.NetworkSelfService.Mikrotik.Entities;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal interface IMikrotikEntityProxyGetterSetter<T>
    //     where T : class, IMikrotikEntity
    // {
    //     object Get(T @this);
    //     void Set(T @this, object value);
    // }
    //
    // internal readonly struct MikrotikEntityProxyGetterSetter<T, TProp> : IMikrotikEntityProxyGetterSetter<T>
    //     where T : class, IMikrotikEntity
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
    // internal readonly struct MikrotikEntityProxy<T> : class, IMikrotikEntityProxy
    //     where T : class, IMikrotikEntity
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
                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.ClassOrStructConstraint(
                                                            SyntaxKind.ClassConstraint),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                    }))))
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
                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)))
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
                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.ClassOrStructConstraint(
                                                            SyntaxKind.ClassConstraint),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                    }))))
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
                                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)))
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
                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.ClassOrStructConstraint(
                                                            SyntaxKind.ClassConstraint),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                    }))))
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

    // using System;
    // using System.Collections.Generic;
    // using System.Linq.Expressions;
    // using System.Reflection;
    // using Emzi0767.NetworkSelfService.Mikrotik.Entities;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal static partial class EntityProxies
    // {
    //     internal static readonly IReadOnlyDictionary<Type, IEnumerable<string>> _pathRegistry = new Dictionary<Type, IEnumerable<string>>()
    //     {
    //         [typeof(ENTITY)] = _pathQUALIFIER,
    //     };
    //
    //     public static IEnumerable<string> GetPath<T>()
    //         where T : class, IMikrotikEntity
    //         => GetPath(typeof(T));
    //
    //     public static IEnumerable<string> GetPath(Type t)
    //         => _pathRegistry.TryGetValue(t, out var path)
    //         ? path
    //         : null;
    //
    //     public static string MapToSerialized<TEntity, TProp>(Expression<Func<TEntity, TProp>> prop)
    //         where TEntity : class, IMikrotikEntity
    //         => prop.Body is MemberExpression member
    //         ? MapToSerialized<TEntity>(member)
    //         : null;
    //
    //     public static string MapToSerialized<TEntity>(MemberExpression member)
    //         where TEntity : class, IMikrotikEntity
    //         => MapToSerialized(typeof(TEntity), member);
    //
    //     public static string MapToSerialized<TEntity>(MemberExpression member, string name)
    //         where TEntity : class, IMikrotikEntity
    //         => MapToSerialized(typeof(TEntity), name);
    //
    //     public static string MapToSerialized(Type tEntity, MemberExpression member)
    //         => member is { Member: PropertyInfo property }
    //         ? MapToSerialized(tEntity, property.Name)
    //         : null;
    //
    //     public static string MapToSerialized(Type tEntity, string name)
    //     {
    //         var dict = tEntity.FullName switch
    //         {
    //             "ENTITY" => _mapQUALIFIER,
    //             _ => null,
    //         };
    //
    //         return dict is not null && dict.TryGetValue(name, out var serialized)
    //             ? serialized
    //             : null;
    //     }
    //
    //     public static string MapFromSerialized<TEntity>(string serialized)
    //         where TEntity : class, IMikrotikEntity
    //         => MapFromSerialized(typeof(TEntity), serialized);
    //
    //     public static string MapFromSerialized(Type tEntity, string serialized)
    //     {
    //         var dict = tEntity.FullName switch
    //         {
    //             "ENTITY" => _unmapQUALIFIER,
    //             _ => null,
    //         };
    //
    //         return dict is not null && dict.TryGetValue(serialized, out var name)
    //             ? name
    //             : null;
    //     }
    //
    //     public static IEnumerable<string> GetProperties<T>()
    //         where T : class, IMikrotikEntity
    //         => GetProperties(typeof(T));
    //
    //     public static IEnumerable<string> GetProperties(Type t)
    //         => t.FullName switch
    //         {
    //             "ENTITY" => _propertiesQUALIFIER,
    //             _ => null,
    //         };
    //
    //     public static IMikrotikEntityProxy GetProxy(Type t, object entity)
    //         => t.FullName switch
    //         {
    //             "ENTITY" => (entity as ENTITY).GetProxy(),
    //             _ => null,
    //         };
    //
    //     public static Type GetPropertyType(Type tEntity, string name)
    //     {
    //         var dict = tEntity.FullName switch
    //         {
    //             "ENTITY" => _typesQUALIFIER,
    //             _ => null,
    //         };
    //
    //         return dict is not null && dict.TryGetValue(name, out var type)
    //             ? type
    //             : null;
    //     }
    //
    //     public static object Instantiate(Type tEntity, MikrotikClient client)
    //         => tEntity.FullName switch
    //         {
    //             "ENTITY" => new ENTITY(client),
    //             _ => null,
    //         };
    // }
    /// <summary>
    /// Generates an entity path map for all collected entities.
    /// </summary>
    /// <param name="metadatas">Collection of all entity metadata instances.</param>
    /// <returns>Generated entity path map.</returns>
    public static CompilationUnitSyntax GenerateEntityPathMapStatic(IEnumerable<EntityMetadata> metadatas)
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
                            SyntaxFactory.IdentifierName("Reflection"))),
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
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                SyntaxFactory.ClassDeclaration(EntityProxiesClassName)
                                    .WithModifiers(
                                        SyntaxFactory.TokenList(
                                            [SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword)]))
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
                                                                                SyntaxFactory.IdentifierName("Type"), SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.GenericName(
                                                                                        SyntaxFactory.Identifier("IEnumerable"))
                                                                                    .WithTypeArgumentList(
                                                                                        SyntaxFactory.TypeArgumentList(
                                                                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                                SyntaxFactory.PredefinedType(
                                                                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword)))))
                                                                            }))))
                                                        .WithVariables(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.VariableDeclarator(
                                                                        SyntaxFactory.Identifier("_pathRegistry"))
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
                                                                                                        SyntaxFactory.IdentifierName("Type"), SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory
                                                                                                            .GenericName(
                                                                                                                SyntaxFactory.Identifier("IEnumerable"))
                                                                                                            .WithTypeArgumentList(
                                                                                                                SyntaxFactory.TypeArgumentList(
                                                                                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                                                        SyntaxFactory.PredefinedType(
                                                                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))))
                                                                                                    }))))
                                                                                .WithArgumentList(
                                                                                    SyntaxFactory.ArgumentList())
                                                                                .WithInitializer(
                                                                                    SyntaxFactory.InitializerExpression(
                                                                                        SyntaxKind.ObjectInitializerExpression,
                                                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                                            metadatas.SelectMany(x => GenerateEntityPath(x))))))))))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                    [
                                                        SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                                                    ])),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("IEnumerable"))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))),
                                                    SyntaxFactory.Identifier("GetPath"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
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
                                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.ClassOrStructConstraint(
                                                                            SyntaxKind.ClassConstraint),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                                    }))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.IdentifierName("GetPath"))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.TypeOfExpression(
                                                                                SyntaxFactory.IdentifierName("T"))))))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("IEnumerable"))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))),
                                                    SyntaxFactory.Identifier("GetPath"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("t"))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName("Type")))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ConditionalExpression(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName("_pathRegistry"),
                                                                        SyntaxFactory.IdentifierName("TryGetValue")))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.IdentifierName("t")),
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
                                                                                                SyntaxFactory.Identifier("path"))))
                                                                                    .WithRefOrOutKeyword(
                                                                                        SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                            }))),
                                                            SyntaxFactory.IdentifierName("path"),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithTypeParameterList(
                                                    SyntaxFactory.TypeParameterList(
                                                        SyntaxFactory.SeparatedList<TypeParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.TypeParameter(
                                                                    SyntaxFactory.Identifier("TEntity")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeParameter(
                                                                    SyntaxFactory.Identifier("TProp"))
                                                            })))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
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
                                                                                                        SyntaxFactory.IdentifierName("TEntity"), SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                                                        SyntaxFactory.IdentifierName("TProp")
                                                                                                    }))))))))))
                                                .WithConstraintClauses(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.TypeParameterConstraintClause(
                                                                SyntaxFactory.IdentifierName("TEntity"))
                                                            .WithConstraints(
                                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.ClassOrStructConstraint(
                                                                            SyntaxKind.ClassConstraint),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                                    }))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ConditionalExpression(
                                                            SyntaxFactory.IsPatternExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("prop"),
                                                                    SyntaxFactory.IdentifierName("Body")),
                                                                SyntaxFactory.DeclarationPattern(
                                                                    SyntaxFactory.IdentifierName("MemberExpression"),
                                                                    SyntaxFactory.SingleVariableDesignation(
                                                                        SyntaxFactory.Identifier("member")))),
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.GenericName(
                                                                            SyntaxFactory.Identifier("MapToSerialized"))
                                                                        .WithTypeArgumentList(
                                                                            SyntaxFactory.TypeArgumentList(
                                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                    SyntaxFactory.IdentifierName("TEntity")))))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SingletonSeparatedList(
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("member"))))),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithTypeParameterList(
                                                    SyntaxFactory.TypeParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.TypeParameter(
                                                                SyntaxFactory.Identifier("TEntity")))))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("member"))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName("MemberExpression")))))
                                                .WithConstraintClauses(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.TypeParameterConstraintClause(
                                                                SyntaxFactory.IdentifierName("TEntity"))
                                                            .WithConstraints(
                                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.ClassOrStructConstraint(
                                                                            SyntaxKind.ClassConstraint),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                                    }))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.IdentifierName("MapToSerialized"))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.TypeOfExpression(
                                                                                    SyntaxFactory.IdentifierName("TEntity"))),
                                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("member"))
                                                                        })))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithTypeParameterList(
                                                    SyntaxFactory.TypeParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.TypeParameter(
                                                                SyntaxFactory.Identifier("TEntity")))))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("member"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("MemberExpression")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("name"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                            })))
                                                .WithConstraintClauses(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.TypeParameterConstraintClause(
                                                                SyntaxFactory.IdentifierName("TEntity"))
                                                            .WithConstraints(
                                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.ClassOrStructConstraint(
                                                                            SyntaxKind.ClassConstraint),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                                    }))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.IdentifierName("MapToSerialized"))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.TypeOfExpression(
                                                                                    SyntaxFactory.IdentifierName("TEntity"))),
                                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("name"))
                                                                        })))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEntity"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("member"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("MemberExpression"))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.ConditionalExpression(
                                                            SyntaxFactory.IsPatternExpression(
                                                                SyntaxFactory.IdentifierName("member"),
                                                                SyntaxFactory.RecursivePattern()
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
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.IdentifierName("MapToSerialized"))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.IdentifierName("tEntity")),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                    SyntaxFactory.MemberAccessExpression(
                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                        SyntaxFactory.IdentifierName(
                                                                                            SyntaxFactory.Identifier(
                                                                                                SyntaxFactory.TriviaList(),
                                                                                                SyntaxKind.PropertyKeyword,
                                                                                                "property",
                                                                                                "property",
                                                                                                SyntaxFactory.TriviaList())),
                                                                                        SyntaxFactory.IdentifierName("Name")))
                                                                            }))),
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEntity"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("name"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                            })))
                                                .WithBody(
                                                    SyntaxFactory.Block(
                                                        SyntaxFactory.LocalDeclarationStatement(
                                                            SyntaxFactory.VariableDeclaration(
                                                                    SyntaxFactory.IdentifierName(
                                                                        SyntaxFactory.Identifier(
                                                                            SyntaxFactory.TriviaList(),
                                                                            SyntaxKind.VarKeyword,
                                                                            "var",
                                                                            "var",
                                                                            SyntaxFactory.TriviaList())))
                                                                .WithVariables(
                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                        SyntaxFactory.VariableDeclarator(
                                                                                SyntaxFactory.Identifier("dict"))
                                                                            .WithInitializer(
                                                                                SyntaxFactory.EqualsValueClause(
                                                                                    SyntaxFactory.SwitchExpression(
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.IdentifierName("tEntity"),
                                                                                                SyntaxFactory.IdentifierName("FullName")))
                                                                                        .WithArms(
                                                                                            SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                                            [
                                                                                                ..metadatas.SelectMany(x => GenerateSwitchArm(x, "_map")),
                                                                                                SyntaxFactory.SwitchExpressionArm(
                                                                                                    SyntaxFactory.DiscardPattern(),
                                                                                                    SyntaxFactory.LiteralExpression(
                                                                                                        SyntaxKind.NullLiteralExpression)),
                                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                                            ]))))))),
                                                        SyntaxFactory.ReturnStatement(
                                                            SyntaxFactory.ConditionalExpression(
                                                                SyntaxFactory.BinaryExpression(
                                                                    SyntaxKind.LogicalAndExpression,
                                                                    SyntaxFactory.IsPatternExpression(
                                                                        SyntaxFactory.IdentifierName("dict"),
                                                                        SyntaxFactory.UnaryPattern(
                                                                            SyntaxFactory.ConstantPattern(
                                                                                SyntaxFactory.LiteralExpression(
                                                                                    SyntaxKind.NullLiteralExpression)))),
                                                                    SyntaxFactory.InvocationExpression(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.IdentifierName("dict"),
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
                                                                                                        SyntaxFactory.Identifier("serialized"))))
                                                                                            .WithRefOrOutKeyword(
                                                                                                SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                                    })))),
                                                                SyntaxFactory.IdentifierName("serialized"),
                                                                SyntaxFactory.LiteralExpression(
                                                                    SyntaxKind.NullLiteralExpression))))),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapFromSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithTypeParameterList(
                                                    SyntaxFactory.TypeParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.TypeParameter(
                                                                SyntaxFactory.Identifier("TEntity")))))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("serialized"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                                                .WithConstraintClauses(
                                                    SyntaxFactory.SingletonList(
                                                        SyntaxFactory.TypeParameterConstraintClause(
                                                                SyntaxFactory.IdentifierName("TEntity"))
                                                            .WithConstraints(
                                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.ClassOrStructConstraint(
                                                                            SyntaxKind.ClassConstraint),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                                    }))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.IdentifierName("MapFromSerialized"))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            SyntaxFactory.Argument(
                                                                                SyntaxFactory.TypeOfExpression(
                                                                                    SyntaxFactory.IdentifierName("TEntity"))),
                                                                            SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                SyntaxFactory.IdentifierName("serialized"))
                                                                        })))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapFromSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEntity"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("serialized"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                            })))
                                                .WithBody(
                                                    SyntaxFactory.Block(
                                                        SyntaxFactory.LocalDeclarationStatement(
                                                            SyntaxFactory.VariableDeclaration(
                                                                    SyntaxFactory.IdentifierName(
                                                                        SyntaxFactory.Identifier(
                                                                            SyntaxFactory.TriviaList(),
                                                                            SyntaxKind.VarKeyword,
                                                                            "var",
                                                                            "var",
                                                                            SyntaxFactory.TriviaList())))
                                                                .WithVariables(
                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                        SyntaxFactory.VariableDeclarator(
                                                                                SyntaxFactory.Identifier("dict"))
                                                                            .WithInitializer(
                                                                                SyntaxFactory.EqualsValueClause(
                                                                                    SyntaxFactory.SwitchExpression(
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.IdentifierName("tEntity"),
                                                                                                SyntaxFactory.IdentifierName("FullName")))
                                                                                        .WithArms(
                                                                                            SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                                            [
                                                                                                ..metadatas.SelectMany(x => GenerateSwitchArm(x, "_unmap")),
                                                                                                SyntaxFactory.SwitchExpressionArm(
                                                                                                    SyntaxFactory.DiscardPattern(),
                                                                                                    SyntaxFactory.LiteralExpression(
                                                                                                        SyntaxKind.NullLiteralExpression)),
                                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                                            ]))))))),
                                                        SyntaxFactory.ReturnStatement(
                                                            SyntaxFactory.ConditionalExpression(
                                                                SyntaxFactory.BinaryExpression(
                                                                    SyntaxKind.LogicalAndExpression,
                                                                    SyntaxFactory.IsPatternExpression(
                                                                        SyntaxFactory.IdentifierName("dict"),
                                                                        SyntaxFactory.UnaryPattern(
                                                                            SyntaxFactory.ConstantPattern(
                                                                                SyntaxFactory.LiteralExpression(
                                                                                    SyntaxKind.NullLiteralExpression)))),
                                                                    SyntaxFactory.InvocationExpression(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.IdentifierName("dict"),
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
                                                                                    })))),
                                                                SyntaxFactory.IdentifierName("name"),
                                                                SyntaxFactory.LiteralExpression(
                                                                    SyntaxKind.NullLiteralExpression))))),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("IEnumerable"))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))),
                                                    SyntaxFactory.Identifier("GetProperties"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
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
                                                                SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(
                                                                    new SyntaxNodeOrToken[]
                                                                    {
                                                                        SyntaxFactory.ClassOrStructConstraint(
                                                                            SyntaxKind.ClassConstraint),
                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.TypeConstraint(
                                                                            SyntaxFactory.IdentifierName("IMikrotikEntity"))
                                                                    }))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.IdentifierName("GetProperties"))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.TypeOfExpression(
                                                                                SyntaxFactory.IdentifierName("T"))))))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("IEnumerable"))
                                                        .WithTypeArgumentList(
                                                            SyntaxFactory.TypeArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))),
                                                    SyntaxFactory.Identifier("GetProperties"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("t"))
                                                                .WithType(
                                                                    SyntaxFactory.IdentifierName("Type")))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.SwitchExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("t"),
                                                                    SyntaxFactory.IdentifierName("FullName")))
                                                            .WithArms(
                                                                SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                [
                                                                    ..metadatas.SelectMany(x => GenerateSwitchArm(x, "_properties")),
                                                                    SyntaxFactory.SwitchExpressionArm(
                                                                        SyntaxFactory.DiscardPattern(),
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.NullLiteralExpression)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                ]))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.IdentifierName(ProxyInterfaceName),
                                                    SyntaxFactory.Identifier("GetProxy"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("t"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("entity"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.SwitchExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("t"),
                                                                    SyntaxFactory.IdentifierName("FullName")))
                                                            .WithArms(
                                                                SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                [
                                                                    ..metadatas.SelectMany(x => GenerateProxySwitchArm(x)),
                                                                    SyntaxFactory.SwitchExpressionArm(
                                                                        SyntaxFactory.DiscardPattern(),
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.NullLiteralExpression)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                ]))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.IdentifierName("Type"),
                                                    SyntaxFactory.Identifier("GetPropertyType"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEntity"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("name"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                            })))
                                                .WithBody(
                                                    SyntaxFactory.Block(
                                                        SyntaxFactory.LocalDeclarationStatement(
                                                            SyntaxFactory.VariableDeclaration(
                                                                    SyntaxFactory.IdentifierName(
                                                                        SyntaxFactory.Identifier(
                                                                            SyntaxFactory.TriviaList(),
                                                                            SyntaxKind.VarKeyword,
                                                                            "var",
                                                                            "var",
                                                                            SyntaxFactory.TriviaList())))
                                                                .WithVariables(
                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                        SyntaxFactory.VariableDeclarator(
                                                                                SyntaxFactory.Identifier("dict"))
                                                                            .WithInitializer(
                                                                                SyntaxFactory.EqualsValueClause(
                                                                                    SyntaxFactory.SwitchExpression(
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.IdentifierName("tEntity"),
                                                                                                SyntaxFactory.IdentifierName("FullName")))
                                                                                        .WithArms(
                                                                                            SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                                            [
                                                                                                ..metadatas.SelectMany(x => GenerateSwitchArm(x, "_types")),
                                                                                                SyntaxFactory.SwitchExpressionArm(
                                                                                                    SyntaxFactory.DiscardPattern(),
                                                                                                    SyntaxFactory.LiteralExpression(
                                                                                                        SyntaxKind.NullLiteralExpression)),
                                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                                            ]))))))),
                                                        SyntaxFactory.ReturnStatement(
                                                            SyntaxFactory.ConditionalExpression(
                                                                SyntaxFactory.BinaryExpression(
                                                                    SyntaxKind.LogicalAndExpression,
                                                                    SyntaxFactory.IsPatternExpression(
                                                                        SyntaxFactory.IdentifierName("dict"),
                                                                        SyntaxFactory.UnaryPattern(
                                                                            SyntaxFactory.ConstantPattern(
                                                                                SyntaxFactory.LiteralExpression(
                                                                                    SyntaxKind.NullLiteralExpression)))),
                                                                    SyntaxFactory.InvocationExpression(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.IdentifierName("dict"),
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
                                                                                                        SyntaxFactory.Identifier(
                                                                                                            SyntaxFactory.TriviaList(),
                                                                                                            SyntaxKind.TypeKeyword,
                                                                                                            "type",
                                                                                                            "type",
                                                                                                            SyntaxFactory.TriviaList()))))
                                                                                            .WithRefOrOutKeyword(
                                                                                                SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                                                                    })))),
                                                                SyntaxFactory.IdentifierName(
                                                                    SyntaxFactory.Identifier(
                                                                        SyntaxFactory.TriviaList(),
                                                                        SyntaxKind.TypeKeyword,
                                                                        "type",
                                                                        "type",
                                                                        SyntaxFactory.TriviaList())),
                                                                SyntaxFactory.LiteralExpression(
                                                                    SyntaxKind.NullLiteralExpression))))),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                    SyntaxFactory.Identifier("Instantiate"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEntity"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("client"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("MikrotikClient"))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.SwitchExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("tEntity"),
                                                                    SyntaxFactory.IdentifierName("FullName")))
                                                            .WithArms(
                                                                SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                [
                                                                    ..metadatas.SelectMany(x => GenerateConstructorSwitchArm(x)),
                                                                    SyntaxFactory.SwitchExpressionArm(
                                                                        SyntaxFactory.DiscardPattern(),
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.NullLiteralExpression)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                ]))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                        ]))))))
            .NormalizeWhitespace();

    // [typeof(ENTITY)] = _pathQUALIFIER,
    /// <summary>
    /// Generates dictionary entry for path map.
    /// </summary>
    /// <param name="metadata">Metadata to generate for.</param>
    /// <returns>Generated dictionary entry.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateEntityPath(in EntityMetadata metadata)
        =>
        [
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.ImplicitElementAccess()
                    .WithArgumentList(
                        SyntaxFactory.BracketedArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.TypeOfExpression(
                                        SyntaxFactory.IdentifierName(metadata.QualifiedName)))))),
                SyntaxFactory.IdentifierName("_path" + CreateQualifier(metadata.QualifiedName))),
            SyntaxFactory.Token(SyntaxKind.CommaToken)
        ];

    // "ENTITY" => _mapQUALIFIER,
    // or
    // "ENTITY" => _unmapQUALIFIER,
    // or
    // "ENTITY" => _propertiesQUALIFIER,
    // or
    // "ENTITY" => _typesQUALIFIER,
    /// <summary>
    /// Generates switch arm for given entity type.
    /// </summary>
    /// <param name="metadata">Metadata to generate for.</param>
    /// <param name="fieldPrefix">Prefix of the field accessor.</param>
    /// <returns>Generated switch arm.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateSwitchArm(in EntityMetadata metadata, string fieldPrefix)
        =>
        [
            SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.ConstantPattern(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(metadata.QualifiedName))),
                SyntaxFactory.IdentifierName(fieldPrefix + CreateQualifier(metadata.QualifiedName))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];

    // "ENTITY" => (entity as ENTITY).GetProxy(),
    /// <summary>
    /// Generates switch arm for given entity type.
    /// </summary>
    /// <param name="metadata">Metadata to generate for.</param>
    /// <returns>Generated switch arm.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateProxySwitchArm(in EntityMetadata metadata)
        =>
        [
            SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.ConstantPattern(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(metadata.QualifiedName))),
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ParenthesizedExpression(
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.AsExpression,
                                SyntaxFactory.IdentifierName("entity"),
                                SyntaxFactory.IdentifierName(metadata.QualifiedName))),
                        SyntaxFactory.IdentifierName("GetProxy")))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];

    // "ENTITY" => new ENTITY(client),
    /// <summary>
    /// Generates switch arm for given entity type.
    /// </summary>
    /// <param name="metadata">Metadata to generate for.</param>
    /// <returns>Generated switch arm.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateConstructorSwitchArm(in EntityMetadata metadata)
        =>
        [
            SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.ConstantPattern(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(metadata.QualifiedName))),
                SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.IdentifierName(metadata.QualifiedName))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName("client")))))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];

    // using System;
    // using System.Collections.Generic;
    //
    // namespace Emzi0767.NetworkSelfService.Mikrotik;
    //
    // internal static partial class EnumProxies
    // {
    //     public static string MapToSerialized(this Type tEnum, object @enum)
    //         => tEnum.FullName switch
    //         {
    //             "ENUM" => ((ENUM)@enum).MapToSerialized(),
    //             _ => null,
    //         };
    //
    //     public static T MapFromSerialized<T>(string serialized)
    //         where T : Enum
    //         => MapFromSerialized(typeof(T), serialized);
    //
    //     public static object MapFromSerialized(Type tEnum, string serialized)
    //         => tEnum.FullName switch
    //         {
    //             "ENUM" => _unmapQUALIFIER.GetValueOrDefault(serialized),
    //             _ => null,
    //         };
    // }
    /// <summary>
    /// Generates an enum map for all collected enums.
    /// </summary>
    /// <param name="metadatas">Collection of all enum metadata instances.</param>
    /// <returns>Generated enum map.</returns>
    public static CompilationUnitSyntax GenerateEnumMapStatic(IEnumerable<EnumMetadata> metadatas)
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
                            SyntaxFactory.IdentifierName("Generic")))
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
                                SyntaxFactory.ClassDeclaration(EnumProxiesClassName)
                                    .WithModifiers(
                                        SyntaxFactory.TokenList(
                                            [SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword)]))
                                    .WithMembers(
                                        SyntaxFactory.List<MemberDeclarationSyntax>(
                                        [
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                    SyntaxFactory.Identifier("MapToSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        [SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEnum"))
                                                                    .WithModifiers(
                                                                        SyntaxFactory.TokenList(
                                                                            SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("@enum"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.ObjectKeyword)))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.SwitchExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("tEnum"),
                                                                    SyntaxFactory.IdentifierName("FullName")))
                                                            .WithArms(
                                                                SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                [
                                                                    ..metadatas.SelectMany(x => GenerateEnumMapSwitchArm(x)),
                                                                    SyntaxFactory.SwitchExpressionArm(
                                                                        SyntaxFactory.DiscardPattern(),
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.NullLiteralExpression)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                ]))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.IdentifierName("T"),
                                                    SyntaxFactory.Identifier("MapFromSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword) }))
                                                .WithTypeParameterList(
                                                    SyntaxFactory.TypeParameterList(
                                                        SyntaxFactory.SingletonSeparatedList<TypeParameterSyntax>(
                                                            SyntaxFactory.TypeParameter(
                                                                SyntaxFactory.Identifier("T")))))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                                            SyntaxFactory.Parameter(
                                                                    SyntaxFactory.Identifier("serialized"))
                                                                .WithType(
                                                                    SyntaxFactory.PredefinedType(
                                                                        SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                                                .WithConstraintClauses(
                                                    SyntaxFactory.SingletonList<TypeParameterConstraintClauseSyntax>(
                                                        SyntaxFactory.TypeParameterConstraintClause(
                                                                SyntaxFactory.IdentifierName("T"))
                                                            .WithConstraints(
                                                                SyntaxFactory.SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                                                    SyntaxFactory.TypeConstraint(
                                                                        SyntaxFactory.IdentifierName("Enum"))))))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.CastExpression(
                                                            SyntaxFactory.IdentifierName("T"),
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.IdentifierName("MapFromSerialized"))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.TypeOfExpression(
                                                                                        SyntaxFactory.IdentifierName("T"))),
                                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Argument(
                                                                                    SyntaxFactory.IdentifierName("serialized"))
                                                                            }))))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.MethodDeclaration(
                                                    SyntaxFactory.PredefinedType(
                                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                                                    SyntaxFactory.Identifier("MapFromSerialized"))
                                                .WithModifiers(
                                                    SyntaxFactory.TokenList(
                                                    [
                                                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                                    ]))
                                                .WithParameterList(
                                                    SyntaxFactory.ParameterList(
                                                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("tEnum"))
                                                                    .WithType(
                                                                        SyntaxFactory.IdentifierName("Type")),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken), SyntaxFactory.Parameter(
                                                                        SyntaxFactory.Identifier("serialized"))
                                                                    .WithType(
                                                                        SyntaxFactory.PredefinedType(
                                                                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                                                            })))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ArrowExpressionClause(
                                                        SyntaxFactory.SwitchExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("tEnum"),
                                                                    SyntaxFactory.IdentifierName("FullName")))
                                                            .WithArms(
                                                                SyntaxFactory.SeparatedList<SwitchExpressionArmSyntax>(
                                                                [
                                                                    ..metadatas.SelectMany(x => GenerateEnumUnmapArm(x)),
                                                                    SyntaxFactory.SwitchExpressionArm(
                                                                        SyntaxFactory.DiscardPattern(),
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.NullLiteralExpression)),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken)
                                                                ]))))
                                                .WithSemicolonToken(
                                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                        ]))))))
            .NormalizeWhitespace();

    // "ENUM" => ((ENUM)@enum).MapToSerialized(),
    /// <summary>
    /// Generates switch arm for given enum map.
    /// </summary>
    /// <param name="metadata">Metadata to generate for.</param>
    /// <returns>Generated switch arm.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateEnumMapSwitchArm(in EnumMetadata metadata)
        =>
        [
            SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.ConstantPattern(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(metadata.QualifiedName))),
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ParenthesizedExpression(
                            SyntaxFactory.CastExpression(
                                SyntaxFactory.IdentifierName(metadata.QualifiedName),
                                SyntaxFactory.IdentifierName("@enum"))),
                        SyntaxFactory.IdentifierName("MapToSerialized")))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];

    // "ENUM" => _unmapQUALIFIER.GetValueOrDefault(serialized),
    /// <summary>
    /// Generates switch arm for given enum unmap.
    /// </summary>
    /// <param name="metadata">Metadata to generate for.</param>
    /// <returns>Generated switch arm.</returns>
    private static IEnumerable<SyntaxNodeOrToken> GenerateEnumUnmapArm(in EnumMetadata metadata)
        =>
        [
            SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.ConstantPattern(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(metadata.QualifiedName))),
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("_unmap" + CreateQualifier(metadata.QualifiedName)),
                            SyntaxFactory.IdentifierName("GetValueOrDefault")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName("serialized")))))),
            SyntaxFactory.Token(SyntaxKind.CommaToken),
        ];
}
