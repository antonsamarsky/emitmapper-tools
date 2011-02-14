using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain;
using EmitMapper.MappingConfiguration;
using Mapping;
using Mapping.DomainConfigurators;
using NUnit.Framework;

namespace MappingTest
{
	/// <summary>
	/// The mapper tests.
	/// </summary>
	[TestFixture]

	public class MapperTest
	{
		/// <summary>
		/// The stopwatch to test time of execution.
		/// </summary>
		private readonly Stopwatch stopWatch;

		/// <summary>
		/// The test entity to map.
		/// </summary>
		private Entity entity;

		/// <summary>
		/// The test table to test.
		/// </summary>
		private Table table;

		/// <summary>
		/// Initializes a new instance of the <see cref="MapperTest"/> class.
		/// </summary>
		public MapperTest()
		{
			this.stopWatch = new Stopwatch();
			this.entity = new Entity();
			this.table = new Table { Fields = new Dictionary<string, string>() };
		}

		/// <summary>
		/// The sets up.
		/// </summary>
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

			this.stopWatch.Start();
			Mapper.MapperCore.Initialize(new DomainMappingRegistrator());
		}

		/// <summary>
		/// The Tears down.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			this.stopWatch.Stop();
			Console.WriteLine(this.stopWatch.ElapsedMilliseconds);
		}

		/// <summary>
		/// Entities to enity mapping test.
		/// </summary>
		[Test, Repeat(100)]
		public void EntityToEnityMappingTest()
		{
			Entity2 entity2 = Mapper.Map<Entity, Entity2>(this.entity);

			Assert.AreEqual(this.entity.Id, entity2.Id);
			Assert.AreEqual(this.entity.Name, entity2.Name);
			Assert.AreEqual(this.entity.Number, entity2.Number);
		}

		/// <summary>
		/// Entities to enity variants mapping test.
		/// http://emitmapper.codeplex.com/wikipage?title=Customization%20using%20default%20configurator&referringTitle=Documentation&ANCHOR#customization_overview
		/// </summary>
		[Test]
		public void EntityToEnityVariantsMappingTest()
		{
			var mapConfig = new DefaultMapConfig().PostProcess<object>((value, state) =>
																																	{
																																		Console.WriteLine("Post processing: " + value.ToString());
																																		return value;
																																	});
			Entity2 entity2 = Mapper.Map<Entity, Entity2>(this.entity, mapConfig);

			Assert.AreEqual(this.entity.Id, entity2.Id);
			Assert.AreEqual(this.entity.Name, entity2.Name);
			Assert.AreEqual(this.entity.Number, entity2.Number);
		}

		/// <summary>
		/// Manuals the entity to enity mapping test.
		/// </summary>
		[Test, Repeat(100)]
		public void ManualEntityToEnityMappingTest()
		{
			Entity2 entity2 = new Entity2
			{
				Id = this.entity.Id,
				Name = this.entity.Name,
				Number = this.entity.Number
			};

			Assert.AreEqual(this.entity.Id, entity2.Id);
			Assert.AreEqual(this.entity.Name, entity2.Name);
			Assert.AreEqual(this.entity.Number, entity2.Number);
		}

		/// <summary>
		/// Entities to table mapping test.
		/// </summary>
		[Test, Repeat(100)]
		public void EntityToTableMappingTest()
		{
			this.table = Mapper.Map(this.entity, this.table);

			this.AssertValues();
		}

		/// <summary>
		/// Manuals the entity to table mapping test.
		/// </summary>
		[Test, Repeat(100)]
		public void ManualEntityToTableMappingTest()
		{
			this.table = new Table
			{
				Fields = new Dictionary<string, string>
				{
					{ "order_name", this.entity.Name },
					{ "order_id", this.entity.Id.ToString() },
					{ "order_number", this.entity.Number.ToString() },
					{ "order_number_2", this.entity.Number.ToString() },
					{ "order_price", this.entity.Price.ToString() },
					{ "UserName", this.entity.UserName }
				}
			};

			this.AssertValues();
		}

		/// <summary>
		/// Tables to entity mapping test.
		/// </summary>
		[Test, Repeat(100)]
		public void TableToEntityMappingTest()
		{
			this.entity = Mapper.Map<Table, Entity>(this.table);

			this.AssertValues();
		}

		/// <summary>
		/// Manuals the table to entity mapping test.
		/// </summary>
		[Test, Repeat(100)]
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

		/// <summary>
		/// Tables to entity mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(100)]
		public void TableToEntityMappingCollectionTest(int capacity)
		{
			this.stopWatch.Start();

			var tables = Enumerable.Range(0, capacity).Select(i => new Table
			{
				Fields = new Dictionary<string, string>
				{
					{ "order_name", "Name_" + i },
					{ "order_number", i.ToString() },
					{ "order_price", Math.Sqrt(i).ToString() },
					{ "UserName", "UserName_" + i }
				}
			});

			var entities = Mapper.MapCollection<Table, Entity>(tables);

			this.stopWatch.Stop();
			Console.WriteLine("Map collection time: " + this.stopWatch.ElapsedMilliseconds);

			var arrayExpected = tables.ToArray();
			var arrayToAssert = entities.ToArray();
			for (var i = 0; i <= entities.Count() - 1; i++)
			{
				Assert.AreEqual(arrayExpected[i].Fields["order_name"], arrayToAssert[i].Name);
				Assert.AreEqual(arrayExpected[i].Fields["order_number"], arrayToAssert[i].Number.ToString());
				Assert.AreEqual(arrayExpected[i].Fields["order_price"], arrayToAssert[i].Price.ToString());
				Assert.AreEqual(arrayExpected[i].Fields["UserName"], arrayToAssert[i].UserName);
			}
		}

		/*
		/// <summary>
		/// Entities to entity mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(100)]
		public void EntityToEntityMappingCollectionTest(int capacity)
		{
			this.stopWatch.Start();

			var entities = Enumerable.Range(0, capacity).Select(i => new Entity
			{
				Name = "Name_" + i,
				Number = i,
				Price = (decimal)Math.Sqrt(i),
				UserName = "UserName_" + i,
			});

			var tables = Mapper.MapCollection<Entity, Table>(entities);

			this.stopWatch.Stop();
			Console.WriteLine("Map collection time: " + this.stopWatch.ElapsedMilliseconds);

			var arrayExpected = tables.ToArray();
			var arrayToAssert = entities.ToArray();
			for (var i = 0; i <= entities.Count() - 1; i++)
			{
				Assert.AreEqual(arrayExpected[i].Fields["order_name"], arrayToAssert[i].Name);
				Assert.AreEqual(arrayExpected[i].Fields["order_number"], arrayToAssert[i].Number.ToString());
				Assert.AreEqual(arrayExpected[i].Fields["order_price"], arrayToAssert[i].Price.ToString());
				Assert.AreEqual(arrayExpected[i].Fields["UserName"], arrayToAssert[i].UserName);
			}
		}
		*/

		/// <summary>
		/// Asserts the values.
		/// </summary>
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
