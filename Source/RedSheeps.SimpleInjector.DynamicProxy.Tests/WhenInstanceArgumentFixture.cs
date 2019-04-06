using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using SimpleInjector;
using Xunit;

namespace RedSheeps.SimpleInjector.DynamicProxy.Tests
{
    public class WhenInstanceArgumentFixture
    {
        [Fact]
        public void WhenSingleInstance()
        {
            var container = new Container();
            container.InterceptWith(x => true, new IncrementInterceptor());
            container.Register<Target>();
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(3, instance.Increment(1));
        }

        [Fact]
        public void WhenMultiInstance()
        {
            var container = new Container();
            container.InterceptWith(x => true, new IncrementInterceptor(), new DoubleInterceptor());
            container.Register<Target>();
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(5, instance.Increment(1));
        }

        [Fact]
        public void WhenNotWeaving()
        {
            var container = new Container();
            container.Register<Target>();
            container.InterceptWith(x => false, new IncrementInterceptor());
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(2, instance.Increment(1));
        }


        public class IncrementInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = (int)invocation.ReturnValue + 1;
            }
        }

        public class DoubleInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = (int)invocation.ReturnValue * 2;
            }
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
