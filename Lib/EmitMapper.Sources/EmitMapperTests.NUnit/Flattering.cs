using EmitMapper;
using NUnit.Framework;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EmitMapperTests
{
    [TestFixture]
    public class Flattering
    {
        public class A
        {
            public string message;
            public string message2;
        }

        public class B
        {
            public class IntB
            {
                public string message = "hello";
                public string GetMessage2()
                {
                    return "medved";
                }
            }

            public IntB intB = new IntB();
        }

        [Test]
        public void TestFlattering1()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(
				new CustomMapConfig
				{
					GetMappingOperationFunc = (from, to) =>
					{
						return new[]
						{
							new	ReadWriteSimple
							{
								Source = new MemberDescriptor(new[] { typeof(B).GetMember("intB")[0], typeof(B.IntB).GetMember("message")[0] }), 
								Destination = new MemberDescriptor(new[] { typeof(A).GetMember("message")[0] })
							},
							new	ReadWriteSimple
							{
								Source = new MemberDescriptor(new[] { typeof(B).GetMember("intB")[0], typeof(B.IntB).GetMember("GetMessage2")[0] }), 
								Destination = new MemberDescriptor(new[] { typeof(A).GetMember("message2")[0] })
							}
						};
					}
				}
            );

            B b = new B();
            A a = mapper.Map(b);
            Assert.AreEqual(b.intB.message, a.message);
            Assert.AreEqual(b.intB.GetMessage2(), a.message2);
        }
    }
}