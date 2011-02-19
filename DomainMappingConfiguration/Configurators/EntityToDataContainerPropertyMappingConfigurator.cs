using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using MappingDefinitions;

namespace DomainMappingConfiguration.Configurators
{
	/// <summary>
	/// The item configuration.
	/// </summary>
	public class EntityToDataContainerPropertyMappingConfigurator : DefaultMapConfig
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityToDataContainerPropertyMappingConfigurator"/> class.
		/// </summary>
		public EntityToDataContainerPropertyMappingConfigurator()
		{
			ConstructBy(() => new DataContainer { Fields = new Dictionary<string, object>() });
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
											if (destination == null || value == null || !(destination is DataContainer))
											{
												return;
											}

											var sourceType = sourceMember is PropertyInfo ? ((PropertyInfo) sourceMember).PropertyType : ((FieldInfo)sourceMember).FieldType;
											var fieldsDescription = DataAttributeManager.GetDataMemberDefinition(sourceMember);
											ConvertSourcePropertyToFields(value, sourceType, (DataContainer)destination, fieldsDescription);
										}
									})).ToArray();
		}

		/// <summary>
		/// Converts the source property to fields.
		/// </summary>
		/// <param name="sourceType">Type of the property.</param>
		/// <param name="sourceValue">The property value.</param>
		/// <param name="container">The container.</param>
		/// <param name="fieldsDescription">The fields description.</param>
		private static void ConvertSourcePropertyToFields(object sourceValue, Type sourceType, DataContainer container, List<Tuple<string, Type>> fieldsDescription)
		{
			if (container == null || container.Fields == null)
			{
				return;
			}

			fieldsDescription.ForEach(fd =>
			{
				if (container.Fields.ContainsKey(fd.Item1))
				{
					return;
				}

				var value = ReflectionUtil.ConvertValue(sourceValue, sourceType, fd.Item2);

				if (value != null)
				{
					container.Fields.Add(fd.Item1, value);
				}
			});
		}
	}
}
