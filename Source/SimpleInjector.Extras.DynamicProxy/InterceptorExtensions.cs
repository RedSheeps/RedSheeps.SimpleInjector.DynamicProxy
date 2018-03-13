using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;

namespace SimpleInjector.Extras.DynamicProxy
{
    public static class InterceptorExtensions
    {
        public static void InterceptWith<TInterceptor>(
            this Container container, Predicate<Type> predicate)
            where TInterceptor : class, IInterceptor
        {
            container.Options.ConstructorResolutionBehavior.GetConstructor(typeof(TInterceptor));

            var interceptWith = new InterceptionHelper()
            {
                BuildInterceptorExpression =
                    e => BuildInterceptorExpression<TInterceptor>(container),
                Predicate = predicate
            };

            container.ExpressionBuilt += interceptWith.OnExpressionBuilt;
        }

        public static void InterceptWith(
            this Container container, Predicate<Type> predicate, Func<IInterceptor> interceptorCreator)
        {
            var interceptWith = new InterceptionHelper()
            {
                BuildInterceptorExpression =
                    e => Expression.Invoke(Expression.Constant(interceptorCreator)),
                Predicate = predicate
            };

            container.ExpressionBuilt += interceptWith.OnExpressionBuilt;
        }

        public static void InterceptWith(
            this Container container, Predicate<Type> predicate, Func<ExpressionBuiltEventArgs, IInterceptor> interceptorCreator)
        {
            var interceptWith = new InterceptionHelper()
            {
                BuildInterceptorExpression = e => Expression.Invoke(
                    Expression.Constant(interceptorCreator),
                    Expression.Constant(e)),
                Predicate = predicate
            };

            container.ExpressionBuilt += interceptWith.OnExpressionBuilt;
        }

        public static void InterceptWith(
            this Container container, Predicate<Type> predicate, IInterceptor interceptor)
        {
            var interceptWith = new InterceptionHelper()
            {
                BuildInterceptorExpression = e => Expression.Constant(interceptor),
                Predicate = predicate
            };

            container.ExpressionBuilt += interceptWith.OnExpressionBuilt;
        }

        private static Expression BuildInterceptorExpression<TInterceptor>(
            Container container)
            where TInterceptor : class
        {
            var interceptorRegistration = container.GetRegistration(typeof(TInterceptor));

            if (interceptorRegistration == null)
            {
                // This will throw an ActivationException
                container.GetInstance<TInterceptor>();
            }

            return interceptorRegistration.BuildExpression();
        }

        private class InterceptionHelper
        {
            private static readonly ProxyGenerator Generator = new ProxyGenerator();

            private static readonly Func<Type, object, IInterceptor, object> CreateClassProxyWithTarget =
                (p, t, i) => Generator.CreateClassProxyWithTarget(p, t, i);

            private static readonly Func<Type, object, IInterceptor, object> CreateInterfaceProxyWithTarget =
                (p, t, i) => Generator.CreateInterfaceProxyWithTarget(p, t, i);

            internal Func<ExpressionBuiltEventArgs, Expression> BuildInterceptorExpression { private get; set; }
            internal Predicate<Type> Predicate { private get; set; }

            public void OnExpressionBuilt(object sender, ExpressionBuiltEventArgs e)
            {
                if (Predicate(e.RegisteredServiceType))
                {
                    e.Expression = BuildProxyExpression(e);
                }
            }

            private Expression BuildProxyExpression(ExpressionBuiltEventArgs e)
            {
                var expr = BuildInterceptorExpression(e);

                var createProxy =
                    e.RegisteredServiceType.GetTypeInfo().IsInterface ?
                    CreateInterfaceProxyWithTarget :
                    CreateClassProxyWithTarget;

                return Expression.Convert(
                    Expression.Invoke(Expression.Constant(createProxy),
                        Expression.Constant(e.RegisteredServiceType, typeof(Type)),
                        e.Expression,
                        expr),
                    e.RegisteredServiceType);
            }
        }

    }
}
