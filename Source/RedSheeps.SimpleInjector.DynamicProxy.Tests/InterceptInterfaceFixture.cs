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
    public class InterceptInterfaceFixture
    {

        [Fact]
        public void WhenParentInstance()
        {
            var container = new Container();
            container.Intercept<IParent>(typeof(IncrementInterceptor), typeof(DoubleInterceptor));
            container.Register<IParent, Parent>();
            container.Register<Child>();
            container.Register<IncrementInterceptor>();
            container.Verify();

            var instance = container.GetInstance<IParent>();
            Assert.Equal(5, instance.Increment(1));
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

        public interface IParent
        {
            int Increment(int value);
        }

        public class Parent : IParent
        {
            private readonly Child _child;

            public Parent(Child child)
            {
                _child = child;
            }
            public virtual int Increment(int value)
            {
                return _child.Increment(value);
            }
        }

        public class Child
        {
            public virtual int Increment(int value)
            {
                return ++value;
            }
        }
    }
}
