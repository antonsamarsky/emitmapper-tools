namespace Mapping
{
	/// <summary>
	/// The mapping registrator. Use it to register mapping configs.
	/// </summary>
	public interface IMappingTypeRegistrator
	{
		/// <summary>
		/// Configures the mapper.
		/// </summary>
		/// <param name="mapperCore">The mapper core.</param>
		void ConfigureMapper(MapperCore mapperCore);
	}
}