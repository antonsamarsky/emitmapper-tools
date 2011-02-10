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
		private readonly Stopwatch stopWatch;
		private Entity entity;
		private Table table;

		public MappingTest()
		{
			this.stopWatch = new Stopwatch();
			this.entity = new Entity();
			this.table = new Table { Fields = new Dictionary<string, string>() };
		}

		[SetUp]
		public void SetUp()
		{
			this.entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = "Anton",
			};

			this.table = new Table
			{
				Fields = new Dictionary<string, string>
				{
					{ "order_name", "Name" },
					{ "order_id", "6f9619ff-8b86-d011-b42d-00cf4fc964ff" },
					{ "order_number", "9345" },
					{ "order_number_2", "9345" },
					{ "order_price", 100.500m.ToString() },
					{ "UserName", "Peter" }
				}
			};

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
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Entity, Table>(new EntityToTableMappingConfigurator());
			this.table = mapper.Map(this.entity, this.table);

			this.AssertValues();
		}

		[Test]
		public void ManualEntityToTableMappingTest()
		{
			this.table = new Table
			{
				Fields = new Dictionary<string, string>
				{
					{ "order_name", entity.Name },
					{ "order_id", entity.Id.ToString() },
					{ "order_number", entity.Number.ToString() },
					{ "order_number_2", entity.Number.ToString() },
					{ "order_price", entity.Price.ToString() },
					{ "UserName", entity.UserName }
				}
			};

			this.AssertValues();
		}

		[Test]
		public void TableToEntityMappingTest()
		{
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Table, Entity>(new TableToEntityMappingConfigurator());
			this.entity = mapper.Map(table);

			this.AssertValues();
		}

		[Test]
		public void ManualTableToEntityMappingTest()
		{
			this.entity = new Entity
			{
				Id = Guid.Parse(this.table.Fields["order_id"]),
				Name = this.table.Fields["order_name"],
				Number = int.Parse(this.table.Fields["order_number"]),
				Price = decimal.Parse(this.table.Fields["order_price"]),
				UserName = this.table.Fields["UserName"]
			};

			this.AssertValues();
		}

		private void AssertValues()
		{
			Assert.AreEqual(this.entity.Id.ToString(), this.table.Fields["order_id"]);
			Assert.AreEqual(this.entity.Number.ToString(), this.table.Fields["order_number"]);
			Assert.AreEqual(this.entity.Number.ToString(), this.table.Fields["order_number_2"]);
			Assert.AreEqual(this.entity.Name, this.table.Fields["order_name"]);
			Assert.AreEqual(this.entity.Price.ToString(), this.table.Fields["order_price"]);
			Assert.AreEqual(this.entity.UserName, this.table.Fields["UserName"]);
		}
	}
}
