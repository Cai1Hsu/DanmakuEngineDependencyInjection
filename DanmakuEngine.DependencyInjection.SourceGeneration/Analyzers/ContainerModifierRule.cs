#pragma warning disable RS2008 // Enable analyzer release tracking

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class ContainerModifierRule : ContainerClassAnalyzingRule
{
    public static DiagnosticDescriptor ContainerMustBePartial = new(
        "DEDI0002",
        title: "Dependency container class MUST be partial",
        messageFormat: "Add the 'partial' modifier to the class",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ContainerCanNotBeStatic = new(
        "DEDI0003",
        title: "Dependency container class CAN NOT be static",
        messageFormat: "Remove the 'static' modifier from the class",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override bool RequiredToBeContainer => true;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ContainerMustBePartial, ContainerCanNotBeStatic);

    public override void AnalyzeSymbol(SymbolAnalysisContext context, bool _)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var containerDeclarationSyntax = namedTypeSymbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax())
                .OfType<ClassDeclarationSyntax>();

        if (containerDeclarationSyntax is null)
            return;

        EnsureNoStaticContainer(context, containerDeclarationSyntax);
        EnsureContainerIsPartial(context);
    }

    private void AnalyzeContainingTypeRecursively(SymbolAnalysisContext context, INamedTypeSymbol namedTypeSymbol)
    {
        var cds = namedTypeSymbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax())
                .OfType<ClassDeclarationSyntax>();

        if (cds is null)
        {
            return;
        }

        if (!cds.All(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))))
        {
            foreach (var location in namedTypeSymbol.Locations)
            {
                context.ReportDiagnostic(Diagnostic.Create(ContainerMustBePartial, location));
            }
        }

        var containingType = namedTypeSymbol.ContainingType;
        if (containingType is not null)
        {
            AnalyzeContainingTypeRecursively(context, containingType);
        }
    }

    private void EnsureContainerIsPartial(SymbolAnalysisContext context)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        AnalyzeContainingTypeRecursively(context, namedTypeSymbol);
    }

    private void EnsureNoStaticContainer(SymbolAnalysisContext context, IEnumerable<ClassDeclarationSyntax>? containerDeclarationSyntax)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        if (namedTypeSymbol.IsStatic)
        {
            var staticModifiers = containerDeclarationSyntax
                .SelectMany(c => c.Modifiers.Where(m => m.IsKind(SyntaxKind.StaticKeyword)))
                .Where(m => m != null);

            if (staticModifiers.Any())
            {
                foreach (var staticModifier in staticModifiers)
                {
                    context.ReportDiagnostic(Diagnostic.Create(ContainerCanNotBeStatic, staticModifier.GetLocation()));
                }
            }
            else
            {
                foreach (var location in namedTypeSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(ContainerCanNotBeStatic, location));
                }
            }
        }
    }
}