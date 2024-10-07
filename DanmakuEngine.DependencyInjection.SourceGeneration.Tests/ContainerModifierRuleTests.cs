using Verifier = DanmakuEngine.DependencyInjection.SourceGeneration.Tests.Verifiers.CSharpAnalyzerVerifier<DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers.ContainerClassAnalyzer>;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Tests;

public class ContainerModifierRuleTests
{
    [Test]
    public async Task Flag_WhenNoPartialModifier()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [DependencyContainer]
                            class {|#0:MyContainer|}
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(ContainerModifierRule.ContainerMustBePartial).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task Flag_WhenContainingClassIsNotPartial()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            class {|#0:Foo|}
                            {
                                [DependencyContainer]
                                partial class MyContainer
                                {
                                }
                            }
                            """;

        var expected = Verifier.Diagnostic(ContainerModifierRule.ContainerMustBePartial).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task No_Error_WhenPartialModifierAppliedForAllClasses()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            partial class Foo
                            {
                                [DependencyContainer]
                                partial class MyContainer
                                {
                                }
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task No_Error_WhenPartialModifier()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task Flag_WhenStaticModifierAdded()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [DependencyContainer]
                            {|#0:static|} partial class MyContainer
                            {
                            }
                            """;

        var expected = Verifier.Diagnostic(ContainerModifierRule.ContainerCanNotBeStatic).WithLocation(0);

        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }

    [Test]
    public async Task No_Error_WhenStaticModifierNotAdded()
    {
        const string text = """
                            using DanmakuEngine.DependencyInjection;
                            
                            [DependencyContainer]
                            partial class MyContainer
                            {
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }
}