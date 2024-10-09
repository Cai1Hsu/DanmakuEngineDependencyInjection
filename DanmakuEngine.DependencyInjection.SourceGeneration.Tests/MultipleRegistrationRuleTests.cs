using Verifier = DanmakuEngine.DependencyInjection.SourceGeneration.Tests.Verifiers.CSharpAnalyzerVerifier<DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers.ContainerClassAnalyzer>;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Tests;

public class MultipleRegistrationRuleTests
{
    [Test]
    public async Task Flag_WhenMultipleRegistration()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<Dependency>]
                            [{|#0:Singleton<Dependency>|}]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }

                            class Dependency
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(AnalysisRules.MultipleRegistration).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task No_Error_WhenNotMultipleRegistration()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<MyContainer>]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task OnlyConsiderImplementType_NoErrorOnlyImplementationTypeAreSame()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<Dependency>]
                            [Singleton<IDependency, Dependency>]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }

                            interface IDependency
                            {
                            }

                            class Dependency : IDependency
                            {
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task OnlyConsiderImplementType_FlagWhenMultipleRegistrationWithDifferentImplementations()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<DependencyBase, Dependency1>]
                            [{|#0:Singleton<DependencyBase, Dependency2>|}]
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }

                            class DependencyBase
                            {
                            }

                            class Dependency1 : DependencyBase
                            {
                            }

                            class Dependency2 : DependencyBase
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(AnalysisRules.MultipleRegistration).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }
}