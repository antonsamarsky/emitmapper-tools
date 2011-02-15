using System;
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
											var table = item as Table;

											if (table == null)
											{
												return ValueToWrite<object>.Skip();
											}

											var attribute = Attribute.GetCustomAttributes(destinationMember, typeof(DataMemberAttribute), true).FirstOrDefault();

											if (attribute == null)
											{
												return ValueToWrite<object>.Skip();
											}

											var fieldName = !string.IsNullOrEmpty(((DataMemberAttribute)attribute).FieldName) ? ((DataMemberAttribute)attribute).FieldName : destinationMember.Name;

											string fieldValue;
											if (!table.Fields.TryGetValue(fieldName, out fieldValue) || fieldValue == null)
											{
												return ValueToWrite<object>.Skip();
											}

											var converter = TypeDescriptor.GetConverter(((PropertyInfo)destinationMember).PropertyType);
											if (converter == null || !converter.CanConvertFrom(typeof(string)))
											{
												return ValueToWrite<object>.Skip();
											}

											var destinationMemberValue = converter.ConvertFromString(fieldValue);
											return destinationMemberValue == null ? ValueToWrite<object>.Skip() : ValueToWrite<object>.ReturnValue(destinationMemberValue);
										})
							})).ToArray();
		}
	}
}