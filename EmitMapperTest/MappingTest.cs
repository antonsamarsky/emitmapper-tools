using System;
using System.Collections.Generic;
using System.Diagnostics;
using Domain;
using DomainMappingConfiguration;
using EmitMapper;
using NUnit.Framework;

namespace EmitMapperTest
{
	[TestFixture]
	public class MappingTest
	{
		private Stopwatch stopWatch = new Stopwatch();

		[SetUp]
		public void SetUp()
		{
			stopWatch.Start();
		}

		[TearDown]
		public void TearDown()
		{
			stopWatch.Stop();
			Console.WriteLine(stopWatch.ElapsedMilliseconds);
		}

		[Test]
		public void EntityToTableMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "name",
				Number = 12323,
				Price = 100.500m
			};

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Entity, Table>(new EntityToTableMappingConfigurator());
			var result = mapper.Map(entity, new Table { Fields = new Dictionary<string, string>() });

			Assert.IsNotNull(result);
		}

		[Test]
		public void HandmadeEntityToTableMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "name",
				Number = 12323,
				Price = 100.500m
			};

			var table = new Table
			{
				Fields = new Dictionary<string, string>
			  {
			    { "order_name", entity.Name },
			    { "order_id", entity.Id.ToString() },
			    { "order_number", entity.Number.ToString() },
			    { "order_price", entity.Price.ToString() }
			  }
			};
		}

		[Test]
		public void TableToEntityMappingTest()
		{
			var table = new Table
			{
				Fields = new Dictionary<string, string>
			  {
			    { "order_name", "Order Name" },
			    { "order_id", "6F9619FF-8B86-D011-B42D-00CF4FC964FF" },
			    { "order_number", "102" },
			    { "order_price", "100500" }
			  }
			};

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Table, Entity>(new TableToEntityMappingConfigurator());
			var result = mapper.Map(table);

			Assert.IsNotNull(result);
		}
	}
}
