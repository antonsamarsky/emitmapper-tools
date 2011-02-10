using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using MappingDefinition;
using MemberDescriptor = EmitMapper.MappingConfiguration.MemberDescriptor;

namespace DomainMappingConfiguration
{
	/// <summary>
	/// The item configuration.
	/// </summary>
	public class EntityToTableMappingConfigurator : DefaultMapConfig
	{
		/// <summary>
		/// Gets the mapping operations.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">To type to.</param>
		/// <returns>The mapping operations.</returns>
		public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			return FilterOperations(from, to, ReflectionUtils.GetPublicFieldsAndProperties(from)
							.Where(member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && ((PropertyInfo)member).GetGetMethod() != null)
							.Select(sourceMember => (IMappingOperation)new SrcReadOperation
							{
								Source = new MemberDescriptor(sourceMember),
								Setter = (destination, value, state) =>
								{
									var table = destination as Table;

									if (table == null || value == null)
									{
										return;
									}

									var dataMemberAttributes = Attribute.GetCustomAttributes(sourceMember, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>();

									List<string> fieldNames;
									if (!dataMemberAttributes.Any())
									{
										fieldNames = new List<string> { sourceMember.Name };
									}
									else
									{
										fieldNames = (from attribute in dataMemberAttributes
																	let name = attribute.FieldName
																	select string.IsNullOrEmpty(name) ? sourceMember.Name : attribute.FieldName).ToList();
									}


									// NOTE: Map the property value to the filed.
									var converter = TypeDescriptor.GetConverter(((PropertyInfo)sourceMember).PropertyType);
									if (converter == null)
									{
										return;
									}

									if (!converter.CanConvertTo(typeof(string)))
									{
										return;
									}

									var stringValue = converter.ConvertToString(value);
									if (stringValue != null)
									{
										fieldNames.ForEach(fieldName =>
												{
													if (table.Fields.ContainsKey(fieldName))
													{
														table.Fields.Remove(fieldName);
													}
													table.Fields.Add(fieldName, stringValue);
												});
									}
								}
							})).ToArray();
		}
	}
}