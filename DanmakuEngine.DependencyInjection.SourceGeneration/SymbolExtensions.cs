using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DanmakuEngine.DependencyInjection.SourceGeneration;

public static class SymbolExtensions
{
    private static SymbolDisplayFormat globalPrefixedFormat = new SymbolDisplayFormat(
        SymbolDisplayGlobalNamespaceStyle.Included,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static string ToGlobalPrefixedFullName(this ISymbol symbol)
        => symbol.ToDisplayString(globalPrefixedFormat);

    public static TypeArgumentListSyntax GetTypeArgumentList(this AttributeSyntax attributeSyntax)
        => attributeSyntax.ChildNodes()
            .OfType<GenericNameSyntax>().Single().TypeArgumentList;
}