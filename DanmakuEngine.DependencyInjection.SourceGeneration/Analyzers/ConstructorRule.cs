#pragma warning disable RS2008 // Disable analyzer release tracking

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class ConstructorRule : IContainerClassAnalysisRule
{
    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

    public bool RequiredToBeContainer => true;

    public const string ConstructorMethod = ".ctor";

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool IsContainer)
    {
        INamedTypeSymbol containerTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var containerAttributes = containerTypeSymbol.GetAttributes()
            .Where(a => a.AttributeClass is not null)
            .Where(a => DependencyRegistrationRule.RegistrationAttributes.Contains(a.AttributeClass!.ToGlobalPrefixedFullName()));

        var implTypes = containerAttributes.Where(a => a.AttributeClass!.TypeArguments.Length is 1 or 2)
            .Select(a => a.AttributeClass!.TypeArguments)
            .Select(t => t.Last())
            .Where(t => t.IsAbstract)
            .Select(t => (t, t.GetMembers().Where(m => m.Kind is SymbolKind.Method && m.Name == ConstructorMethod)));

        var registreredTypes = implTypes.ToList();
    }
}