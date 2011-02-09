using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using MappingDefinition;
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
                                if (!(item is Table))
                                {
                                    return new ValueToWrite<object>();
                                }

                                var dataMemberAttribute = Attribute.GetCustomAttributes(destinationMember, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>().FirstOrDefault();
                                string fieldName = dataMemberAttribute != null && !string.IsNullOrEmpty(dataMemberAttribute.FieldName) ? dataMemberAttribute.FieldName : destinationMember.Name;

                                // Map field to property value.
                                var fieldValue = ((Table)item).Fields[fieldName];
                                if (fieldValue == null)
                                {
                                    return new ValueToWrite<object>();
                                }

                                var converter = TypeDescriptor.GetConverter(((PropertyInfo)destinationMember).PropertyType);
                                if (converter == null)
                                {
                                    return new ValueToWrite<object>();
                                }

                                var destinationMemberValue = converter.ConvertFromString(fieldValue);
                                return destinationMemberValue == null ? new ValueToWrite<object>() : ValueToWrite<object>.ReturnValue(destinationMemberValue);
                            })
                     })).ToArray();
        }
    }
}