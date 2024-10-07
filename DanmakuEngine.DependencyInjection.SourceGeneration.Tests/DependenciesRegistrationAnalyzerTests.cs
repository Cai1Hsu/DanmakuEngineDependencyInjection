using Verifier = DanmakuEngine.DependencyInjection.SourceGeneration.Tests.Verifiers.CSharpAnalyzerVerifier<DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers.DependencyRegistrationAnalyzer>;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Tests;

public class DependenciesRegistrationAnalyzerTests
{
    [Test]
    public async Task Flag_WhenRegisterOnNonDependencyContainer()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [{|#0:Singleton<MyContainer>|}]
                            partial class MyContainer
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(DependencyRegistrationAnalyzer.DependencyContainerRule).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task No_Error_When_Not_Mixing()
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
}