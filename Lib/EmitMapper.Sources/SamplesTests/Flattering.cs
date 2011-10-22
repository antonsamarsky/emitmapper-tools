using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EMConfigurations;

namespace EmitMapper.Samples.SamplesTests
{
	[TestClass]
	public class Flattering
	{
		public class ModelObject
		{
			public DateTime BaseDate { get; set; }
			public ModelSubObject Sub { get; set; }
			public ModelSubObject Sub2 { get; set; }
			public ModelSubObject SubWithExtraName { get; set; }
		}

		public class ModelSubObject
		{
			public string ProperName { get; set; }
			public ModelSubSubObject SubSub { get; set; }
		}

		public class ModelSubSubObject
		{
			public string IAmACoolProperty { get; set; }
		}

		public class ModelDto
		{
			public DateTime BaseDate { get; set; }
			public string SubProperName { get; set; }
			public string Sub2ProperName { get; set; }
			public string SubWithExtraNameProperName { get; set; }
			public string SubSubSubIAmACoolProperty { get; set; }
		}

		[TestMethod]
		public void TestFlattering()
		{
			var source = new ModelObject
				{
					BaseDate = DateTime.Now,
					Sub = new ModelSubObject
					{
						ProperName = "Some name",
						SubSub = new ModelSubSubObject
						{
							IAmACoolProperty = "Cool daddy-o"
						}
					},
					Sub2 = new ModelSubObject
					{
						ProperName = "Sub 2 name"
					},
					SubWithExtraName = new ModelSubObject
					{
						ProperName = "Some other name"
					},
				};

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<ModelObject, ModelDto>(
				new FlatteringConfig()
			);

			var b = mapper.Map(source);
		}

	}
}