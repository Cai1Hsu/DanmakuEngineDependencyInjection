#pragma warning disable RS2008 // Enable analyzer release tracking

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ContainerClassAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(
            RegistrationRule.DependencyContainerRule,
            ContainerModifierRule.ContainerCanNotBeStatic,
            ContainerModifierRule.ContainerMustBePartial,
            MultipleRegistrationRule.MultipleRegistration
        );

    public static ImmutableArray<ContainerClassAnalyzingRule> AnalyzingRules = ImmutableArray.Create<ContainerClassAnalyzingRule>(
        new RegistrationRule(),
        new ContainerModifierRule(),
        new MultipleRegistrationRule()
    );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    public void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var attributes = namedTypeSymbol.GetAttributes();

        bool isContainer = attributes.Any(a => a.AttributeClass?.ToGlobalPrefixedFullName() == RegistrationRule.DependencyContainerAttribute);

        foreach (var rule in AnalyzingRules)
        {
            if (!rule.RequiredToBeContainer || isContainer)
            {
                rule.AnalyzeSymbol(context, isContainer);
            }
        }
    }
}