using System.Collections.Generic;
using EmitMapper;

namespace Mapping
{
	/// <summary>
	/// The class to map objects.
	/// </summary>
	public class Mapper
	{
		private static readonly MapperCore mapperCore;

		/// <summary>
		/// Initializes static members of the <see cref="Mapper"/> class. 
		/// </summary>
		static Mapper()
		{
			mapperCore = new MapperCore();
		}

		/// <summary>
		/// Gets or sets the mapper core.
		/// </summary>
		/// <value>
		/// The mapper core.
		/// </value>
		public static MapperCore MapperCore { get { return mapperCore; } }

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The object from.</param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		public static TTo Map<TFrom, TTo>(TFrom @from)
		{
			return mapperCore.Map<TFrom, TTo>(@from);
		}

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The object from.</param>
		/// <param name="mappingConfigurator">The mapping configurator.</param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		public static TTo Map<TFrom, TTo>(TFrom @from, IMappingConfigurator mappingConfigurator)
		{
			return mapperCore.Map<TFrom, TTo>(@from, mappingConfigurator);
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
		public static TTo Map<TFrom, TTo>(TFrom @from, TTo @to)
		{
			return mapperCore.Map(@from, @to);
		}

		/// <summary>
		/// Maps the specified from.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The object from.</param>
		/// <param name="to">The destination object.</param>
		/// <param name="mappingConfigurator">The mapping configurator.</param>
		/// <returns>
		/// The mapped object.
		/// </returns>
		public static TTo Map<TFrom, TTo>(TFrom @from, TTo @to, IMappingConfigurator mappingConfigurator)
		{
			return mapperCore.Map(@from, @to, mappingConfigurator);
		}

		/// <summary>
		/// Maps the collection.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="from">The from objects collection.</param>
		/// <returns>The output mapped collection.</returns>
		public static IEnumerable<TTo> MapCollection<TFrom, TTo>(IEnumerable<TFrom> @from)
		{
			return mapperCore.MapCollection<TFrom, TTo>(@from);
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
		public static IEnumerable<TTo> MapCollection<TFrom, TTo>(IEnumerable<TFrom> @from, IMappingConfigurator mappingConfigurator)
		{
			return mapperCore.MapCollection<TFrom, TTo>(@from);
		}
	}
}