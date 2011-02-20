using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using MappingDefinitions;

namespace DomainMappingConfiguration.Configurators
{
	/// <summary>
	/// The object to data container configuration.
	/// </summary>
	public class ObjectToDataContainerConfigurator : DefaultMapConfig
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectToDataContainerConfigurator"/> class.
		/// </summary>
		public ObjectToDataContainerConfigurator()
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
			return this.FilterOperations(from, to, DataAttributeManager.GetTypeDataContainerDescription(from)
									.Select(fieldsDescription =>
									{
										var fieldName = fieldsDescription.Key;
										var sourceMember = fieldsDescription.Value.Item1;
										var fieldType = fieldsDescription.Value.Item2;
										return new SrcReadOperation
											{
												Source = new MemberDescriptor(sourceMember),
												Setter = (destination, value, state) =>
												{
													if (destination == null || value == null || !(destination is DataContainer))
													{
														return;
													}

													var container = destination as DataContainer;

													var sourceType = ReflectionUtils.GetMemberType(sourceMember);
													var destinationMemberValue = ReflectionUtil.ConvertValue(value, sourceType, fieldType);

													container.Fields.Add(fieldName, destinationMemberValue);
												}
											};
									})).ToArray();
		}
	}
}