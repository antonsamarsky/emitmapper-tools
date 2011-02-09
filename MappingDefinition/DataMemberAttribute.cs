using System;

namespace MappingDefinition
{
	/// <summary>
  /// The data member attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
  public class DataMemberAttribute : Attribute 
  {
    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public virtual string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the type of the field.
    /// </summary>
    /// <value>The type of the field.</value>
		public virtual string FieldType { get; set; }
  }
}