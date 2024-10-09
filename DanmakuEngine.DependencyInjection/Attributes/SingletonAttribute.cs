namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class SingletonAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public class SingletonAttribute<TDependency, TImplementation> : Attribute
    where TImplementation : TDependency
{
}
