﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using SimpleInjector;
using Xunit;

namespace RedSheeps.SimpleInjector.DynamicProxy.Tests
{
    public class WhenCreateWithEventArgsFuncArgumentFixture
    {
        [Fact]
        public void WhenWeaving()
        {
            var container = new Container();
            container.InterceptWith(x => true, e =>
            {
                Assert.NotNull(e);
                return new IncrementInterceptor();
            });
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
            container.InterceptWith(x => false, _ => new IncrementInterceptor());
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

        public class Target
        {
            public virtual int Increment(int value)
            {
                return ++value;
            }
        }
    }
}
