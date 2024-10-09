using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public interface IContainerClassAnalysisRule
{
    ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    bool RequiredToBeContainer { get; }

    void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer);
}