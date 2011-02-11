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

namespace Mapping
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
											if (!(item is Table))
											{
												return ValueToWrite<object>.Skip();
											}

											var dataMemberAttribute = Attribute.GetCustomAttributes(destinationMember, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>().FirstOrDefault();
											var fieldName = dataMemberAttribute != null && !string.IsNullOrEmpty(dataMemberAttribute.FieldName) ? dataMemberAttribute.FieldName : destinationMember.Name;

											// Map field to property value.
											if (!((Table)item).Fields.ContainsKey(fieldName))
											{
												return ValueToWrite<object>.Skip();
											}

											var fieldValue = ((Table)item).Fields[fieldName];
											if (fieldValue == null)
											{
												return ValueToWrite<object>.Skip();
											}

											var converter = TypeDescriptor.GetConverter(((PropertyInfo)destinationMember).PropertyType);
											if (converter == null)
											{
												return ValueToWrite<object>.Skip();
											}

											if (!converter.CanConvertFrom(typeof(string)))
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