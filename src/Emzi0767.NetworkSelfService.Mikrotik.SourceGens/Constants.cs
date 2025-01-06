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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Emzi0767.NetworkSelfService.Mikrotik.SourceGens;

/// <summary>
/// Contains static and constant values for the generator.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the [GenerateMikrotikEntityMetadata] attribute source code. This is a marker attribute, which marks a type for
    /// introspection and proxy generation.
    /// </summary>
    public static CompilationUnitSyntax GenerateMikrotikEntityMetadataAttribute = SyntaxFactory.CompilationUnit()
        .WithUsings(
            SyntaxFactory.SingletonList<UsingDirectiveSyntax>(
                SyntaxFactory.UsingDirective(
                    SyntaxFactory.IdentifierName("System"))))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.FileScopedNamespaceDeclaration(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName("Emzi0767"),
                                SyntaxFactory.IdentifierName("NetworkSelfService")),
                            SyntaxFactory.IdentifierName("Mikrotik")))
                    .WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.ClassDeclaration("GenerateMikrotikEntityMetadataAttribute")
                                .WithAttributeLists(
                                    SyntaxFactory.SingletonList<AttributeListSyntax>(
                                        SyntaxFactory.AttributeList(
                                            SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                                SyntaxFactory.Attribute(
                                                        SyntaxFactory.IdentifierName("AttributeUsage"))
                                                    .WithArgumentList(
                                                        SyntaxFactory.AttributeArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList<AttributeArgumentSyntax>(
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
}
