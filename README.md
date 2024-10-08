# DanmakuEngine.DependencyInjection

A source generator based dependency injection library for C#.

**Still under heavy development.**

## Usages

### Registering dependencies

#### Compile-time registration

This library is designed to be used with the source generator. You can register a dependency by decorating the class with the `Singleton`, `Scoped`, or `Transient` attribute. The class must be decorated  with `DependencyContainer` attribute and be marked as `partial`.

```CSharp
[Singleton<IDependency, DependencyImpl>]
[Scoped<Dependency>]
[DependencyContainer]
partial class MyDependencyContainer
{
}
```

See [Lifetime span attributes](#lifetime-span-attributes) for more information.

##### Custom factory method

In the container class, you can define a custom factory method to create an instance of a class.

```CSharp
[DependencyContainer]
partial class MyDependencyContainer
{
    [Singleton<Dependency4>]
    private static Dependency4 CreateDependency4(IReadOnlyDependencyContainer container)
    {
        return new Dependency4(container.Resolve<IDependency>());
    }
}
```

The method must be static and return the instance of the class you want to create. The method must only has one parameter, which is the `IReadOnlyDependencyContainer` reference.

#### Runtime registration

##### Manual registration

You can register a dependency by calling the `Register[Lifetime]` method on a container.

```CSharp
Container.RegisterSingleton<IDependency, DependencyImpl>(_ => new DependencyImpl());
Container.RegisterScoped<Dependency>(c => new Dependency(c.Resolve<IDependency>()));
```

Easy to understand, like most of the IoC containers.

This basically does the same thing as the compile-time registration, but it is done at runtime.

For auto-wiring, you should use the compile-time registration.

##### Cache an existing instance

This mostly used only when you uses a container as a consumer. See [Container as Consumer](#container-as-consumer) for more information.

Since you can only cache a single instance of a class, so all cached instances are implicitly singletons.

You can cache an existing instance by calling the `Cache` Method on a container.

```CSharp
Container.Cache<DependencyImpl>(new DependencyImpl());
```

The generic type parameter can be omitted, and the method will use the type of the instance as the key. So you can write:

```CSharp
Container.Cache(new DependencyImpl());
```

Also, you can cache an instance as a interface type.

```CSharp
Container.CacheAs<IDependency, DependencyImpl>(new DependencyImpl());
```

Also, the second generic type parameter can be omitted. So you can write:

```CSharp
Container.CacheAs<IDependency>(new DependencyImpl());
```

##### Cache/CacheAs Attribute

You can cache an instance by decorating the class with the `Cache` or `CacheAs` attribute.

```CSharp
partial class MyDependencyContainer
{
    [Cache]
    private readonly DependencyImpl _dependencyImpl = new DependencyImpl();

    [CacheAs<IDependency>]
    private readonly DependencyImpl _dependencyImpl = new DependencyImpl();
}
```

An `CacheMembers` method is generated by the source generator. You can call this method to cache all the instances that are decorated with the `Cache` or `CacheAs` attribute.

### Resolving dependencies

#### Manual resolving

You can resolve a dependency by calling the `Resolve` method on a container.

```CSharp
IDependency d1 = Container.Resolve<IDependency>();
object d2 = Container.Resolve(typeof(IDependency));
```

#### Constructor injection(Auto-wiring)

If the class is registered with the container and satisfies some specific conditions, the container can automatically inject dependencies into the constructor.

The conditions are:
1. The constructor you want to use with auto-wiring must be decorated with the `AutoWired` attribute, or the class only has one constructor.
2. All parameter dependencies of the selected constructor must be registered with the container.

#### Field/Property injection

If the class is decorated with `RequireDependency` attribute, you can inject dependencies into fields or properties by decorating them with the `Inject` attribute.

```CSharp
[RequireDependency]
partial class Consumer
{
    public IReadOnlyDependencyContainer Dependencies => AnotherContainer;

    // The dependency will be injected into this field during activation.
    [Inject]
    private IDependency _dependency;

    public Consumer()
    {
        RegisterForActivation().Activate();
    }
}
```

You don't have to call `RegisterForActivation` in the constructor, you can do it in elsewhere like `Start` or `load` method of the framework you are using. But you must call the `Activate` method to so that the instance can be activated.

See [IRequireDependency](#irequiredependency) for more information.

### Container as Consumer

Usually, you will want to use the container as a consumer. You can decorate the container class with the `RequireDependency` attribute. And the container is implemented as a comsumer who comsumes dependencies from it self.

This is automatically done by the source generator. No other code is needed.

```CSharp
[Singleton<IDependency, DependencyImpl>]
[RequireDependency]
[DependencyContainer]
partial class MyDependencyContainer
{
    public MyDependencyContainer()
    {
        Resolve<IDependency>();
    }
}
```

But you can also implement the `IRequireDependency` interface manually. Especially when you want to merge the container with another container, or you want to use the container as a consumer in a different way.

```CSharp
[DependencyContainer]
partial class MyDependencyContainer : IRequireDependency
{
    public IReadOnlyDependencyContainer Dependencies => this.MergeWith(AnotherContainer);

    public MyDependencyContainer()
    {
        // Resolving dependency from the `AnotherContainer`
        Resolve<IDependency>();
    }
}
```

## Interfaces

### IReadOnlyDependencyContainer

Marks that a class is a read-only dependency container, which means that it can be used to resolve dependencies but not to register new ones.

```CSharp
public interface IReadOnlyDependencyContainer
{
    T Resolve<T>();
    object Resolve(Type type);
}
```

### IDependencyContainer

Marks that a class is a dependency container, which means that it can be used to resolve dependencies, register new ones, and merge with another read-only dependency container.

To register a new dependency, use the `Singleton`, `Scoped`, or `Transient` attributes to decorate the class that you want to register.

```CSharp
[Transient<IDependency, DependencyImpl>]
[Singleton<Dependency>]
[DependencyContainer]
partial class MyDependencyContainer
{
}
```

You don't have to explicitly implement this interface, but you should decorate the class with the `DependencyContainer` attribute. The source generator will generate the implementation for you. Please make sure that the class is marked as `partial`.

### IRequireDependency

`Dependencies` property is used to get the `IReadOnlyDependencyContainer` reference.

`RegisterForActivation` method is used to register this instance for activation. The method will returns an activator, which can be used to activate the instance. Call the `Activate` method on the activator to activate the instance.

When you decorate a class with the `RequireDependency` attribute, the source generator will generate the `IRequireDependency` inheritance for the class.

You will have to implement the `Dependencies` property but not the `RegisterForActivation` method. The `RegisterForActivation` is generated by the source generator.

Then you should call the `RegisterForActivation` method and activate the instance by calling the `Activate` method.

```CSharp
[RequireDependency]
partial class Consumer
{
    public IReadOnlyDependencyContainer Dependencies => AnotherContainer;

    public Consumer()
    {
        RegisterForActivation().Activate();
    }
}
```

#### What is registration for activation?

Words is cheap, show me the code.

```CSharp
CandicateActivator IRequireDependency.RegisterForActivation()
{
    return new CandicateActivator
    {
        ActivationAction = delegate
        {
            _dependency1 = Dependencies.Resolve<IDependency>();
            _dependency2 = Dependencies.Resolve<AnotherDependency>();
        },
    }
}
```

When you call `Activate` method, the `ActivationAction` will be executed. The `ActivationAction` is a delegate that contains the code that you want to execute when the instance is activated.

If you are not using *Field/Poroperty injection*, you don't have to call `RegisterForActivation` method. The generated code will simply return `null!`;

## Attributes

### DependencyContainer

Marks that a class is a dependency container.

```CSharp
[DependencyContainer]
partial class MyDependencyContainer
{
}
```

This attribute is inherited by child classes.

### RequireDependency

Marks that a class requires dependencies. The source generator will generate the `IRequireDependency` inheritance for the class. You must implement the `Dependencies` property.

Then, you can resolve dependencies by calling the `Resolve` method on the `Dependencies` property. Or you can use the `Inject` attribute to inject dependencies into fields or properties.

See [Field/Property injection](#fieldproperty-injection) for more information.

### Lifetime span attributes

The `Singleton`, `Scoped`, and `Transient` attributes are used to register a class with a specific lifetime span.

#### Singleton

Marks that a dependency is a singleton. The dependency will be created only once and will be shared across all consumers and consumers who use a container which is merged with the container that registered the dependency.

```CSharp
[Singleton<Dependency1>]
[DependencyContainer]
partial class DependencyConsumer
{
}
```

```CSharp
Container1.MergeWith(Container2);

Assert.AreSame(Container1.Resolve<Dependency1>(),
    Container2.Resolve<Dependency1>()); // true
```

#### Transient

Marks that a dependency is transient. The dependency will be created every time it is resolved no matter if it is resolved by the same consumer or different consumers.

```CSharp
[Transient<Dependency2>]
[DependencyContainer]
partial class DependencyConsumer
{
}
```

```CSharp
Assert.AreSame(Container1.Resolve<Dependency2>(),
    Container1.Resolve<Dependency2>()); // false
```

#### Scoped

Marks that a dependency is scoped. The dependency will be created once per scope and will be shared across all consumers within the same scope.

Every `IReadOnlyDependencyContainer` is a *Scope*, and the `IDependencyContainer` is the *Root Scope*.

When a container is merged with another container, the merged container becomes a new scope. Because the merged container is `IReadOnlyDependencyContainer`, and the dependencies registered in the merged container are shared across all consumers who use the merged container.

```CSharp
[Scoped<Dependency3>]
[DependencyContainer]
partial class DependencyConsumer
{
}
```

```CSharp
Container1.MergeWith(Container2);

Assert.AreSame(Container1.Resolve<Dependency3>(),
    Container2.Resolve<Dependency3>()); // false

Assert.AreSame(Container1.Resolve<Dependency3>(),
    Container1.Resolve<Dependency3>()); // true

Assert.AreSame(Container2.Resolve<Dependency3>(),
    Container2.Resolve<Dependency3>()); // true
```

We also provided a `BeginScope` method to create a new scope. This is like merging the container with an empty container.

```CSharp
using (var scope = Container.BeginScope())
{
    Assert.AreSame(Container.Resolve<Dependency3>(),
        scope.Resolve<Dependency3>()); // false
} // Dispose the scope, and all scoped dependencies.
```

The scope is actually a `IReadOnlyDependencyContainer`, so you can use it to resolve dependencies and probably merge it with another container.
