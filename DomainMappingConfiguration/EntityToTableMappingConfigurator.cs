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
		/// <summary>
		/// The collection of attributes values.
		/// </summary>
		private readonly IDictionary<MemberInfo, IEnumerable<KeyValuePair<string, Type>>> memberFieldsDescription;

		/// <summary>
		/// The converters collection.
		/// </summary>
		private readonly IDictionary<Type, TypeConverter> typeCoverters;

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityToTableMappingConfigurator"/> class.
		/// </summary>
		public EntityToTableMappingConfigurator()
		{
			ConstructBy(() => new Table { Fields = new Dictionary<string, object>() });

			this.memberFieldsDescription = new Dictionary<MemberInfo, IEnumerable<KeyValuePair<string, Type>>>();
			this.typeCoverters = new Dictionary<Type, TypeConverter>();
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

													var fieldsDescription = this.GetFieldsDescription(sourceMember);
													ConvertSourcePropertyToFields(((PropertyInfo)sourceMember).PropertyType, value, destination as Table, fieldsDescription);
												}
											})).ToArray();
		}

		/// <summary>
		/// Gets the fields description.
		/// </summary>
		/// <param name="memberInfo">The member info.</param>
		/// <returns>The fields description.</returns>
		private IEnumerable<KeyValuePair<string, Type>> GetFieldsDescription(MemberInfo memberInfo)
		{
			IEnumerable<KeyValuePair<string, Type>> result;

			if (memberFieldsDescription.TryGetValue(memberInfo, out result))
			{
				return result;
			}

			result = from attribute in Attribute.GetCustomAttributes(memberInfo, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>()
							 let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? memberInfo.Name : attribute.FieldName
							 let fieldType = attribute.FieldType ?? ((PropertyInfo)memberInfo).PropertyType
							 select new KeyValuePair<string, Type>(fieldName, fieldType);

			memberFieldsDescription.Add(memberInfo, result);

			return result;
		}

		/// <summary>
		/// Converts the source property to fields.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="propertyValue">The property value.</param>
		/// <param name="table">The table.</param>
		/// <param name="fieldsDescription">The fields description.</param>
		private void ConvertSourcePropertyToFields(Type propertyType, object propertyValue, Table table, IEnumerable<KeyValuePair<string, Type>> fieldsDescription)
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

				object value = fd.Value.IsAssignableFrom(propertyType) ? propertyValue : this.ConvertValue(propertyValue, propertyType, fd.Value);

				if (value != null)
				{
					table.Fields.Add(fd.Key, value);
				}
			});
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

			TypeConverter typeConverter;
			if (this.typeCoverters.TryGetValue(destinationType, out typeConverter))
			{
				if (typeConverter.CanConvertFrom(sourceType))
				{
					result = typeConverter.ConvertFrom(sourceValue);
				}
				else
				{
					if (this.typeCoverters.TryGetValue(sourceType, out typeConverter))
					{
						if (typeConverter.CanConvertTo(destinationType))
						{
							result = typeConverter.ConvertTo(sourceValue, destinationType);
						}
					}
				}
			}
			else
			{
				if (this.typeCoverters.TryGetValue(sourceType, out typeConverter))
				{
					if (typeConverter.CanConvertTo(destinationType))
					{
						result = typeConverter.ConvertTo(sourceValue, destinationType);
					}
					else
					{
						typeConverter = TypeDescriptor.GetConverter(destinationType);
						if (typeConverter != null && typeConverter.CanConvertFrom(sourceType))
						{
							result = typeConverter.ConvertFrom(sourceValue);
							this.typeCoverters.Add(destinationType, typeConverter);
						}
					}
				}
				else
				{
					typeConverter = TypeDescriptor.GetConverter(sourceType);
					if (typeConverter != null && typeConverter.CanConvertTo(destinationType))
					{
						result = typeConverter.ConvertTo(sourceValue, destinationType);
						this.typeCoverters.Add(sourceType, typeConverter);
					}
					else
					{
						typeConverter = TypeDescriptor.GetConverter(destinationType);
						if (typeConverter != null && typeConverter.CanConvertFrom(sourceType))
						{
							result = typeConverter.ConvertFrom(sourceValue);
							this.typeCoverters.Add(destinationType, typeConverter);
						}
					}
				}
			}

			return result;
		}
	}
}
