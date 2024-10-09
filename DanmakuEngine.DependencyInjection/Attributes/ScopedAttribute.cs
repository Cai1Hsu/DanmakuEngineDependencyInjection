namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class ScopedAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class ScopedAttribute<TDependency, TImplementation> : Attribute
    where TImplementation : TDependency
{
}
