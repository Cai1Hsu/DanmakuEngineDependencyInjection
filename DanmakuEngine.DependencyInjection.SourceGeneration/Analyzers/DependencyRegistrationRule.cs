using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class DependencyRegistrationRule : IContainerClassAnalysisRule
{
    public bool RequiredToBeContainer => false;

    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(AnalysisRules.DependencyContainerRule);

    public static readonly ImmutableArray<string> RegistrationAttributes =
    [
        "global::DanmakuEngine.DependencyInjection.SingletonAttribute",
        "global::DanmakuEngine.DependencyInjection.TransientAttribute",
        "global::DanmakuEngine.DependencyInjection.ScopedAttribute"
    ];

    public static readonly string DependencyContainerAttribute = "global::DanmakuEngine.DependencyInjection.DependencyContainerAttribute";

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        ImmutableArray<AttributeData> attributes = namedTypeSymbol.GetAttributes();

        if (isContainer)
        {
            return;
        }

        foreach (AttributeData? attribute in attributes)
        {
            string? fullName = attribute.AttributeClass?.ToGlobalPrefixedFullName();

            if (fullName is null)
                continue;

            if (RegistrationAttributes.Contains(fullName))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(AnalysisRules.DependencyContainerRule,
                        attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation())
                );
            }
        }
    }
}