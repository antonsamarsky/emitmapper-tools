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
	public class EntityToTableMappingConfigurator : DefaultMapConfig
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityToTableMappingConfigurator"/> class.
		/// </summary>
		public EntityToTableMappingConfigurator()
		{
			ConstructBy(() => new Table { Fields = new Dictionary<string, object>() });
		}

		/// <summary>
		/// Gets the mapping operations.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">To type to.</param>
		/// <returns>The mapping operations.</returns>
		public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			return this.FilterOperations(from, to, ReflectionUtils.GetPublicFieldsAndProperties(from)
									.Where(member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && ((PropertyInfo)member).GetGetMethod() != null)
									.Select(sourceMember => (IMappingOperation)new SrcReadOperation
									{
										Source = new MemberDescriptor(sourceMember),
										Setter = (destination, value, state) =>
										{
											if (destination == null || value == null || !(sourceMember is PropertyInfo) || !(destination is Table))
											{
												return;
											}

											var fieldsDescription = DataAttributeManager.GetDataMemberDefinition(sourceMember);
											ConvertSourcePropertyToFields(((PropertyInfo)sourceMember).PropertyType, value, (Table)destination, fieldsDescription);
										}
									})).ToArray();
		}

		/// <summary>
		/// Converts the source property to fields.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <param name="propertyValue">The property value.</param>
		/// <param name="table">The dictionary.</param>
		/// <param name="fieldsDescription">The fields description.</param>
		private static void ConvertSourcePropertyToFields(Type propertyType, object propertyValue, Table table, List<KeyValuePair<string, Type>> fieldsDescription)
		{
			if (table == null || table.Fields == null)
			{
				return;
			}

			fieldsDescription.ForEach(fd =>
			{
				if (table.Fields.ContainsKey(fd.Key))
				{
					return;
				}

				var value = fd.Value.IsAssignableFrom(propertyType) ? propertyValue : ReflectionUtil.ConvertValue(propertyValue, propertyType, fd.Value);

				if (value != null)
				{
					table.Fields.Add(fd.Key, value);
				}
			});
		}
	}
}
