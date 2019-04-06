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
    public class WhenMethodTypeParameterFixture
    {
        [Fact]
        public void WhenCreateClassProxyWithTarget()
        {
            var container = new Container();
            container.Register<IncrementInterceptor>();
            container.InterceptWith<IncrementInterceptor>(x => x != typeof(IncrementInterceptor));
            container.Register<NotImplementInterface>();
            container.Verify();

            var instance = container.GetInstance<NotImplementInterface>();
            Assert.Equal(3, instance.Increment(1));
        }

        [Fact]
        public void WhenCreateInterfaceProxyWithTarget()
        {
            var container = new Container();
            container.Register<IncrementInterceptor>();
            container.Register<IInterface, ImplementInterface>();
            container.InterceptWith<IncrementInterceptor>(x => x != typeof(IncrementInterceptor));
            container.Verify();

            var instance = container.GetInstance<IInterface>();
            Assert.Equal(3, instance.Increment(1));
        }

        [Fact]
        public void WhenNotWeaving()
        {
            var container = new Container();
            container.Register<IncrementInterceptor>();
            container.Register<NotImplementInterface>();
            container.InterceptWith<IncrementInterceptor>(x => false);
            container.Verify();

            var instance = container.GetInstance<NotImplementInterface>();
            Assert.Equal(2, instance.Increment(1));
        }

        [Fact]
        public void WhenNotRegisterdInterceptor()
        {
            var container = new Container();
            container.InterceptWith<IncrementInterceptor>(x => true);
            container.Register<NotImplementInterface>();
            Assert.Throws<InvalidOperationException>(() => container.Verify());
        }

        public class IncrementInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = (int)invocation.ReturnValue + 1;
            }
        }

        public class NotImplementInterface
        {
            public virtual int Increment(int value)
            {
                return ++value;
            }
        }

        public interface IInterface
        {
            int Increment(int value);
        }

        public class ImplementInterface : IInterface
        {
            public virtual int Increment(int value)
            {
                return ++value;
            }
        }
    }
}
