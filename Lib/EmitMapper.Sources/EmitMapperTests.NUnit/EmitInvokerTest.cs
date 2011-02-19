using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EmitMapper.EmitBuilders;
using EmitMapper;
using System.Diagnostics;
using EmitMapper.EmitInvoker;
using System.Reflection;

namespace EmitMapperTests
{
    [TestFixture]
    public class EmitInvokerTest
    {
        [Test]
        public void EmitInvokerTest_TestCall1()
        {
            int i = 0;
            var caller = (DelegateInvokerAction_0)DelegateInvoker.GetDelegateInvoker((Action)(() => i++) );
            caller.CallAction();
            caller.CallAction();
            caller.CallAction();
            Assert.AreEqual(3, i);

            var caller2 = (DelegateInvokerAction_0)DelegateInvoker.GetDelegateInvoker((Action)(() => i += 2));
            caller2.CallAction();
            caller2.CallAction();
            Assert.AreEqual(7, i);
        }

        [Test]
        public void EmitInvokerTest_TestCall2()
        {
            var caller = (DelegateInvokerFunc_0)DelegateInvoker.GetDelegateInvoker((Func<int>)(() => 3));
            Assert.AreEqual(3, caller.CallFunc());

            var caller2 = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker((Func<int, int, int>)((l, r) => l + r));
            //DynamicAssemblyManager.SaveAssembly();
            Assert.AreEqual(5, caller2.CallFunc(2, 3));
        }

        public int InvokeTest1()
        {
            return 3;
        }

        public static int InvokeTest2(int par)
        {
            return par;
        }

        [Test]
        public void EmitInvokerTest_TestCall3()
        {
            var caller = (MethodInvokerFunc_0)MethodInvoker.GetMethodInvoker(this, GetType().GetMethod("InvokeTest1"));
            Assert.AreEqual(3, caller.CallFunc());

            var caller2 = (MethodInvokerFunc_1)MethodInvoker.GetMethodInvoker(this, GetType().GetMethod("InvokeTest2", BindingFlags.Static | BindingFlags.Public));
            Assert.AreEqual(5, caller2.CallFunc(5));

        }

    }
}
