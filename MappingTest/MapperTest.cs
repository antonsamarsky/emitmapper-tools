using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain;
using DomainMappingConfiguration;
using EmitMapper.MappingConfiguration;
using Mapping;
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
		/// Entities to enity mapping test.
		/// </summary>
		[Test]
		public void EntityToEnityMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = null,
			};

			// Cold mapping
			Mapper.Map<Entity, Entity2>(new Entity());

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			Entity2 entity2 = Mapper.Map<Entity, Entity2>(entity);

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping took: {0} ms", stopWatch.ElapsedMilliseconds));

			Assert.AreEqual(entity.Id, entity2.Id);
			Assert.AreEqual(entity.Name, entity2.Name);
			Assert.AreEqual(entity.UserName, entity2.UserName);
		}

		/// <summary>
		/// Entities to enity variants mapping test.
		/// http://emitmapper.codeplex.com/wikipage?title=Customization%20using%20default%20configurator&referringTitle=Documentation&ANCHOR#customization_overview
		/// </summary>
		[Test]
		public void EntityToEnityVariantsMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = null,
			};

			var mapConfig = new DefaultMapConfig().PostProcess<object>((value, state) =>
			{
				Console.WriteLine("Post processing: " + value.ToString());
				return value;
			});

			Mapper.MapperCore.AddMappingConfiguration<Entity, Entity2>(mapConfig);

			// Cold mapping
			Mapper.Map<Entity, Entity2>(new Entity());

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			Entity2 entity2 = Mapper.Map<Entity, Entity2>(entity);

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping took: {0} ms", stopWatch.ElapsedMilliseconds));

			Assert.AreEqual(entity.Id, entity2.Id);
			Assert.AreEqual(entity.Name, entity2.Name);
			Assert.AreEqual(entity.Number, entity2.Number);
			Assert.AreEqual(entity.UserName, entity2.UserName);
		}

		/// <summary>
		/// Entities to table mapping test.
		/// </summary>
		[Test]
		public void EntityToTableMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = "Anton",
			};

			Mapper.MapperCore.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.Map<Entity, Table>(new Entity());

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var table = Mapper.Map<Entity, Table>(entity);

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping took: {0} ms", stopWatch.ElapsedMilliseconds));

			Assert.AreEqual(entity.Id.ToString(), table.Fields["order_id"]);
			Assert.AreEqual(entity.Number.ToString(), table.Fields["order_number"]);
			Assert.AreEqual(entity.Number.ToString(), table.Fields["order_number_2"]);
			Assert.AreEqual(entity.Name, table.Fields["order_name"]);
			Assert.AreEqual(entity.Price.ToString(), table.Fields["order_price"]);
			Assert.AreEqual(entity.UserName, table.Fields["UserName"]);
		}

		/// <summary>
		/// Tables to entity mapping test.
		/// </summary>
		[Test]
		public void TableToEntityMappingTest()
		{
			var table = new Table
			{
				Fields = new Dictionary<string, string>
				{
					{ "order_name", "Name" },
					{ "order_id", Guid.NewGuid().ToString() },
					{ "order_number", "9345" },
					{ "order_number_2", "9345" },
					{ "order_price", 100.500m.ToString() },
					{ "UserName", "Peter" }
				}
			};

			Mapper.MapperCore.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.Map<Table, Entity>(new Table { Fields = new Dictionary<string, string>() });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var entity = Mapper.Map<Table, Entity>(table);

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping took: {0} ms", stopWatch.ElapsedMilliseconds));

			Assert.AreEqual(table.Fields["order_id"], entity.Id.ToString());
			Assert.AreEqual(table.Fields["order_number"], entity.Number.ToString());
			Assert.AreEqual(table.Fields["order_number_2"], entity.Number.ToString());
			Assert.AreEqual(table.Fields["order_name"], entity.Name);
			Assert.AreEqual(table.Fields["order_price"], entity.Price.ToString());
			Assert.AreEqual(table.Fields["UserName"], entity.UserName);
		}

		/// <summary>
		/// Tables to entity mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(10000)]
		public void TableToEntityMappingCollectionTest(int capacity)
		{
			var tables = Enumerable.Range(0, capacity).Select(i => new Table
			{
				Fields = new Dictionary<string, string>
				{
					{ "order_id", Guid.NewGuid().ToString() },
					{ "order_name", "Name_" + i },
					{ "order_number", i.ToString() },
					{ "order_price", Math.Sqrt(i).ToString() },
					{ "UserName", "UserName_" + i }
				}
			});

			Mapper.MapperCore.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<Table, Entity>(new List<Table> { new Table { Fields = new Dictionary<string, string>() } });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var entities = Mapper.MapCollection<Table, Entity>(tables);

			var entitiesArray = entities.ToArray();
			var tablesArray = tables.ToArray();

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the collection took: {0} ms", stopWatch.ElapsedMilliseconds));

			for (int i = 0; i < capacity; i++)
			{
				var table = tablesArray[i];
				var entity = entitiesArray[i];

				Assert.AreEqual(table.Fields["order_name"], entity.Name);
				Assert.AreEqual(table.Fields["order_number"], entity.Number.ToString());
				Assert.AreEqual(table.Fields["order_price"], entity.Price.ToString());
				Assert.AreEqual(table.Fields["UserName"], entity.UserName);
			}
		}

		[TestCase(10000)]
		public void EntityTableToMappingCollectionTest(int capacity)
		{
			var entities = Enumerable.Range(0, capacity).Select(i => new Entity
			{
				Id = Guid.NewGuid(),
				Number = i,
				Name = "Name_" + i,
				UserName = "UserName_" + i,
				Price = (decimal)Math.Sqrt(i),
			});

			Mapper.MapperCore.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<Entity, Table>(new List<Entity> { new Entity() });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var tables = Mapper.MapCollection<Entity, Table>(entities);

			var entitiesArray = entities.ToArray();
			var tablesArray = tables.ToArray();

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the collection took: {0} ms", stopWatch.ElapsedMilliseconds));

			for (int i = 0; i < capacity; i++)
			{
				var table = tablesArray[i];
				var entity = entitiesArray[i];

				Assert.AreEqual(entity.Name, table.Fields["order_name"]);
				Assert.AreEqual(entity.Number.ToString(), table.Fields["order_number"]);
				Assert.AreEqual(entity.Price.ToString(), table.Fields["order_price"]);
				Assert.AreEqual(entity.UserName, table.Fields["UserName"]);
			}
		}
	}
}
