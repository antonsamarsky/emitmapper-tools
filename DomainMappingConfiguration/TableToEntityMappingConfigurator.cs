using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
		/// Initializes a new instance of the <see cref="TableToEntityMappingConfigurator"/> class.
		/// </summary>
		public TableToEntityMappingConfigurator()
		{
			this.memberFieldDescription = new ConcurrentDictionary<MemberInfo, KeyValuePair<string, Type>>();
		}

		/// <summary>
		/// Gets the mapping operations.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">To type to.</param>
		/// <returns>The mapping operations.</returns>
		public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			return this.FilterOperations(from, to, ReflectionUtils.GetPublicFieldsAndProperties(to)
									.Where(member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && ((PropertyInfo)member).GetSetMethod() != null)
									.Select(destinationMember => (IMappingOperation)new DestWriteOperation
									{
										Destination = new MemberDescriptor(destinationMember),
										Getter = (ValueGetter<object>)((item, state) =>
										{
											if (item == null || !(item is Table) || !(destinationMember is PropertyInfo))
											{
												return ValueToWrite<object>.Skip();
											}

											var fieldDescription = this.GetFieldDescription(destinationMember);
											var destinationMemberValue = ConvertFieldToDestinationProperty((Table)item, (PropertyInfo)destinationMember, fieldDescription);

											return destinationMemberValue == null ? ValueToWrite<object>.Skip() : ValueToWrite<object>.ReturnValue(destinationMemberValue);
										})
									})).ToArray();
		}

		/// <summary>
		/// Converts the field to destination property.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="destinationProperty">The destination property.</param>
		/// <param name="fieldDescription">The field description.</param>
		/// <returns>
		/// The conversion result.
		/// </returns>
		protected static object ConvertFieldToDestinationProperty(Table table, PropertyInfo destinationProperty, KeyValuePair<string, Type> fieldDescription)
		{
			if (table == null || table.Fields == null || string.IsNullOrEmpty(fieldDescription.Key))
			{
				return null;
			}

			object fieldValue;
			if (!table.Fields.TryGetValue(fieldDescription.Key, out fieldValue) || fieldValue == null)
			{
				return null;
			}

			var sourceType = fieldDescription.Value ?? fieldValue.GetType();
			var destinationType = destinationProperty.PropertyType;

			return destinationType.IsAssignableFrom(sourceType) ? fieldValue : ReflectionUtil.ConvertValue(fieldValue, sourceType, destinationType);
		}

		/// <summary>
		/// Gets the field description.
		/// </summary>
		/// <param name="memberInfo">The member info.</param>
		/// <returns>The field description.</returns>
		protected virtual KeyValuePair<string, Type> GetFieldDescription(MemberInfo memberInfo)
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
	}
}