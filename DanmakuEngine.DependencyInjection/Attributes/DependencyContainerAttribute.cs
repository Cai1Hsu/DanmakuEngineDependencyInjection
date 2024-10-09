namespace DanmakuEngine.DependencyInjection;

// It's not necessary to decorate a marker interface with this attribute, but it's possible.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
public class DependencyContainerAttribute : Attribute
{
}
