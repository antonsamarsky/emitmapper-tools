using System.Linq;
using EmitMapper;
using NUnit.Framework;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using System;
using System.Collections.Generic;
using System.Collections;

namespace EmitMapperTests
{
    [TestFixture]
    public class CustomMapping
    {
        public class A1
        {
            private string _fld2 = "";

            public string fld1 = "";
            public string fld2
            {
                get
                {
                    return _fld2;
                }
            }
            public void SetFld2(string value)
            {
                _fld2 = value;
            }
        }


        public class A2
        {
            public string fld1;
            public string fld2;
        }

        public class B2
        {
            public string fld2 = "B2::fld2";
            public string fld3 = "B2::fld3";
        }

        [Test]
        public void Test_CustomConverter()
        {
            A2 a = Context.objMan.GetMapper<B2, A2>(
				new DefaultMapConfig()
                    .ConvertUsing<object, string>( v => "333" )
                    .ConvertUsing<object, string>( v => "hello" )
					.SetConfigName("ignore")
            ).Map(new B2());
            Assert.IsNull(a.fld1);
            Assert.AreEqual("hello", a.fld2);

            a = Context.objMan.GetMapper<B2, A2>().Map(new B2());
            Assert.AreEqual("B2::fld2", a.fld2);
        }

		public class AA
		{
			public string fld1;
			public string fld2;
		}

		public class BB
		{
			public string fld1 = "B2::fld1";
			public string fld2 = "B2::fld2";
		}

		[Test]
		public void Test_CustomConverter2()
		{
			AA a = Context.objMan.GetMapper<BB, AA>(
				new DefaultMapConfig().ConvertUsing<object, string>(v => "converted " + v.ToString())
			).Map(new BB());
			Assert.AreEqual("converted B2::fld1", a.fld1);
			Assert.AreEqual("converted B2::fld2", a.fld2);
		}

        public class A3
        {
            public class Int
            {
                public string str1;
                public string str2;
            }

            public struct SInt
            {
                public string str1;
                public string str2;
            }

            public Int fld;
            public SInt? fld2;
            public string status;
        }

        public class B3
        {
            public class Int
            {
                public string str1 = "B3::Int::str1";
            }
            public struct SInt
            {
                public string str1;
            }

            public Int fld = new Int();
            public SInt fld2 = new SInt();

            public B3()
            {
                fld2.str1 = "B3::SInt::str1";
            }
        }
        [Test]
        public void Test_PostProcessing()
        {
            A3 a = Context.objMan.GetMapper<B3, A3>(
                new DefaultMapConfig()
                    .PostProcess<A3.Int>((i, state) => { i.str2 = "processed"; return i; })
                    .PostProcess<A3.SInt?>((i, state) => { return new A3.SInt() { str1 = i.Value.str1, str2 = "processed" }; })
                    .PostProcess<A3>((i, state) => { i.status = "processed"; return i; })
            ).Map(new B3());
            Assert.AreEqual("B3::Int::str1", a.fld.str1);
            Assert.AreEqual("processed", a.fld.str2);

            Assert.AreEqual("B3::SInt::str1", a.fld2.Value.str1);
            Assert.AreEqual("processed", a.fld2.Value.str2);

            Assert.AreEqual("processed", a.status);
        }
    }
}