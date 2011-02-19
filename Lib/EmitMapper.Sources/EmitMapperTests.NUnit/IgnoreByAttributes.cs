using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper;

namespace EmitMapperTests
{
	[TestFixture]
	public class IgnoreByAttributes
	{
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MyIgnoreAttribute : Attribute
{
}

public class IgnoreByAttributesSrc
{
	[MyIgnoreAttribute]
	public string str1 = "IgnoreByAttributesSrc::str1";
	public string str2 = "IgnoreByAttributesSrc::str2";
}

public class IgnoreByAttributesDst
{
	public string str1 = "IgnoreByAttributesDst::str1";
	public string str2 = "IgnoreByAttributesDst::str2";
}

public class MyConfigurator : DefaultMapConfig
{
	public override IMappingOperation[] GetMappingOperations(Type from, Type to)
	{
		base.IgnoreMembers<object, object>(GetIgnoreFields(from).Concat(GetIgnoreFields(to)).ToArray());
		return base.GetMappingOperations(from, to);
	}
	private IEnumerable<string> GetIgnoreFields(Type type)
	{
		return type
			.GetFields()
			.Where(
				f => f.GetCustomAttributes(typeof(MyIgnoreAttribute), false).Length > 0
			)
			.Select(f => f.Name)
			.Concat(
				type
				.GetProperties()
				.Where(
					p => p.GetCustomAttributes(typeof(MyIgnoreAttribute), false).Length > 0
				)
				.Select(p => p.Name)
			);
	}
}

[Test]
public void Test()
{
	var mapper = ObjectMapperManager.DefaultInstance.GetMapper<IgnoreByAttributesSrc, IgnoreByAttributesDst>(new MyConfigurator());
	var dst = mapper.Map(new IgnoreByAttributesSrc());
	Assert.AreEqual("IgnoreByAttributesDst::str1", dst.str1);
	Assert.AreEqual("IgnoreByAttributesSrc::str2", dst.str2);
}

	}
}
