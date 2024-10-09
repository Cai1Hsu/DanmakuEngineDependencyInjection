#pragma warning disable RS2008 // Disable analyzer release tracking

using Microsoft.CodeAnalysis;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers;

public static class AnalyzingRules
{

    public static readonly DiagnosticDescriptor DependencyContainerRule = new(
       "DEDI0001",
       title: "Dependencies MUST be registered on a dependency container",
       messageFormat: "Decorate the class with the [DependencyContainer] attribute",
       description: "",
       category: "Design",
       defaultSeverity: DiagnosticSeverity.Warning,
       isEnabledByDefault: true);


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


    public static DiagnosticDescriptor MultipleRegistration = new(
        "DEDI0004",
        title: "Multiple registration of the same type is not allowed",
        messageFormat: "Remove the duplicate registration",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DependencyTypeCanNotBeStatic = new(
        "DEDI0005",
        title: "Dependency type can not be static",
        messageFormat: "Dependency type can not be static",
        description: "",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

}