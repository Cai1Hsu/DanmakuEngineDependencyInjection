using System.Formats.Asn1;
using Verifier = DanmakuEngine.DependencyInjection.SourceGeneration.Tests.Verifiers.CSharpAnalyzerVerifier<DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers.ContainerClassAnalyzer>;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Tests;

[TestFixture]
public class RegisterationTypeRuleTests
{
    [Test]
    public async Task Flag_WhenImplementationTypeIsAbstract()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<IService, {|#0:AbstractClass|}>]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }

                            interface IService
                            {
                            }

                            abstract class AbstractClass : IService
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(AnalysisRules.ImplementationTypeMustBeConcrete).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task Flag_WhenImplementationTypeIsInterface_HasSingleTypeArgument()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<{|#0:IService|}>]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }

                            interface IService
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(AnalysisRules.ImplementationTypeMustBeConcrete).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task Flag_WhenImplementationTypeIsInterface_HasMultipleTypeArguments()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<IService, {|#0:IDerivedService|}>]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }

                            interface IService
                            {
                            }

                            interface IDerivedService : IService
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(AnalysisRules.ImplementationTypeMustBeConcrete).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }
}