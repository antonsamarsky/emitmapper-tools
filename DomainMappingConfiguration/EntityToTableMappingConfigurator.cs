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
			ConstructBy(() => new Table { Fields = new Dictionary<string, dynamic>() });
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
													if (destination == null || value == null)
													{
														return;
													}

													var sourceProperty = (PropertyInfo)sourceMember;

													var fieldsDescription = GetFieldsDescription(sourceProperty);

													ConvertSourceProperyToFields(sourceProperty.PropertyType, value, destination as Table, fieldsDescription);
												}
											})).ToArray();
		}

		private static IEnumerable<KeyValuePair<string, Type>> GetFieldsDescription(PropertyInfo propertyInfo)
		{
			return from attribute in Attribute.GetCustomAttributes(propertyInfo, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>()
						 let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? propertyInfo.Name : attribute.FieldName
						 let fieldType = attribute.FieldType ?? propertyInfo.PropertyType
						 select new KeyValuePair<string, Type>(fieldName, fieldType);
		}

		private static void ConvertSourceProperyToFields(Type propertyType, object propertyValue, Table table, IEnumerable<KeyValuePair<string, Type>> fieldsDescription)
		{
			if (table == null)
			{
				return;
			}

			if (!fieldsDescription.Any())
			{
				return;
			}

			fieldsDescription.ToList().ForEach(fd =>
			{
				if (table.Fields.ContainsKey(fd.Key))
				{
					return;
				}

				dynamic value = null;
				if (fd.Value.IsAssignableFrom(propertyType))
				{
					value = propertyValue;
				}
				else
				{
					var converter = TypeDescriptor.GetConverter(fd.Value);
					if (converter != null && converter.CanConvertFrom(propertyType))
					{
						value = converter.ConvertFrom(propertyValue);
					}
					else
					{
						converter = TypeDescriptor.GetConverter(propertyType);
						if (converter != null && converter.CanConvertTo(fd.Value))
						{
							value = converter.ConvertTo(propertyValue, fd.Value);
						}
					}
				}

				if (value != null)
				{
					table.Fields.Add(fd.Key, value);
				}
			});
		}
	}
}
