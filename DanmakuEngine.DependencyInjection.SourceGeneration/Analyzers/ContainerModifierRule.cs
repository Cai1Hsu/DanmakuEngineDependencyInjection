using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class ContainerModifierRule : IContainerClassAnalysisRule
{
    public bool RequiredToBeContainer => true;

    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(AnalysisRules.ContainerMustBePartial, AnalysisRules.ContainerCanNotBeStatic);

    // Since we don't generate code for marker types, we don't need to check for them
    public bool WantMarkerRegistrationType => false;

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool _isContainer, bool _hasRegistrations)
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
                context.ReportDiagnostic(Diagnostic.Create(AnalysisRules.ContainerMustBePartial, location));
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
                    context.ReportDiagnostic(Diagnostic.Create(AnalysisRules.ContainerCanNotBeStatic, staticModifier.GetLocation()));
                }
            }
            else
            {
                foreach (var location in namedTypeSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(AnalysisRules.ContainerCanNotBeStatic, location));
                }
            }
        }
    }
}