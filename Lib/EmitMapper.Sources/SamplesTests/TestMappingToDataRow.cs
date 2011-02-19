using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System.Data;

namespace EmitMapper.Samples.SamplesTests
{
	[TestClass]
	public class TestMappingToDataRow
	{
		public class Map2DataRowConfig : MapConfigBase<Map2DataRowConfig>
		{
			public override IMappingOperation[] GetMappingOperations(Type from, Type to)
			{
				var objectMembers = ReflectionUtils.GetPublicFieldsAndProperties(from);
				return base.FilterOperations(
					from,
					to,
					objectMembers.Select(
						m => (IMappingOperation)new SrcReadOperation
						{
							Source = new MemberDescriptor(m),
							Setter = (obj, value, state) =>
								{
									((DataRow)obj)[m.Name] = value ?? DBNull.Value;
								}
						}
					)
				).ToArray();
			}
		}
		// Using: 

		// Test data object
		public class TestDTO
		{
			public string field1 = "field1";
			public int field2 = 10;
			public bool field3 = true;
		}

		[TestMethod]
		public void MappingToDataRow_test()
		{
			// this is the mapper 
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TestDTO, DataRow>(new Map2DataRowConfig());

			// initialization of test DTO object
			TestDTO testDataObject = new TestDTO
			{
				field1 = "field1",
				field2 = 10,
				field3 = true
			};

			// Initializing of test table. Usual this table is read from database.
			DataTable dt = new DataTable();
			dt.Columns.Add("field1", typeof(string));
			dt.Columns.Add("field2", typeof(int));
			dt.Columns.Add("field3", typeof(bool));
			dt.Rows.Add();
			DataRow dr = dt.Rows[0];

			// Mapping test object to datarow
			mapper.Map(testDataObject, dr);

			// Check if object is correctly mapped
			Assert.AreEqual("field1", dr["field1"]);
			Assert.AreEqual(10, dr["field2"]);
			Assert.AreEqual(true, dr["field3"]);
		}
	}
}
