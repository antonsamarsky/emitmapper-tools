using System.Collections.Generic;

namespace Mapping
{
	/// <summary>
	/// The class to map objects.
	/// </summary>
	public static class Mapper
	{
		/// <summary>
		/// The mapper core instance.
		/// </summary>
		private static readonly MapperCore MapperInstance;

		/// <summary>
		/// Initializes static members of the <see cref="Mapper"/> class. 
		/// </summary>
		static Mapper()
		{
			MapperInstance = new MapperCore();
		}

		/// <summary>
		/// Gets the mapper core.
		/// </summary>
		/// <value>
		/// The mapper core.
		/// </value>
		public static MapperCore DataMapper
		{
			get { return MapperInstance; }
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
		public static TTo Map<TFrom, TTo>(TFrom @from)
		{
			return MapperInstance.Map<TFrom, TTo>(@from);
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
			return MapperInstance.Map(@from, @to);
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
			return MapperInstance.MapCollection<TFrom, TTo>(@from);
		}
	}
}