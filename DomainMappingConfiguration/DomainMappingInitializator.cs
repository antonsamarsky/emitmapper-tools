using Domain;
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
			mapperCore.AddConfiguration<DataContainer, object>(new DataContainerToEntityMappingConfigurator());
			mapperCore.AddConfiguration<object, DataContainer>(new EntityToDataContainerMappingConfigurator());
		}
	}
}