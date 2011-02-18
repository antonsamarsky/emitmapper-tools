using Domain;
using EmitMapper;
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
			mapperCore.AddConfiguration<Table, object>(new TableToEntityMappingConfigurator());
            mapperCore.AddConfiguration<object, Table>(new EntityToTableMappingConfigurator());
		}
	}
}