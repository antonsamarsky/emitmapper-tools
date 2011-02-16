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
		/// Initializes a new instance of the <see cref="EntityToTableMappingConfigurator"/> class.
		/// </summary>
		public EntityToTableMappingConfigurator()
		{
			ConstructBy(() => new Table { Fields = new Dictionary<string, object>() });
			memberFieldsDescription = new Dictionary<MemberInfo, IEnumerable<KeyValuePair<string, Type>>>();
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
		private static void ConvertSourcePropertyToFields(Type propertyType, object propertyValue, Table table, IEnumerable<KeyValuePair<string, Type>> fieldsDescription)
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

				object value = null;
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
