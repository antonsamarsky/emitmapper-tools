using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace DomainMappingConfiguration.Configurators
{
	/// <summary>
	/// The object to data container configuration.
	/// </summary>
	public class ObjectToDataContainerConfigurator : MapConfigBase<ObjectToDataContainerConfigurator>
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
			return this.FilterOperations(from, to, ReflectionUtils.GetTypeDataContainerDescription(from)
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

													var sourceType = EmitMapper.Utils.ReflectionUtils.GetMemberType(sourceMember);
													var destinationMemberValue = ReflectionUtils.ConvertValue(value, sourceType, fieldType);

													container.Fields.Add(fieldName, destinationMemberValue);
												}
											};
									})).ToArray();
		}
	}
}