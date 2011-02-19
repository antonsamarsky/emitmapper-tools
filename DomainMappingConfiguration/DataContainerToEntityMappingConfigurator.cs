using System;
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
	public class DataContainerToEntityMappingConfigurator : DefaultMapConfig
	{
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
											if (item == null || !(item is DataContainer) || !(destinationMember is PropertyInfo))
											{
												return ValueToWrite<object>.Skip();
											}

											var fieldDescription = DataAttributeManager.GetDataMemberDefinition(destinationMember);
											var destinationMemberValue = ConvertFieldToDestinationProperty((DataContainer)item, (PropertyInfo)destinationMember, fieldDescription.FirstOrDefault());

											return destinationMemberValue == null ? ValueToWrite<object>.Skip() : ValueToWrite<object>.ReturnValue(destinationMemberValue);
										})
									})).ToArray();
		}

		/// <summary>
		/// Converts the field to destination property.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="destinationProperty">The destination property.</param>
		/// <param name="fieldDescription">The field description.</param>
		/// <returns>
		/// The conversion result.
		/// </returns>
		private static object ConvertFieldToDestinationProperty(DataContainer container, PropertyInfo destinationProperty, KeyValuePair<string, Type> fieldDescription)
		{
			if (container == null || container.Fields == null || string.IsNullOrEmpty(fieldDescription.Key))
			{
				return null;
			}

			object sourceValue;
			if (!container.Fields.TryGetValue(fieldDescription.Key, out sourceValue) || sourceValue == null)
			{
				return null;
			}

			var sourceType = fieldDescription.Value ?? sourceValue.GetType();
			var destinationType = destinationProperty.PropertyType;

			return ReflectionUtil.ConvertValue(sourceValue, sourceType, destinationType);
		}
	}
}