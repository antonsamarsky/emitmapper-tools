using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly List<object> Mappers;

        private static readonly List<Tuple<Type,Type,IMappingConfigurator>> MappingConfigurations;

		/// <summary>
		/// Initializes the <see cref="MapperCore"/> class.
		/// </summary>
		static MapperCore()
		{
            DefaultConfigurator = new DefaultMapConfig();
            Mappers = new List<object>();
            MappingConfigurations = new List<Tuple<Type, Type, IMappingConfigurator>>();
		}

		/// <summary>
		/// Gets the configurators.
		/// </summary>
        public virtual List<Tuple<Type, Type, IMappingConfigurator>> Configurations
		{
            get { return MappingConfigurations; }
		}

		/// <summary>
		/// Initializes the mapper.
		/// </summary>
		/// <param name="mapperInitializator">The mapper initialization.</param>
		public void Initialize(IMapperInitializator mapperInitializator)
		{
			mapperInitializator.ConfigureMapper(this);
		}

		/// <summary>
        /// Adds the configurator instance.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
        /// <param name="configurator">The configurator.</param>
        public virtual void AddConfiguration<TFrom, TTo>(IMappingConfigurator configurator)
		{
            Assert.IsNotNull(configurator, "configurator");

            MappingConfigurations.Add(new Tuple<Type, Type, IMappingConfigurator>(typeof(TFrom), typeof(TTo), configurator));
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
            Assert.ArgumentNotNull(@from, "@from");

            var mapper = GetMapper<TFrom, TTo>();
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

            var mapper = GetMapper<TFrom, TTo>();
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

            var mapper = GetMapper<TFrom, TTo>();
            return mapper.MapEnum(@from);
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
        protected virtual ObjectsMapper<TFrom, TTo> GetMapper<TFrom, TTo>()
		{
            var mapper = Mappers.FirstOrDefault(m =>
            {
                var typeFrom = m.GetType().GetGenericArguments()[0];
                var typeTo = m.GetType().GetGenericArguments()[1];

                return typeFrom.IsAssignableFrom(typeof(TFrom)) && typeTo.IsAssignableFrom(typeof(TTo));
            });

            if (mapper == null)
            {
                var configuration = MappingConfigurations.Find(mp => mp.Item1.IsAssignableFrom(typeof(TFrom)) && mp.Item2.IsAssignableFrom(typeof(TTo)));
                var config = configuration == null ? DefaultConfigurator : configuration.Item3;

                mapper = ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>(config);
                Mappers.Add(mapper);
            }

            return (ObjectsMapper<TFrom, TTo>)mapper;
		}
	}
}