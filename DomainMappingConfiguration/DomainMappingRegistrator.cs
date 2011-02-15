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
			mapperCore.AddMappingConfiguration<Table, object>(new TableToEntityMappingConfigurator());
			mapperCore.AddMappingConfiguration<object, Table>(new EntityToTableMappingConfigurator());
		}
	}
}