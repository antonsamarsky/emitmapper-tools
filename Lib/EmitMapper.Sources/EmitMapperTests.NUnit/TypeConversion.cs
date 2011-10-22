using NUnit.Framework;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace EmitMapperTests
{
    [TestFixture]
    public class TypeConversion
    {
        public class A1
        {
            public int fld1;
            public string fld2;
        }

        public class B1
        {
            public decimal fld1 = 15;
            public decimal fld2 = 11;
        }

        public class A2
        {
            public string[] fld3;
        }

        public class B2
        {
            public int fld3 = 99;
        }

        public class A3
        {
            public A1 a1 = new A1();
            public A2 a2 = new A2();
        }

        public class B3
        {
            public A1 a1 = new A1();
            public A2 a2 = new A2();
        }

        public class A4
        {
            private string m_str;
            public string str
            {
                set
                {
                    m_str = value;
                }
                get
                {
                    return m_str;
                }
            }
        }

        public class B4
        {
            public class BInt
            {
                public override string ToString()
                {
                    return "string";
                }
            }


            BInt m_bint = new BInt();
            public BInt str
            {
                get
                {
                    return m_bint;
                }
            }
        }

        public class A5
        {
            public string fld2;
        }

        public class B5
        {
            public decimal fld2 = 11;
        }

        [Test]
        public void Test1()
        {
            A1 a = new A1();
            B1 b = new B1();
            var mapper = Context.objMan.GetMapper<B1, A1>();
			//DynamicAssemblyManager.SaveAssembly();
            mapper.Map(b, a);
            Assert.AreEqual(a.fld1, 15);
            Assert.AreEqual(a.fld2, "11");
        }

        [Test]
        public void Test2()
        {
            A2 a = new A2();
            B2 b = new B2();
            Context.objMan.GetMapper<B2, A2>().Map(b, a);
			//DynamicAssemblyManager.SaveAssembly();
            Assert.AreEqual("99", a.fld3[0]);
        }

        [Test]
        public void Test3_ShallowCopy()
        {
            A3 a = new A3();
            B3 b = new B3();
            b.a1.fld1 = 15;
            Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().DeepMap()).Map(b, a);
            Assert.AreEqual(a.a1.fld1, 15);
            b.a1.fld1 = 666;
            Assert.AreEqual(a.a1.fld1, 15);

            Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap()).Map(b, a);
            b.a1.fld1 = 777;
            Assert.AreEqual(777, a.a1.fld1);

            b = new B3();
            Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap<A1>().DeepMap<A2>()).Map(b, a);
            b.a1.fld1 = 333;
            b.a2.fld3 = new string[1];

            Assert.AreEqual(333, a.a1.fld1);
            Assert.IsNull(a.a2.fld3);
        }

        [Test]
        public void Test4()
        {
            A4 a = new A4();
            B4 b = new B4();
            Context.objMan.GetMapper<B4, A4>().Map(b, a);
            Assert.AreEqual(a.str, "string");
        }
    }
}