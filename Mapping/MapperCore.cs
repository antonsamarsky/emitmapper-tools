using System;
using System.Collections.Generic;
using Diagnostics;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace Mapping
{
	/// <summary>
	/// The mapper core.
	/// </summary>
	public class MapperCore
	{
		private static readonly IMappingConfigurator DefaultConfigurator;

		/// <summary>
		/// The list of configurations.
		/// </summary>
		private static readonly List<Tuple<Type, Type, IMappingConfigurator>> MappingConfigurators;

		/// <summary>
		/// Initializes the <see cref="MapperCore"/> class.
		/// </summary>
		static MapperCore()
		{
			DefaultConfigurator = new DefaultMapConfig();
			MappingConfigurators = new List<Tuple<Type, Type, IMappingConfigurator>>();
		}

		/// <summary>
		/// Gets the configurators.
		/// </summary>
		public virtual List<Tuple<Type, Type, IMappingConfigurator>> Configurators
		{
			get { return MappingConfigurators; }
		}

		/// <summary>
		/// Initializes the mapper.
		/// </summary>
		/// <param name="mappingTypeRegistrator">The mapping type registrator.</param>
		public void Initialize(IMappingTypeRegistrator mappingTypeRegistrator)
		{
			mappingTypeRegistrator.ConfigureMapper(this);
		}

		/// <summary>
		/// Adds the mapping configurator.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="configurator">The configurator.</param>
		public virtual void AddMappingConfiguration<TFrom, TTo>(IMappingConfigurator configurator)
		{
			AddMappingConfigurator(typeof(TFrom), typeof(TTo), configurator);
		}

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The object from.</param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		public virtual TTo Map<TFrom, TTo>(TFrom @from)
		{
			return this.Map<TFrom, TTo>(@from, this.GetConfigurator<TFrom, TTo>());
		}

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The object from.</param>
		/// <param name="to">The destination object.</param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		public virtual TTo Map<TFrom, TTo>(TFrom @from, TTo @to)
		{
			return this.Map(@from, @to, this.GetConfigurator<TFrom, TTo>());
		}

		/// <summary>
		/// Maps the collection.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The from objects collection.</param>
		/// <returns>The output mapped collection.</returns>
		public virtual IEnumerable<TTo> MapCollection<TFrom, TTo>(IEnumerable<TFrom> @from)
		{
			return this.MapCollection<TFrom, TTo>(@from, this.GetConfigurator<TFrom, TTo>());
		}

		/// <summary>
		/// Maps the collection.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The from objects collection.</param>
		/// <param name="mappingConfigurator">The mapping configurator.</param>
		/// <returns>
		/// The output mapped collection.
		/// </returns>
		protected virtual IEnumerable<TTo> MapCollection<TFrom, TTo>(IEnumerable<TFrom> @from, IMappingConfigurator mappingConfigurator)
		{
			Assert.ArgumentNotNullOrEmpty(@from, "@from");
			Assert.ArgumentNotNull(mappingConfigurator, "@mappingConfigurator");

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(mappingConfigurator);
			return mapper.MapEnum(@from);
		}

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="typeFrom">The type from.</param>
		/// <param name="mappingConfigurator">The mapping configurator.</param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		protected virtual TTo Map<TFrom, TTo>(TFrom typeFrom, IMappingConfigurator mappingConfigurator)
		{
			Assert.ArgumentNotNull(typeFrom, "typeFrom");
			Assert.ArgumentNotNull(mappingConfigurator, "@mappingConfigurator");

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(mappingConfigurator);
			return mapper.Map(typeFrom);
		}

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="typeFrom">The object from.</param>
		/// <param name="typeTo">The destination object.</param>
		/// <param name="mappingConfigurator"></param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		protected virtual TTo Map<TFrom, TTo>(TFrom typeFrom, TTo typeTo, IMappingConfigurator mappingConfigurator)
		{
			Assert.ArgumentNotNull(typeFrom, "typeFrom");
			Assert.ArgumentNotNull(typeTo, "typeTo");
			Assert.ArgumentNotNull(mappingConfigurator, "@mappingConfigurator");

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(mappingConfigurator);
			return mapper.Map(typeFrom, typeTo);
		}

		/// <summary>
		/// Adds the mapping configurator.
		/// </summary>
		/// <param name="typeFrom">The type from.</param>
		/// <param name="typeTo">The type to.</param>
		/// <param name="configurator">The configurator.</param>
		protected virtual void AddMappingConfigurator(Type typeFrom, Type typeTo, IMappingConfigurator configurator)
		{
			Assert.ArgumentNotNull(configurator, "configurator");
			Assert.ArgumentNotNull(typeFrom, "typeFrom");
			Assert.ArgumentNotNull(typeTo, "typeTo");

			MappingConfigurators.Add(new Tuple<Type, Type, IMappingConfigurator>(typeFrom, typeTo, configurator));
		}

		/// NOTE: Resolving from IoC can be added here.
		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <returns>
		/// The configurator instance.
		/// </returns>
		protected virtual IMappingConfigurator GetConfigurator<TFrom, TTo>()
		{
			var configuration = MappingConfigurators.Find(mp => mp.Item1.IsAssignableFrom(typeof(TFrom)) && mp.Item2.IsAssignableFrom(typeof(TTo)));

			return configuration == null ? DefaultConfigurator : configuration.Item3;
		}
	}
}