using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public interface IContainerClassAnalysisRule
{
    ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    bool RequiredToBeContainer { get; }

    // registrations on interfaces without [DependencyContainer] can be inherited
    // but the interface itself can't be a container, it's just a marker
    bool WantMarkerRegistrationType { get; }

    bool ValidOnTypesWithoutRegistrations { get; }

    void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer, bool hasRegistrations);
}