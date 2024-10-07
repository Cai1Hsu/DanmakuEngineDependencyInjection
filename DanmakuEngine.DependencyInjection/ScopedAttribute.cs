namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class ScopedAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class ScopedAttribute<TDependency, TImplementation> : Attribute
    where TImplementation : TDependency
{
}
