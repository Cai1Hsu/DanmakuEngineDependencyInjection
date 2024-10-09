using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class DependencyRegistrationRule : IContainerClassAnalysisRule
{
    public bool RequiredToBeContainer => false;

    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(AnalysisRules.DependencyContainerRule);

    public bool WantMarkerRegistrationType => true;

    public static readonly string DependencyContainerAttribute = "global::DanmakuEngine.DependencyInjection.DependencyContainerAttribute";

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer, bool hasRegistrations)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        if (isContainer || !hasRegistrations)
        {
            return;
        }

        ImmutableArray<AttributeData> attributes = namedTypeSymbol.GetAttributes();

        foreach (AttributeData? attribute in attributes)
        {
            string? fullName = attribute.AttributeClass?.ToGlobalPrefixedFullName();

            if (fullName is null)
                continue;

            if (ContainerClassAnalyzer.RegistrationAttributes.Contains(fullName))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(AnalysisRules.DependencyContainerRule,
                        attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation())
                );
            }
        }
    }
}