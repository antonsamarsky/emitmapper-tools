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
	public class TableToEntityMappingConfigurator : DefaultMapConfig
	{
		/// <summary>
		/// Gets the mapping operations.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">To type to.</param>
		/// <returns>The mapping operations.</returns>
		public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			return FilterOperations(from, to, ReflectionUtils.GetPublicFieldsAndProperties(to)
											.Where(member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && ((PropertyInfo)member).GetSetMethod() != null)
											.Select(destinationMember => (IMappingOperation)new DestWriteOperation
											{
												Destination = new MemberDescriptor(destinationMember),
												Getter = (ValueGetter<object>)((item, state) =>
												{
													if (item == null)
													{
														return ValueToWrite<object>.Skip();
													}

													var fieldDescription = GetFieldDescription(destinationMember);

													var destinationMemberValue = ConvertFieldToDestinationPropery(item as Table, fieldDescription.Key, fieldDescription.Value, destinationMember as PropertyInfo);

													return destinationMemberValue == null ? ValueToWrite<object>.Skip() : ValueToWrite<object>.ReturnValue(destinationMemberValue);
												})
											})).ToArray();
		}

		private static KeyValuePair<string, Type> GetFieldDescription(MemberInfo memberInfo)
		{
			var attribute = Attribute.GetCustomAttributes(memberInfo, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>().FirstOrDefault();

			if (attribute == null)
			{
				return new KeyValuePair<string, Type>();
			}

			var fieldName = !string.IsNullOrEmpty(attribute.FieldName) ? attribute.FieldName : memberInfo.Name;
			var fieldType = attribute.FieldType;

			return new KeyValuePair<string, Type>(fieldName, fieldType);
		}

		private static dynamic ConvertFieldToDestinationPropery(Table table, string fieldName, Type fieldType, PropertyInfo destinationPropery)
		{
			if (table == null)
			{
				return null;
			}

			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}

			dynamic fieldValue;
			if (!table.Fields.TryGetValue(fieldName, out fieldValue) || fieldValue == null)
			{
				return null;
			}

			var sourceType = fieldType ?? fieldValue.GetType();
			var destinationType = destinationPropery.PropertyType;

			dynamic result = null;
			if (destinationType.IsAssignableFrom(sourceType))
			{
				result = fieldValue;
			}
			else
			{
				var converter = TypeDescriptor.GetConverter(destinationType);
				if (converter != null && converter.CanConvertFrom(sourceType))
				{
					result = converter.ConvertFrom(fieldValue);
				}
				else
				{
					converter = TypeDescriptor.GetConverter(sourceType);
					if (converter != null && converter.CanConvertTo(destinationType))
					{
						result = converter.ConvertTo(fieldValue, destinationType);
					}
				}
			}

			return result;
		}
	}
}