using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Xunit;

namespace SimpleInjector.Extras.DynamicProxy.Tests
{
    public class WhenPassingTypeAsArgumentFixture
    {
        [Fact]
        public void WhenSingleInstance()
        {
            var container = new Container();
            container.InterceptWith(x => x == typeof(Target), typeof(IncrementInterceptor));
            container.Register<Target>();
            container.Register<IncrementInterceptor>();
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(3, instance.Increment(1));
        }

        [Fact]
        public void WhenMultiInstance()
        {
            var container = new Container();
            container.InterceptWith(x => x == typeof(Target), typeof(IncrementInterceptor), typeof(DoubleInterceptor));
            container.Register<Target>();
            container.Register<IncrementInterceptor>();
            container.Register<DoubleInterceptor>();
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(5, instance.Increment(1));
        }

        [Fact]
        public void WhenNotWeaving()
        {
            var container = new Container();
            container.Register<Target>();
            container.InterceptWith(x => false, typeof(IncrementInterceptor));
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(2, instance.Increment(1));
        }

        [Fact]
        public void WhenNotInterceptorTypeWeaving()
        {
            var container = new Container();
            Assert.Throws<ArgumentException>(() => container.InterceptWith(x => x == typeof(Target), typeof(NotInterceptorType)));
        }

        public class IncrementInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = ((int)invocation.ReturnValue) + 1;
            }
        }

        public class DoubleInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = ((int)invocation.ReturnValue) * 2;
            }
        }

        public class NotInterceptorType
        {
        }

        public class Target
        {
            public virtual int Increment(int value)
            {
                return ++value;
            }
        }
    }
}
