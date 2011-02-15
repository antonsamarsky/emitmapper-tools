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
        private static readonly Type DefaultType = typeof(string);

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
                                    dynamic table = destination;

                                    if (table == null || value == null)
                                    {
                                        return;
                                    }

                                    var fieldDescription = from attribute in Attribute.GetCustomAttributes(sourceMember, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>()
                                                           let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? sourceMember.Name : attribute.FieldName
                                                           let fieldType = attribute.FieldType ?? DefaultType
                                                           select new { Name = fieldName, Type = fieldType };

                                    if (!fieldDescription.Any())
                                    {
                                        return;
                                    }

                                    // NOTE: Map the property value to the filed.
                                    var sourceType = ((PropertyInfo)sourceMember).PropertyType;
                                    var converterLazy = new Lazy<TypeConverter>(() => TypeDescriptor.GetConverter(((PropertyInfo)sourceMember).PropertyType));
                                    
                                    fieldDescription.Distinct().ToList().ForEach(fd =>
                                    {
                                        if (table.Fields.ContainsKey(fd.Name))
                                        {
                                            return;
                                        }

                                        dynamic mappedValue = null;
                                        if (fd.Type.IsAssignableFrom(sourceType))
                                        {
                                            mappedValue = value;
                                        }
                                        else
                                        {
                                            var converter = converterLazy.Value;
                                            if (converter != null && converter.CanConvertTo(fd.Type))
                                            {
                                                mappedValue = converter.ConvertTo(value, fd.Type);
                                            }
                                        }

                                        if (mappedValue != null)
                                        {
                                            table.Fields.Add(fd.Name, mappedValue);
                                        }
                                    });

                                }
                            })).ToArray();
        }
    }
}
