namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class TransientAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class TransientAttribute<TDependency, TImplementation> : Attribute
    where TImplementation : TDependency
{
}
