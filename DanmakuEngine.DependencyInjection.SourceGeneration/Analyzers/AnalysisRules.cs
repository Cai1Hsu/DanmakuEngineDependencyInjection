#pragma warning disable RS2008 // Disable analyzer release tracking

using Microsoft.CodeAnalysis;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public static class AnalysisRules
{
    public static readonly DiagnosticDescriptor AnalyzerException = new(
        "DEDI0000",
        title: "Analyzer Exception",
        messageFormat: "An exception occurred while analyzing rule {0} on {1}, message: {1}",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DependencyContainerRule = new(
       "DEDI0001",
       title: "Dependencies MUST be registered on a dependency container",
       messageFormat: "Decorate the class with the [DependencyContainer] attribute",
       description: "",
       category: "Design",
       defaultSeverity: DiagnosticSeverity.Warning,
       isEnabledByDefault: true);


    public static readonly DiagnosticDescriptor ContainerMustBePartial = new(
        "DEDI0002",
        title: "Dependency container class MUST be partial",
        messageFormat: "Add the 'partial' modifier to the class",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContainerCanNotBeStatic = new(
        "DEDI0003",
        title: "Dependency container class CAN NOT be static",
        messageFormat: "Remove the 'static' modifier from the class",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);


    public static readonly DiagnosticDescriptor MultipleRegistration = new(
        "DEDI0004",
        title: "Multiple registration of the same type is not allowed",
        messageFormat: "Remove the duplicate registration",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ImplementationTypeMustBeConcrete = new(
        "DEDI0006",
        title: "Implementation type must be concrete",
        messageFormat: "Implementation type can not be an interface or abstract class",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}