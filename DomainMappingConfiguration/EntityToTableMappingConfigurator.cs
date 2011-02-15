using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using MappingDefinitions;
using MemberDescriptor = EmitMapper.MappingConfiguration.MemberDescriptor;

namespace DomainMappingConfiguration
{
	/// <summary>
	/// The item configuration.
	/// </summary>
	public class EntityToTableMappingConfigurator : DefaultMapConfig
	{
		public EntityToTableMappingConfigurator()
		{
			ConstructBy(() => new Table { Fields = new Dictionary<string, string>() });
		}

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

									var attributes = Attribute.GetCustomAttributes(sourceMember, typeof(DataMemberAttribute), true);

									if (!attributes.Any())
									{
										return;
									}

									// NOTE: Map the property value to the filed.
									var converter = TypeDescriptor.GetConverter(((PropertyInfo)sourceMember).PropertyType);
									if (converter == null || !converter.CanConvertTo(typeof(string)))
									{
										return;
									}

									foreach (var attribute in attributes.Cast<DataMemberAttribute>())
									{
										var fieldName = string.IsNullOrEmpty(attribute.FieldName) ? sourceMember.Name : attribute.FieldName;
										if (table.Fields.ContainsKey(fieldName))
										{
											continue;
										}

										var stringValue = converter.ConvertToString(value);
										table.Fields.Add(fieldName, stringValue ?? string.Empty);
									}

								}
							})).ToArray();
		}
	}
}