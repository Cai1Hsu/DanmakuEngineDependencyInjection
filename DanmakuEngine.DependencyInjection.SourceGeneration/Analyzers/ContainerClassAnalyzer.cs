#pragma warning disable RS2008 // Disable analyzer release tracking

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ContainerClassAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => AnalyzingRules.SelectMany(r => r.SupportedDiagnostics).ToImmutableArray();

    public static readonly ImmutableArray<IContainerClassAnalysisRule> AnalyzingRules = ImmutableArray.Create<IContainerClassAnalysisRule>(
        new DependencyRegistrationRule(),
        new ContainerModifierRule(),
        new MultipleRegistrationRule(),
        new RegistrationTypeRule()
    );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    public static readonly string IDependencyContainer = "global::DanmakuEngine.DependencyInjection.IDependencyContainer";

    public void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var attributes = namedTypeSymbol.GetAttributes();

        bool isContainer = attributes.Any(a => a.AttributeClass?.ToGlobalPrefixedFullName() == DependencyRegistrationRule.DependencyContainerAttribute)
            || namedTypeSymbol.AllInterfaces.Any(i => i.ToGlobalPrefixedFullName() == IDependencyContainer);

        foreach (var rule in AnalyzingRules)
        {
            if (!rule.RequiredToBeContainer || isContainer)
            {
                try
                {
                    rule.AnalyzeSymbol(context, isContainer);
                }
#if DEBUG
                catch (Exception e)
                {
                    foreach (var location in namedTypeSymbol.Locations)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            AnalysisRules.AnalyzerException,
                            location, rule.GetType().Name, namedTypeSymbol.Name, e.Message));
                    }
                }
#else
                catch (Exception)
                {
                }
#endif
            }
        }
    }
}