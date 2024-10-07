using Microsoft.CodeAnalysis;

namespace DanmakuEngine.DependencyInjection.SourceGeneration;

public static class SymbolExtensions
{
    private static SymbolDisplayFormat globalPrefixedFormat = new SymbolDisplayFormat(
        SymbolDisplayGlobalNamespaceStyle.Included,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static string ToGlobalPrefixedFullName(this ISymbol symbol)
        => symbol.ToDisplayString(globalPrefixedFormat);
}