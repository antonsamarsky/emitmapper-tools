using System;
using System.Collections.Concurrent;
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
		/// The member field description
		/// </summary>
		private readonly ConcurrentDictionary<MemberInfo, KeyValuePair<string, Type>> memberFieldDescription;

		/// <summary>
		/// The converters collection.
		/// </summary>
		private readonly ConcurrentDictionary<Type, TypeConverter> typeCoverters;

		/// <summary>
		/// Initializes a new instance of the <see cref="TableToEntityMappingConfigurator"/> class.
		/// </summary>
		public TableToEntityMappingConfigurator()
		{
			this.memberFieldDescription = new ConcurrentDictionary<MemberInfo, KeyValuePair<string, Type>>();
			this.typeCoverters = new ConcurrentDictionary<Type, TypeConverter>();
		}

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

								var fieldDescription = this.GetFieldDescription(destinationMember);
								var destinationMemberValue = this.ConvertFieldToDestinationProperty(item as Table, fieldDescription.Key, fieldDescription.Value, destinationMember as PropertyInfo);

								return destinationMemberValue == null ? ValueToWrite<object>.Skip() : ValueToWrite<object>.ReturnValue(destinationMemberValue);
							})
						})).ToArray();
		}

		/// <summary>
		/// Gets the field description.
		/// </summary>
		/// <param name="memberInfo">The member info.</param>
		/// <returns>The field description.</returns>
		private KeyValuePair<string, Type> GetFieldDescription(MemberInfo memberInfo)
		{
			return this.memberFieldDescription.GetOrAdd(memberInfo, mi =>
			{
				var attribute = Attribute.GetCustomAttributes(mi, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>().FirstOrDefault();

				if (attribute == null)
				{
					return new KeyValuePair<string, Type>();
				}

				var fieldName = !string.IsNullOrEmpty(attribute.FieldName) ? attribute.FieldName : mi.Name;
				var fieldType = attribute.FieldType;

				return new KeyValuePair<string, Type>(fieldName, fieldType);
			});
		}

		/// <summary>
		/// Converts the field to destination property.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="fieldType">Type of the field.</param>
		/// <param name="destinationProperty">The destination property.</param>
		/// <returns>The conversion result.</returns>
		private object ConvertFieldToDestinationProperty(Table table, string fieldName, Type fieldType, PropertyInfo destinationProperty)
		{
			if (table == null)
			{
				return null;
			}

			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}

			object fieldValue;
			if (!table.Fields.TryGetValue(fieldName, out fieldValue) || fieldValue == null)
			{
				return null;
			}

			var sourceType = fieldType ?? fieldValue.GetType();
			var destinationType = destinationProperty.PropertyType;

			return destinationType.IsAssignableFrom(sourceType) ? fieldValue : this.ConvertValue(fieldValue, sourceType, destinationType);
		}

		/// <summary>
		/// Converts the value.
		/// </summary>
		/// <param name="sourceValue">The source value.</param>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <returns>The converted value.</returns>
		private object ConvertValue(object sourceValue, Type sourceType, Type destinationType)
		{
			object result = null;

			TypeConverter typeConverter = this.typeCoverters.GetOrAdd(destinationType, TypeDescriptor.GetConverter);
			if (typeConverter != null && typeConverter.CanConvertFrom(sourceType))
			{
				result = typeConverter.ConvertFrom(sourceValue);
			}
			else
			{
				typeConverter = this.typeCoverters.GetOrAdd(sourceType, TypeDescriptor.GetConverter);
				if (typeConverter != null && typeConverter.CanConvertTo(destinationType))
				{
					result = typeConverter.ConvertTo(sourceValue, destinationType);
				}
			}

			return result;
		}
	}
}