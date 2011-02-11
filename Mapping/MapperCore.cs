using System.Collections.Generic;
using Diagnostics;
using Domain;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using Mapping.Configurators;

namespace Mapping
{
	/// <summary>
	/// The mapper core.
	/// </summary>
	public class MapperCore
	{
		/// <summary>
		/// entityToTableMappingConfigurator instance.
		/// </summary>
		private static readonly IMappingConfigurator EntityToTableMappingConfigurator = new EntityToTableMappingConfigurator();

		/// <summary>
		/// tableToEntityMappingConfigurator instance.
		/// </summary>
		private static readonly IMappingConfigurator TableToEntityMappingConfigurator = new TableToEntityMappingConfigurator();

		/// <summary>
		/// defaultMapConfig instance.
		/// </summary>
		private static readonly IMappingConfigurator DefaultMapConfig = new DefaultMapConfig();

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
			Assert.ArgumentNotNull(@from, "@from");

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(this.GetConfigurator<TFrom, TTo>());
			return mapper.Map(@from);
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
			Assert.ArgumentNotNull(@from, "@from");
			Assert.ArgumentNotNull(@to, "@to");

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(this.GetConfigurator<TFrom, TTo>());
			return mapper.Map(@from, @to);
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
			Assert.ArgumentNotNullOrEmpty(@from, "@from");

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(this.GetConfigurator<TFrom, TTo>());
			return mapper.MapEnum(@from);
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <returns>
		/// The Configurator.
		/// </returns>
		/// NOTE: Use IoC instead.
		protected virtual IMappingConfigurator GetConfigurator<TFrom, TTo>()
		{
			if (typeof(Table) == typeof(TTo))
			{
				return EntityToTableMappingConfigurator;
			}

			if (typeof(TFrom) == typeof(Table))
			{
				return TableToEntityMappingConfigurator;
			}

			return DefaultMapConfig;
		}
	}
}