namespace DanmakuEngine.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class SingletonAttribute<TDependency> : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class SingletonAttribute<TDependency, TImplementation> : SingletonAttribute<TDependency>
{
}
