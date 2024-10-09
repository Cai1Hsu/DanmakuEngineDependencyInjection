using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class MultipleRegistrationRule : IContainerClassAnalysisRule
{
    public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(AnalysisRules.MultipleRegistration);

    public bool RequiredToBeContainer => true;

    public void AnalyzeSymbol(SymbolAnalysisContext context, bool isContainer)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        ImmutableArray<AttributeData> attributes = namedTypeSymbol.GetAttributes();

        // Get the generic type parameters of all attributes
        var registrationAttributes = attributes.Where(a => a.AttributeClass is not null
            && DependencyRegistrationRule.RegistrationAttributes.Contains(a.AttributeClass.ToGlobalPrefixedFullName()));

        if (registrationAttributes is null || !registrationAttributes.Any())
            return;

        EnsureNoDuplicateRegistration(context, registrationAttributes);
    }

    private void EnsureNoDuplicateRegistration(SymbolAnalysisContext context, IEnumerable<AttributeData> registrationAttributes)
    {
        var registrations = new HashSet<string>();

        foreach (var attribute in registrationAttributes)
        {
            var typeArguments = attribute.AttributeClass!.TypeArguments;

            if (typeArguments.Length == 0)
                continue;

            var dependencyType = typeArguments.First().ToGlobalPrefixedFullName();

            if (registrations.Contains(dependencyType))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    AnalysisRules.MultipleRegistration,
                    attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()));
            }
            else
            {
                registrations.Add(dependencyType);
            }
        }
    }
}