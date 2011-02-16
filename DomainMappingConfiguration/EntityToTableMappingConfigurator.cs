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

													var fieldDescriptions = GetFieldDescriptions(sourceProperty);

													ConvertSourceProperyToFields(sourceProperty.PropertyType, value, destination as Table, fieldDescriptions);
												}
											})).ToArray();
		}

		private static IEnumerable<Tuple<string, Type>> GetFieldDescriptions(PropertyInfo propertyInfo)
		{
			return from attribute in Attribute.GetCustomAttributes(propertyInfo, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>()
						 let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? propertyInfo.Name : attribute.FieldName
						 let fieldType = attribute.FieldType ?? propertyInfo.PropertyType
						 select new Tuple<string, Type>(fieldName, fieldType);
		}

		private static void ConvertSourceProperyToFields(Type propertyType, object propertyValue, Table table, IEnumerable<Tuple<string, Type>> fieldDescriptions)
		{
			if (table == null)
			{
				return;
			}

			if (!fieldDescriptions.Any())
			{
				return;
			}

			var converterLazy = new Lazy<TypeConverter>(() => TypeDescriptor.GetConverter(propertyType));

			fieldDescriptions.ToList().ForEach(fd =>
			{
				if (table.Fields.ContainsKey(fd.Item1))
				{
					return;
				}

				dynamic mappedValue = null;
				if (fd.Item2.IsAssignableFrom(propertyType))
				{
					mappedValue = propertyValue;
				}
				else
				{
					var converter = converterLazy.Value;
					if (converter != null && converter.CanConvertTo(fd.Item2))
					{
						mappedValue = converter.ConvertTo(propertyValue, fd.Item2);
					}
				}

				if (mappedValue != null)
				{
					table.Fields.Add(fd.Item1, mappedValue);
				}
			});
		}
	}
}
