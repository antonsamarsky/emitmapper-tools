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
		/// Entities to entity mapping test.
		/// </summary>
		[Test]
		public void _EntityToEntityMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = null,
			};

			Entity2 entity2 = Mapper.Map<Entity, Entity2>(entity);

			Assert.AreEqual(entity.Id, entity2.Id);
			Assert.AreEqual(entity.Name, entity2.Name);
			Assert.AreEqual(entity.UserName, entity2.UserName);
		}

		/// <summary>
		/// Entities to entity variants mapping test.
		/// </summary>
		[Test]
		public void _EntityToEntityVariantsMappingTest()
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

			Mapper.DataMapper.AddConfiguration<Entity, Entity2>(mapConfig);

			Entity2 entity2 = Mapper.Map<Entity, Entity2>(entity);

			Assert.AreEqual(entity.Id, entity2.Id);
			Assert.AreEqual(entity.Name, entity2.Name);
			Assert.AreEqual(entity.Number, entity2.Number);
			Assert.AreEqual(entity.UserName, entity2.UserName);
		}

		/// <summary>
		/// Entities to table mapping test.
		/// </summary>
		[Test]
		public void _EntityToTableMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = string.Empty,
			};

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());
			var table = Mapper.Map<Entity, Table>(entity);

			Assert.AreEqual(entity.Id.ToString(), table.Fields["order_id"]);
			Assert.AreEqual(entity.Number, table.Fields["order_number"]);
			Assert.AreEqual(entity.Number, table.Fields["order_number_2"]);
			Assert.AreEqual(entity.Name, table.Fields["order_name"]);
			Assert.AreEqual(entity.Price, table.Fields["order_price"]);
			Assert.AreEqual(entity.UserName, table.Fields["UserName"]);
		}

		/// <summary>
		/// Tables to entity mapping test.
		/// </summary>
		[Test]
		public void _TableToEntityMappingTest()
		{
			var table = new Table
			{
				Fields = new Dictionary<string, object>
				{
					{ "order_name", "Name" },
					{ "order_id", Guid.NewGuid().ToString() },
					{ "order_number", 9345 },
					{ "order_number_2", 9345 },
					{ "order_price", 100.500m },
					{ "UserName", "Peter" }
				}
			};

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());
			var entity = Mapper.Map<Table, Entity>(table);

			Assert.AreEqual(table.Fields["order_id"].ToString(), entity.Id.ToString());
			Assert.AreEqual(table.Fields["order_number"], entity.Number);
			Assert.AreEqual(table.Fields["order_number_2"], entity.Number);
			Assert.AreEqual(table.Fields["order_name"], entity.Name);
			Assert.AreEqual(table.Fields["order_price"], entity.Price);
			Assert.AreEqual(table.Fields["UserName"], entity.UserName);
		}

		/// <summary>
		/// Entities to entity mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(1000000)]
		public void EntityToEntityMappingCollectionTest(int capacity)
		{
			var entities = Enumerable.Range(0, capacity).Select(i => new Entity
			{
				Id = Guid.NewGuid(),
				Number = i,
				Name = "Name_" + i,
				UserName = "UserName_" + i,
				Price = (decimal)Math.Sqrt(i),
			}).ToArray();

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<Entity, Entity2>(new List<Entity> { new Entity() });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var entities2 = Mapper.MapCollection<Entity, Entity>(entities).ToArray();

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the collection with {0} elements took: {1} ms. Approx. mapping time per object: {2} sec.", capacity, stopWatch.ElapsedMilliseconds, ((float)stopWatch.ElapsedMilliseconds) / capacity));

			for (int i = 0; i < capacity; i++)
			{
				var entity2 = entities2[i];
				var entity = entities[i];

				Assert.AreEqual(entity.Id, entity2.Id);
				Assert.AreEqual(entity.Name, entity2.Name);
				Assert.AreEqual(entity.UserName, entity2.UserName);
			}
		}

		/// <summary>
		/// Tables to entity mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(1000000)]
		public void TableToEntityMappingCollectionTest(int capacity)
		{
			var tables = Enumerable.Range(0, capacity).Select(i => new Table
			{
				Fields = new Dictionary<string, object>
				{
					{ "order_id", Guid.NewGuid().ToString() },
					{ "order_name", "Name_" + i },
					{ "order_number", i },
					{ "order_number_2", i },
					{ "order_price", (decimal)Math.Sqrt(i) },
					{ "UserName", "UserName_" + i }
				}
			}).ToArray();

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<Table, Entity>(new List<Table> { new Table { Fields = new Dictionary<string, object>() } });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var entities = Mapper.MapCollection<Table, Entity>(tables).ToArray();

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the collection with {0} elements took: {1} ms. Approx. mapping time per object: {2} sec.", capacity, stopWatch.ElapsedMilliseconds, ((float)stopWatch.ElapsedMilliseconds) / capacity));

			for (int i = 0; i < capacity; i++)
			{
				var table = tables[i];
				var entity = entities[i];

				Assert.AreEqual(table.Fields["order_name"], entity.Name);
				Assert.AreEqual(table.Fields["order_number"], entity.Number);
				Assert.AreEqual(table.Fields["order_price"], entity.Price);
				Assert.AreEqual(table.Fields["UserName"], entity.UserName);
			}
		}

		/// <summary>
		/// Entities to table mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(10000)]
		public void EntityToTableMappingCollectionTest(int capacity)
		{
			var entities = Enumerable.Range(0, capacity).Select(i => new Entity
			{
				Id = Guid.NewGuid(),
				Number = i,
				Name = "Name_" + i,
				UserName = "UserName_" + i,
				Price = (decimal)Math.Sqrt(i),
			}).ToArray();

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<Entity, Table>(new List<Entity> { new Entity() });

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            GC.Collect();

			var tables = Mapper.MapCollection<Entity, Table>(entities).ToArray();

            stopWatch.Stop();
            Console.WriteLine(string.Format("Mapping of the collection with {0} elements took: {1} ms. Approx. mapping time per object: {2} sec.", capacity, stopWatch.ElapsedMilliseconds, ((float)stopWatch.ElapsedMilliseconds) / capacity));

            for (int i = 0; i < capacity; i++)
            {
                var table = tables[i];
                var entity = entities[i];

                Assert.AreEqual(entity.Name, table.Fields["order_name"]);
                Assert.AreEqual(entity.Number, table.Fields["order_number"]);
                Assert.AreEqual(entity.Price, table.Fields["order_price"]);
                Assert.AreEqual(entity.UserName, table.Fields["UserName"]);
            }
		}
	}
}
