using Verifier = DanmakuEngine.DependencyInjection.SourceGeneration.Tests.Verifiers.CSharpAnalyzerVerifier<DanmakuEngine.DependencyInjection.SourceGeneration.Analyzers.ContainerClassAnalyzer>;

namespace DanmakuEngine.DependencyInjection.SourceGeneration.Tests;

public class AllowMarkerTypeTests
{
    [Test]
    public async Task AllowInterfaceAsMarkerType()
    {
        const string text = """
                            #pragma warning disable DEDI0001

                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<TestObject>]
                            interface IDependencyBase
                            {
                            }

                            class TestObject
                            {
                                public TestObject()
                                {}
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task AllowAbstractClassAsMarkerType()
    {
        const string text = """
                            #pragma warning disable DEDI0001
                            
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<TestObject>]
                            abstract class DependencyBase
                            {
                            }

                            class TestObject
                            {
                                public TestObject()
                                {}
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task AllowInterfaceAsMarkerType_WithAttribute()
    {
        const string text = """
                            #pragma warning disable DEDI0001

                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<TestObject>]
                            [DependencyContainer]
                            interface IDependencyBase
                            {
                            }

                            class TestObject
                            {
                                public TestObject()
                                {}
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Test]
    public async Task AllowAbstractClassAsMarkerType_WithAttribute()
    {
        const string text = """
                            #pragma warning disable DEDI0001
                            
                            using DanmakuEngine.DependencyInjection;
                            
                            [Singleton<TestObject>]
                            [DependencyContainer]
                            abstract class DependencyBase
                            {
                            }

                            class TestObject
                            {
                                public TestObject()
                                {}
                            }
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }
}