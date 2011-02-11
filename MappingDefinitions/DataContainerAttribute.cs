using System;

namespace MappingDefinitions
{
	/// <summary>
	/// The data container attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DataContainerAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the template id.
		/// </summary>
		/// <value>The template id.</value>
		public virtual string TableName { get; set; }

		/// <summary>
		/// Gets or sets the link to the entity data storage.
		/// </summary>
		/// <value>The link to the entity data storage.</value>
		public virtual string Link { get; set; }
	}
}