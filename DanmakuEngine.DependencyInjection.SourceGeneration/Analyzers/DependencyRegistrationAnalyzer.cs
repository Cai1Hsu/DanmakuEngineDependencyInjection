#pragma warning disable RS2008 // Enable analyzer release tracking

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DependencyRegistrationAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor DependencyContainerRule = new(
       "DEDI0001",
       title: "Dependencies MUST be registered on a dependency container",
       messageFormat: "Decorate the class with the [DependencyContainer] attribute",
       description: "",
       category: "Design",
       defaultSeverity: DiagnosticSeverity.Warning,
       isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(DependencyContainerRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    public static ImmutableArray<string> RegistrationAttributes =
    [
        "global::DanmakuEngine.DependencyInjection.SingletonAttribute",
        "global::DanmakuEngine.DependencyInjection.TransientAttribute",
        "global::DanmakuEngine.DependencyInjection.ScopedAttribute"
    ];

    public static string DependencyContainerAttribute = "global::DanmakuEngine.DependencyInjection.DependencyContainerAttribute";

    public void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        ImmutableArray<AttributeData> attributes = namedTypeSymbol.GetAttributes();

        if (attributes.Any(a => a.AttributeClass?.ToGlobalPrefixedFullName() == DependencyContainerAttribute))
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
                    Diagnostic.Create(DependencyContainerRule,
                        attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation())
                );
            }
        }
    }
}