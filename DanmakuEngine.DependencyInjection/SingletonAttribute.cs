namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class SingletonAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class SingletonAttribute<TDependency, TImplementation> : SingletonAttribute<TDependency>
{
}
