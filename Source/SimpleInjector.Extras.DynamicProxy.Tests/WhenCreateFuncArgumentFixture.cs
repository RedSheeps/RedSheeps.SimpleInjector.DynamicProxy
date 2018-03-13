using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Xunit;

namespace SimpleInjector.Extras.DynamicProxy.Tests
{
    public class WhenCreateFuncArgumentFixture
    {
        [Fact]
        public void WhenWeaving()
        {
            var container = new Container();
            container.InterceptWith(x => true, () => new IncrementInterceptor());
            container.Register<Target>();
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(3, instance.Increment(1));
        }

        [Fact]
        public void WhenNotWeaving()
        {
            var container = new Container();
            container.Register<Target>();
            container.InterceptWith(x => false, () => new IncrementInterceptor());
            container.Verify();

            var instance = container.GetInstance<Target>();
            Assert.Equal(2, instance.Increment(1));
        }


        public class IncrementInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = ((int)invocation.ReturnValue) + 1;
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
