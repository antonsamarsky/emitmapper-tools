using Domain;
using Mapping;

namespace DomainMappingConfiguration
{
	/// <summary>
	/// The test domain mapping registrator.
	/// </summary>
	public class DomainMappingRegistrator : IMappingTypeRegistrator
	{
		/// <summary>
		/// Configures the mapper.
		/// </summary>
		/// <param name="mapperCore">The mapper core.</param>
		public void ConfigureMapper(MapperCore mapperCore)
		{
			mapperCore.AddMappingConfigurator<Table, object>(new TableToEntityMappingConfigurator());
			mapperCore.AddMappingConfigurator<object, Table>(new EntityToTableMappingConfigurator());
		}
	}
}