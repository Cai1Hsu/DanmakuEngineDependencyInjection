using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public abstract class ContainerClassAnalyzingRule
{
    public abstract bool RequiredToBeContainer { get; }

    public abstract void AnalyzeSymbol(SymbolAnalysisContext context, bool IsContainer);
}