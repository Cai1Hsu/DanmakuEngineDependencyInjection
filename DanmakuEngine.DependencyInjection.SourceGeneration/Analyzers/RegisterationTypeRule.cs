using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class RegistrationTypeRule : IContainerClassAnalysisRule
{
    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(AnalysisRules.ImplementationTypeMustBeConcrete);

    public bool RequiredToBeContainer => true;

    public bool WantMarkerRegistrationType => true;

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer, bool hasRegistrations)
    {
        INamedTypeSymbol containerTypeSymbol = (INamedTypeSymbol)context.Symbol;

        var containerAttributes = containerTypeSymbol.GetAttributes()
            .Where(a => a.AttributeClass is not null)
            .Where(a => ContainerClassAnalyzer.RegistrationAttributes.Contains(a.AttributeClass!.ToGlobalPrefixedFullName()));

        var registrations = containerAttributes
            .Where(a => a.AttributeClass!.IsGenericType)
            .Where(a => a.AttributeClass!.TypeArguments.Length is 1 or 2)
            .Where(a => a.ApplicationSyntaxReference is not null)
            .Select(a => (a.AttributeClass!, a.ApplicationSyntaxReference!));

        if (registrations is null)
            return;

        // Not needed for now
        // EnsureDependencyTypes(context, registrations);
        EnsureImplementationTypes(context, registrations);
    }

    // private void EnsureDependencyTypes(SymbolAnalysisContext context, IEnumerable<(INamedTypeSymbol, SyntaxReference)> registrations)
    // {
    //     foreach (var registration in registrations)
    //     {
    //         var dependencyType = registration.Item1.TypeArguments.First();

    //         TypeArgumentListSyntax? typeArgumentList = null;

    //         if (dependencyType.IsStatic)
    //         {
    //             typeArgumentList ??= GetTypeArgumentListSyntax(registration.Item2);

    //             context.ReportDiagnostic(Diagnostic.Create(
    //                 AnalyzingRules.DependencyTypeCanNotBeStatic,
    //                 typeArgumentList.Arguments.First()
    //                     .GetLocation()));
    //         }
    //     }
    // }

    private void EnsureImplementationTypes(SymbolAnalysisContext context, IEnumerable<(INamedTypeSymbol, SyntaxReference)> registrations)
    {
        foreach (var registration in registrations)
        {
            var implType = registration.Item1.TypeArguments.Last();

            TypeArgumentListSyntax? typeArgumentList = null;

            // interface is included
            if (implType.IsAbstract)
            {
                typeArgumentList ??= GetTypeArgumentListSyntax(registration.Item2);

                context.ReportDiagnostic(Diagnostic.Create(
                    AnalysisRules.ImplementationTypeMustBeConcrete,
                    typeArgumentList.Arguments.Last()
                        .GetLocation()));
            }
        }
    }

    private static TypeArgumentListSyntax GetTypeArgumentListSyntax(SyntaxReference syntaxReference)
        => ((AttributeSyntax)syntaxReference.GetSyntax()).GetTypeArgumentList();
}
