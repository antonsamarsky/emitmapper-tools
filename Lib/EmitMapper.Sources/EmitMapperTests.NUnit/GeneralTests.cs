using System;
using System.Linq;
using EmitMapper;
using System.Reflection;
using EmitMapper.MappingConfiguration;
using NUnit.Framework;

namespace EmitMapperTests
{
    [TestFixture]
    public class GeneralTests
    {
        public static void TestRefl(object obj)
        {
            FieldInfo field = obj.GetType().GetField("a");
            field.SetValue(obj, 10);
        }

        public class A
        {
            public enum En
            {
                En1,
                En2,
                En3
            }
            public class AInt
            {
                internal int intern = 13;
                public string str = "AInt";

                public AInt()
                {
                    intern = 13;
                }
            }

            private string m_str1 = "A::str1";

            public string str1
            {
                get
                {
                    return m_str1;
                }
                set
                {
                    m_str1 = value;
                }
            }

            public string str2 = "A::str2";
            public AInt obj;
            public En en = En.En3;

            int[] m_arr;

            public int[] arr
            {
                set
                {
                    m_arr = value;
                }
                get
                {
                    return m_arr;
                }
            }

            public AInt[] objArr;

            public string str3 = "A::str3";

            public A()
            {
                Console.WriteLine("A::A()");
            }
        }

        public class B
        {
            public enum En
            {
                En1,
                En2,
                En3
            }
            public class BInt
            {
                public string str = "BInt";
                /*
				public string str
				{
					get
					{
						throw new Exception("reading BInt::str");
					}
					set { }
				}
				 */
            }

            public string str1 = "B::str1";
            public string str2
            {
                get
                {
                    return "B::str2";
                }

            }
            public BInt obj = new BInt();
            public En en = En.En2;

            public BInt[] objArr;

            public int[] arr
            {
                get
                {
                    return new int[] { 1, 5, 9 };
                }
            }

            public object str3 = null;


            public B()
            {
                Console.WriteLine("B::B()");

                objArr = new BInt[2];
                objArr[0] = new BInt();
                objArr[0].str = "b objArr 1";
                objArr[1] = new BInt();
                objArr[1].str = "b objArr 2";
            }
        }

        internal class A1
        {
            public string f1 = "A1::f1";
            public string f2 = "A1::f2";
        }

        internal class B1
        {
            public string f1 = "B1::f1";
            public string f2 = "B1::f2";
        }

        public class Simple1
        {
            public int I = 10;
            public A.En fld1 = A.En.En1;
        }

        public class Simple2
        {
            public int I = 20;
            public B.En fld1 = B.En.En2;
        }

        [Test]
        public void SimpleTest()
        {
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Simple2, Simple1>();
			//DynamicAssemblyManager.SaveAssembly();
			Simple1 s = mapper.Map(new Simple2());
			Assert.AreEqual(20, s.I);
			Assert.AreEqual(A.En.En2, s.fld1);
        }

        [Test]
        public void SimpleTestEnum()
        {
            var mapper = Context.objMan.GetMapper<B.En, A.En>();
            //DynamicAssemblyManager.SaveAssembly();
            A.En aen = mapper.Map(B.En.En3);
            Assert.AreEqual(A.En.En3, aen);
        }

        public struct Struct1
        {
            public int fld;
        }

        public struct Struct2
        {
            public int fld;
        }

        [Test]
        public void SimpleTestStruct()
        {
            var mapper = Context.objMan.GetMapper<Struct2, Struct1>();
            //DynamicAssemblyManager.SaveAssembly();
            Struct1 s = mapper.Map(new Struct2() { fld = 13 });
            Assert.AreEqual(13, s.fld);
        }

        public struct Class1
        {
            public int fld;
        }

        public struct Class2
        {
            public int fld;
        }

        [Test]
        public void SimpleTestClass()
        {
            var mapper = Context.objMan.GetMapper<Class2, Class1>();
            //DynamicAssemblyManager.SaveAssembly();
            Class1 s = mapper.Map(new Class2() { fld = 13 });
            Assert.AreEqual(13, s.fld);
        }


        [Test]
        public void GeneralTests_Test1()
        {
            A a = new A();
            B b = new B();
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(new DefaultMapConfig().DeepMap());
            //DynamicAssemblyManager.SaveAssembly();
            mapper.Map(b, a);
            Assert.AreEqual(a.en, A.En.En2);
            Assert.AreEqual(a.str1, b.str1);
            Assert.AreEqual(a.str2, b.str2);
            Assert.AreEqual(a.obj.str, b.obj.str);
            Assert.AreEqual(a.obj.intern, 13);
            Assert.AreEqual(a.arr.Length, b.arr.Length);
            Assert.AreEqual(a.arr[0], b.arr[0]);
            Assert.AreEqual(a.arr[1], b.arr[1]);
            Assert.AreEqual(a.arr[2], b.arr[2]);

            Assert.AreEqual(a.objArr.Length, b.objArr.Length);
            Assert.AreEqual(a.objArr[0].str, b.objArr[0].str);
            Assert.AreEqual(a.objArr[1].str, b.objArr[1].str);
            Assert.IsNull(a.str3);
        }

        [Test]
        public void GeneralTests_Test2()
        {
            A a = new A();
            B b = new B();

            a.obj = new A.AInt();
            b.obj = null;

            var mapper = Context.objMan.GetMapper<B, A>();
            //DynamicAssemblyManager.SaveAssembly();
            mapper.Map(b, a);
            Assert.IsNull(a.obj);
        }

        [Test]
        public void GeneralTests_Test3()
        {
            A a = new A();
            B b = new B();
            a.obj = new A.AInt();
            a.obj.intern = 15;

            var mapper = Context.objMan.GetMapper<B, A>();
            mapper.Map(b, a);
            Assert.AreEqual(15, a.obj.intern);
        }

        public class Source
        {
            public string field1 = "Source::field1";
            public string field2 = "Source::field2";
            public string field3 = "Source::field3";
        }

        public class Destination
        {
            public string m_field1;
            public string m_field2;
            public string m_field3;
        }

        [Test]
        public void GeneralTests_Example2()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Source, Destination>(
				new DefaultMapConfig().MatchMembers((m1, m2) => "m_" + m1 == m2)
            );

            Source src = new Source();
            Destination dst = mapper.Map(src);
            Assert.AreEqual(src.field1, dst.m_field1);
            Assert.AreEqual(src.field2, dst.m_field2);
            Assert.AreEqual(src.field3, dst.m_field3);
        }

		public class A2
		{
			public string str;
		}

		public class B2
		{
			public string str = "str";
		}

		[Test]
		public void GeneralTests_ConvertUsing()
		{
			var a = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
				new DefaultMapConfig().ConvertUsing<string, string>(s => "converted " + s)
			).Map(new B2());
			Assert.AreEqual("converted str", a.str);
		}

		[Test]
		public void GeneralTests_Ignore()
		{
			var a = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(
				new DefaultMapConfig().IgnoreMembers<B, A>(new[]{"str1"})
			).Map(new B());
			Assert.AreEqual("A::str1", a.str1);
			Assert.AreEqual(a.en, A.En.En2);
		}

        public class A3
        {
            public class Int1
            {
                public string str1;
                public string str2;
                public int i;
            }

            public class Int2
            {
                public Int1 i1;
                public Int1 i2;
                public Int1 i3;
            }

            public Int2 i1;
            public Int2 i2;
            public Int2 i3;
        }

        public class B3
        {
            public class Int1
            {
                public string str1 = "1";
                public string str2 = null;
                public long i = 10;
            }

            public class Int2
            {
                public Int1 i1 = new Int1();
                public Int1 i2 = new Int1();
                public Int1 i3 = null;
            }

            public Int2 i1 = null;
            public Int2 i2 = new Int2();
            public Int2 i3 = new Int2();

        }

        [Test]
        public void GeneralTests_Exception()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B3, A3>();
            //DynamicAssemblyManager.SaveAssembly();
            var a = mapper.Map(new B3());
            Assert.IsNotNull(a);
            Assert.IsNull(a.i1);
            Assert.IsNotNull(a.i2);
            Assert.IsNotNull(a.i3);
            Assert.IsNotNull(a.i2.i1);
            Assert.IsNotNull(a.i2.i2);
            Assert.IsNull(a.i2.i3);
            Assert.IsNotNull(a.i3.i1);
            Assert.IsNotNull(a.i3.i2);
            Assert.IsNull(a.i3.i3);

            Assert.AreEqual("1", a.i2.i1.str1);
            Assert.AreEqual("1", a.i2.i2.str1);
            Assert.AreEqual("1", a.i3.i1.str1);
            Assert.AreEqual("1", a.i3.i2.str1);
            Assert.IsNull(a.i2.i1.str2);
            Assert.IsNull(a.i2.i2.str2);
            Assert.IsNull(a.i3.i1.str2);
            Assert.IsNull(a.i3.i2.str2);
            Assert.AreEqual(10, a.i2.i1.i);
            Assert.AreEqual(10, a.i2.i2.i);
            Assert.AreEqual(10, a.i3.i1.i);
            Assert.AreEqual(10, a.i3.i2.i);
        }

		public class ConstructBy_Source
		{
			public class NestedClass
			{
				public string str = "ConstructBy_Source::str";
			}
			public NestedClass field = new NestedClass();
		}

		public class ConstructBy_Destination
		{
			public class NestedClass
			{
				public string str;
				public int i;
				public NestedClass(int i)
				{
					str = "ConstructBy_Destination::str";
					this.i = i;
				}
			}
			public NestedClass field;

            public ConstructBy_Destination(int i)
            {
            }
		}

		[Test]
		public void ConstructByTest()
		{
			var mapper = ObjectMapperManager
				.DefaultInstance
				.GetMapper<ConstructBy_Source, ConstructBy_Destination>(
					new DefaultMapConfig()
                        .ConstructBy <ConstructBy_Destination.NestedClass>(
						    () => new ConstructBy_Destination.NestedClass(3)
					    )
                        .ConstructBy<ConstructBy_Destination>(() => new ConstructBy_Destination(-1))
				);
			var d = mapper.Map(new ConstructBy_Source());
			Assert.AreEqual("ConstructBy_Source::str", d.field.str);
			Assert.AreEqual(3, d.field.i);
		}

        [Test]
        public void ConstructByTest2()
        {
            var mapper = ObjectMapperManager
                .DefaultInstance
                .GetMapper<string, Guid>(
                    new DefaultMapConfig()
                        .ConvertUsing<string, Guid>( s => new Guid(s) )
                );
            var guid = Guid.NewGuid();
            var d = mapper.Map(guid.ToString());
            Assert.AreEqual(guid, d);
        }

		public class TreeNode
		{
			public string data;
			public TreeNode next;
			public TreeNode[] subNodes; 
		}

		[Test]
		public void TestRecursiveClass()
		{
			var tree = new TreeNode
			{
				data = "node 1",
				next = new TreeNode
				{
					data = "node 2",
					next = new TreeNode
					{
						data = "node 3",
						subNodes = new[]
						{
							new TreeNode
							{
								data = "sub sub data 1"
							},
							new TreeNode
							{
								data = "sub sub data 2"
							}
						}

					}
				},
				subNodes = new[]
				{
					new TreeNode
					{
						data = "sub data 1"
					}
				}
			};
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TreeNode, TreeNode>(
				new DefaultMapConfig().DeepMap()
			);
			var tree2 = mapper.Map(tree);
			Assert.AreEqual("node 1", tree2.data);
			Assert.AreEqual("node 2", tree2.next.data);
			Assert.AreEqual("node 3", tree2.next.next.data);
			Assert.AreEqual("sub data 1", tree2.subNodes[0].data);
			Assert.AreEqual("sub sub data 1", tree2.next.next.subNodes[0].data);
			Assert.AreEqual("sub sub data 2", tree2.next.next.subNodes[1].data);
			Assert.IsNull(tree2.next.next.next);
		}
    }
}