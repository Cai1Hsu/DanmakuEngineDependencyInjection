#pragma warning disable RS2008 // Enable analyzer release tracking

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public class MultipleRegistrationRule : ContainerClassAnalyzingRule
{
    public static DiagnosticDescriptor MultipleRegistration = new(
        "DEDI0004",
        title: "Multiple registration of the same type is not allowed",
        messageFormat: "Remove the duplicate registration",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override bool RequiredToBeContainer => true;

    public override void AnalyzeSymbol(SymbolAnalysisContext context, bool IsContainer)
    {
        INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        ImmutableArray<AttributeData> attributes = namedTypeSymbol.GetAttributes();

        // Get the generic type parameters of all attributes
        var registrationAttributes = attributes.Where(a => a.AttributeClass is not null
            && RegistrationRule.RegistrationAttributes.Contains(a.AttributeClass.ToGlobalPrefixedFullName()));

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
                    MultipleRegistration,
                    attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()));
            }
            else
            {
                registrations.Add(dependencyType);
            }
        }
    }
}