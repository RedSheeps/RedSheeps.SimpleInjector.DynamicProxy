# SimpleInjector.Extras.DynamicProxy

## About

SimpleInjector extension for enabling AOP in conjunction with Castle. A library for easily applying DynamicProxy of Catsle.Core to SimpleInjector.

```cs
var container = new Container();

container.InterceptWith<IncrementInterceptor>(x => x == typeof(Target));

container.Register<Target>();
container.Register<IncrementInterceptor>();
container.Verify();

var target = container.GetInstance<Target>();
```

In the lambda expression of the argument, specify the condition of Type to which the Interceptor is applied. Interceptor also resolves instances with Container. 

Therefore, it is possible to inject to the Interceptor.

You can choose how to apply the Interceptor. Here is a simple example.

```cs
// Apply Interceptor by specifying Types.
container.InterceptWith<IncrementInterceptor>(x => x == typeof(Target));
container.InterceptWith(x => x == typeof(Target), typeof(FirstInterceptor), typeof(SecondInterceptor));

// Specify the instance directly and apply.
container.InterceptWith(x => true, new FirstInterceptor());
container.InterceptWith(x => true, new FirstInterceptor(), new SecondInterceptor());

// Specify and apply Func to create an instance.
container.InterceptWith(x => true, () => new FirstInterceptor());
container.InterceptWith(x => true, () => new IInterceptor[] {new FirstInterceptor(), new SecondInterceptor()});

// Apply ExpressionBuiltEventArgs with Func which takes an argument when creating an instance.
container.InterceptWith(x => true, e => new FirstInterceptor());
container.InterceptWith(x => true, e => new IInterceptor[] {new FirstInterceptor(), new SecondInterceptor()});
```

## NuGet

https://www.nuget.org/packages/SimpleInjector.Extras.DynamicProxy/
