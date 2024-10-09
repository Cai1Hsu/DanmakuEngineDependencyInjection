using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class ConstructorAnalysisRule : IContainerClassAnalysisRule
{
    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

    public bool RequiredToBeContainer => true;

    public bool WantMarkerRegistrationType => true;

    public bool ValidOnTypesWithoutRegistrations => false;

    public const string ConstructorMethod = ".ctor";

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer, bool hasRegistrations)
    {
        INamedTypeSymbol containerTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var containerAttributes = containerTypeSymbol.GetAttributes()
            .Where(a => a.AttributeClass is not null)
            .Where(a => ContainerClassAnalyzer.RegistrationAttributes.Contains(a.AttributeClass!.ToGlobalPrefixedFullName()));

        var implTypes = containerAttributes.Where(a => a.AttributeClass!.TypeArguments.Length is 1 or 2)
            .Select(a => a.AttributeClass!.TypeArguments)
            .Select(t => t.Last())
            .Where(t => t.IsAbstract)
            .Select(t => (t, t.GetMembers().Where(m => m.Kind is SymbolKind.Method && m.Name == ConstructorMethod)));

        var registreredTypes = implTypes.ToList();
    }
}