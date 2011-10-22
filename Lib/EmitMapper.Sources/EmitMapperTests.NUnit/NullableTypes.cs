using System;
using System.Linq;
using EmitMapper;
using NUnit.Framework;
using EmitMapper.MappingConfiguration;

namespace EmitMapperTests
{
    [TestFixture]
    public class NullableTypes
    {
        public class A1
        {
			public class Int1
			{
				public string s;
				public Int1()
				{
				}
				public Int1(int i)
				{
					s = "A1::Int1::s";
				}
			}

            public int fld1;
			public int fld2;
			public Int1 i;
            public int? fld3;
        }

        public class B1
        {
			public class Int1
			{
				public string s = "B1::Int1::s";
			}

            public int? fld1 = 10;
			public int? fld2;
			public Int1 i;
            public int? fld3;
        }
        [Test]
        public void Nullable_to_Value()
        {
            var mapper = ObjectMapperManager
				.DefaultInstance
				.GetMapper<B1, A1>(
					new DefaultMapConfig()
					 .NullSubstitution<B1.Int1, A1.Int1>( (state)=>new A1.Int1(0) )
                     .NullSubstitution<int?, int>((state) => 3)
                     .NullSubstitution<int?, int?>((state) => 4)
				);
			//DynamicAssemblyManager.SaveAssembly();
			var a = mapper.Map(new B1());
            Assert.AreEqual(10, a.fld1);
			Assert.IsNotNull(a.i);
			Assert.AreEqual("A1::Int1::s", a.i.s);
			Assert.AreEqual(3, a.fld2);
            Assert.AreEqual(4, a.fld3);
        }

        public class A2
        {
            public int? fld1;
        }

        public class B2
        {
            public int fld1 = 10;
        }
        [Test]
        public void Value_to_Nullable()
        {
            A2 a = Context.objMan.GetMapper<B2, A2>().Map(new B2());
            Assert.AreEqual(10, a.fld1);
        }

        public class A3
        {
            public struct AInt
            {
                public string fld1;
				public string fld2;
				public string fld3;
            }

            public AInt? fld1;
			public AInt? fld2 { get; set; }
			public AInt? fld3 { get; set; }
			public AInt? fld4;
			public AInt? fld5;
			public AInt? fld6;
			public AInt fld7;

			public A3()
			{
				var a = new AInt();
				a.fld3 = "a";
				fld2 = a;
				fld5 = a;
			}
        }
        public class B3
        {
            public struct BInt
            {
				public string fld1;
				public string fld2;
            }

            public BInt? fld1;
			public BInt? fld2 { get; set; }
			public BInt? fld3;
			public BInt? fld4 { get; set; }
			public BInt? fld5;
			public BInt fld6;
			public BInt? fld7;
        }
        [Test]
        public void NullableStruct_to_Struct()
        {
            NullableTypes.B3.BInt bint = new B3.BInt();
            bint.fld1 = "b";
            B3 b = new B3();
            b.fld1 = bint;
			b.fld2 = bint;
			b.fld3 = bint;
			b.fld4 = bint;
			b.fld6 = bint;
			b.fld7 = bint;
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B3, A3>();
			//DynamicAssemblyManager.SaveAssembly();

			A3 a = mapper.Map(b);
            Assert.AreEqual("b", a.fld1.Value.fld1);
			Assert.AreEqual("b", a.fld2.Value.fld1);
			Assert.AreEqual("b", a.fld3.Value.fld1);
			Assert.AreEqual("b", a.fld4.Value.fld1);
			Assert.AreEqual("b", a.fld6.Value.fld1);
			Assert.AreEqual("b", a.fld7.fld1);
			Assert.AreEqual("a", a.fld2.Value.fld3);
			Assert.IsFalse(a.fld5.HasValue);
        }

        public class A4
        {
            public struct AInt
            {
                public string fld1;
            }

            public AInt fld1;
        }
        public class B4
        {
            public struct BInt
            {
                public string fld1;
            }

            public BInt? fld1 = new BInt();
        }
        [Test]
        public void Struct_to_NullableStruct()
        {
            NullableTypes.B4.BInt bint = new B4.BInt();
            bint.fld1 = "b";
            B4 b = new B4();
            b.fld1 = bint;
            A4 a = Context.objMan.GetMapper<B4, A4>().Map(b);
            Assert.AreEqual("b", a.fld1.fld1);
        }

        public class A5
        {
            public enum En
            {
                value1,
                value2,
                value3,
            }

            public int? fld1 = 0;
            public int? fld2 = 10;
            public En? fld3 = En.value1;
            public En fld4 = En.value1;
            public int? fld5 = 0;
            public int? fld6 = null;
        }

        public class B5
        {
            public enum En
            {
                value1,
                value2,
                value3,
            }

            public int fld1 = 10;
            public string fld2 = null;
            public En fld3 = En.value2;
            public En? fld4 = En.value3;
            public int? fld5 = 13;
            public string fld6 = "11";
        }
        [Test]
        public void Test_Nullable()
        {
            A5 a = Context.objMan.GetMapper<B5, A5>().Map(new B5());
            Assert.AreEqual(10, a.fld1.Value);
            Assert.IsNull(a.fld2);
            Assert.AreEqual(A5.En.value2, a.fld3.Value);
            Assert.AreEqual(A5.En.value3, a.fld4);
            Assert.AreEqual(13, a.fld5.Value);
            Assert.AreEqual(11, a.fld6.Value);
        }

        public class A6
        {
            public int? i { get; set; }
            public DateTime? dt { get; set; }
        }

        public class B6
        {
        }

        [Test]
        public void Test_Object_Nullable()
        {
            var a = ObjectMapperManager
                .DefaultInstance
                .GetMapper<B6, A6>(
					new DefaultMapConfig().DeepMap().ConvertUsing<object, object>(v => null)
				)
				.Map(new B6());
            Assert.IsNull(a);
        }

        public class A7
        {
            public int? i { get; set; }
        }

        public class B7
        {
            public decimal i = 100;
        }

        [Test]
        public void Test_Object_Nullable7()
        {
            var a = ObjectMapperManager
                .DefaultInstance
                .GetMapper<B7, A7>(
					new DefaultMapConfig().DeepMap().ConvertUsing<object, int>(v => 100)
				)
				.Map(new B7());

            Assert.AreEqual(100, a.i);
        }

    }
}