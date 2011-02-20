using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
		[Test]
		public void EntityChildToDataContainerTest()
		{
			var entityChild = new EntityChild
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				AdditionalValue = "Add"
			};
			Mapper.DataMapper.Initialize(new DomainMappingInitializator());
			var table = Mapper.Map<EntityChild, DataContainer>(entityChild);

			Assert.AreEqual(entityChild.Id.ToString(), table.Fields["order_id"]);
			Assert.AreEqual(entityChild.Number, table.Fields["order_number"]);
			Assert.AreEqual(entityChild.Number, table.Fields["order_number_2"]);
			Assert.AreEqual(entityChild.Name, table.Fields["order_name"]);
			Assert.AreEqual(entityChild.Price, table.Fields["order_price"]);
			Assert.AreEqual(entityChild.AdditionalValue, table.Fields["AdditionalValue"]);
		}

		/// <summary>
		/// Entities to entity mapping test.
		/// </summary>
		[Test]
		public void EntityToEntityMappingTest()
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
		public void EntityToEntityVariantsMappingTest()
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
		public void EntityToDataContainerMappingTest()
		{
			var entity = new Entity
			{
				Id = Guid.NewGuid(),
				Name = "Entity Name",
				Number = 134567,
				Price = 100.500m,
				UserName = string.Empty,
				Time = DateTime.Now
			};

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());
			var container = Mapper.Map<Entity, DataContainer>(entity);

			Assert.AreEqual(entity.Id.ToString(), container.Fields["order_id"]);
			Assert.AreEqual(entity.Number, container.Fields["order_number"]);
			Assert.AreEqual(entity.Number, container.Fields["order_number_2"]);
			Assert.AreEqual(entity.Name, container.Fields["order_name"]);
			Assert.AreEqual(entity.Price, container.Fields["order_price"]);
			Assert.AreEqual(entity.UserName, container.Fields["UserName"]);
		}

		/// <summary>
		/// Tables to entity mapping test.
		/// </summary>
		[Test]
		public void DataContainerToEntityMappingTest()
		{
			var container = new DataContainer
			{
				Fields = new Dictionary<string, object>
				{
					{ "order_name", "Name" },
					{ "order_id", Guid.NewGuid().ToString() },
					{ "order_number", 9345 },
					{ "order_number_2", 9345 },
					{ "order_price", decimal.MinValue },
					{ "UserName", "Peter" },
					{ "Time", DateTime.Now.ToString() }
				}
			};

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());
			var entity = Mapper.Map<DataContainer, Entity>(container);

			Assert.AreEqual(container.Fields["order_id"].ToString(), entity.Id.ToString());
			Assert.AreEqual(container.Fields["order_number"], entity.Number);
			Assert.AreEqual(container.Fields["order_number_2"], entity.Number);
			Assert.AreEqual(container.Fields["order_name"], entity.Name);
			Assert.AreEqual(container.Fields["order_price"], entity.Price);
			Assert.AreEqual(container.Fields["UserName"], entity.UserName);
		}

		/// <summary>
		/// Entities to entity mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(10000)]
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
		public void DataContainerToEntityMappingCollectionTest(int capacity)
		{
			var containers = Enumerable.Range(0, capacity).Select(i => new DataContainer
			{
				Fields = new Dictionary<string, object>
				{
					{ "order_id", Guid.NewGuid().ToString() },
					{ "order_name", "Name_" + i },
					{ "order_number", i },
					{ "order_number_2", i },
					{ "order_price", (decimal)Math.Sqrt(i) },
					{ "UserName", "UserName_" + i },
					{ "Time", DateTime.Now.ToString() }
				}
			}).ToArray();

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<DataContainer, Entity>(new List<DataContainer> { new DataContainer { Fields = new Dictionary<string, object>() } });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var entities = Mapper.MapCollection<DataContainer, Entity>(containers).ToArray();

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the collection with {0} elements took: {1} ms. Approx. mapping time per object: {2} sec.", capacity, stopWatch.ElapsedMilliseconds, ((float)stopWatch.ElapsedMilliseconds) / capacity));

			for (int i = 0; i < capacity; i++)
			{
				var container = containers[i];
				var entity = entities[i];

				Assert.AreEqual(container.Fields["order_name"], entity.Name);
				Assert.AreEqual(container.Fields["order_number"], entity.Number);
				Assert.AreEqual(container.Fields["order_price"], entity.Price);
				Assert.AreEqual(container.Fields["UserName"], entity.UserName);
			}
		}

		/// <summary>
		/// Entities to table mapping collection test.
		/// </summary>
		/// <param name="capacity">The capacity.</param>
		[TestCase(1000000)]
		public void EntityToDataContainerMappingCollectionTest(int capacity)
		{
			var entities = Enumerable.Range(0, capacity).Select(i => new Entity
			{
				Id = Guid.NewGuid(),
				Number = i,
				Name = "Name_" + i,
				UserName = "UserName_" + i,
				Price = (decimal)Math.Sqrt(i),
				Time = DateTime.Now
			}).ToArray();

			Mapper.DataMapper.Initialize(new DomainMappingInitializator());

			// Cold mapping
			Mapper.MapCollection<Entity, DataContainer>(new List<Entity> { new Entity() });

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var containers = Mapper.MapCollection<Entity, DataContainer>(entities).ToArray();

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the collection with {0} elements took: {1} ms. Approx. mapping time per object: {2} sec.", capacity, stopWatch.ElapsedMilliseconds, ((float)stopWatch.ElapsedMilliseconds) / capacity));

			for (int i = 0; i < capacity; i++)
			{
				var container = containers[i];
				var entity = entities[i];

				Assert.AreEqual(entity.Name, container.Fields["order_name"]);
				Assert.AreEqual(entity.Number, container.Fields["order_number"]);
				Assert.AreEqual(entity.Price, container.Fields["order_price"]);
				Assert.AreEqual(entity.UserName, container.Fields["UserName"]);
			}
		}

		/// <summary>
		///  Running tests concurrently
		/// </summary>
		/// <param name="numberOfThreads">The number of threads</param>
		[TestCase(1000)]
		public void ConcurrentMappingTest(int numberOfThreads)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			GC.Collect();

			var tasks = new List<Task>();
			for (int i = 0; i < numberOfThreads; i++)
			{
				tasks.Add(Task.Factory.StartNew(DataContainerToEntityMappingTest));
				tasks.Add(Task.Factory.StartNew(EntityToDataContainerMappingTest));
				tasks.Add(Task.Factory.StartNew(() => EntityToDataContainerMappingCollectionTest(numberOfThreads)));
				tasks.Add(Task.Factory.StartNew(() => DataContainerToEntityMappingCollectionTest(numberOfThreads)));
			}
			Task.WaitAll(tasks.ToArray());

			stopWatch.Stop();
			Console.WriteLine(string.Format("Mapping of the concurrent tests took: {0} ms.", stopWatch.ElapsedMilliseconds));
		}
	}
}
