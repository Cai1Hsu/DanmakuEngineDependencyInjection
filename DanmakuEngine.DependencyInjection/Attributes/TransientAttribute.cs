namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class TransientAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class TransientAttribute<TDependency, TImplementation> : Attribute
    where TImplementation : TDependency
{
}
