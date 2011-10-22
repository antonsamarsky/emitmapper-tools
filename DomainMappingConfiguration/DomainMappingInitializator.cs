using Domain;
using DomainMappingConfiguration.Configurators;
using Mapping;

namespace DomainMappingConfiguration
{
	/// <summary>
	/// The test domain mapping registrator.
	/// </summary>
	public class DomainMappingInitializator : IMapperInitializator
	{
		/// <summary>
		/// Configures the mapper.
		/// </summary>
		/// <param name="mapperCore">The mapper core.</param>
		public void ConfigureMapper(MapperCore mapperCore)
		{
			// mapperCore.AddConfiguration<DataContainer, object>(new DataContainerToEntityPropertyMappingConfigurator());
			// mapperCore.AddConfiguration<object, DataContainer>(new EntityToDataContainerPropertyMappingConfigurator());
			mapperCore.AddConfiguration<DataContainer, object>(new DataContainerToObjectConfigurator());
			mapperCore.AddConfiguration<object, DataContainer>(new ObjectToDataContainerConfigurator());
		}
	}
}