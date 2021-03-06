using System;
using System.Linq;
using Domain;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace DomainMappingConfiguration.Configurators
{
	/// <summary>
	/// The data container to object configuration.
	/// </summary>
	public class DataContainerToObjectConfigurator : MapConfigBase<DataContainerToObjectConfigurator>
	{
		/// <summary>
		/// Gets the mapping operations.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">To type to.</param>
		/// <returns>The mapping operations.</returns>
		public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			return this.FilterOperations(from, to, ReflectionUtils.GetTypeDataContainerDescription(to)
									.Select(fieldsDescription =>
									{
										var fieldName = fieldsDescription.Key;
										var destinationMember = fieldsDescription.Value.Item1;
										var fieldType = fieldsDescription.Value.Item2;
										return new DestWriteOperation
											{
												Destination = new MemberDescriptor(destinationMember),
												Getter = (ValueGetter<object>)((item, state) =>
												{
													if (item == null || !(item is DataContainer))
													{
														return ValueToWrite<object>.Skip();
													}
													var container = item as DataContainer;

													var destinationType = EmitMapper.Utils.ReflectionUtils.GetMemberType(destinationMember);
													var destinationMemberValue = ReflectionUtils.ConvertValue(container.Fields[fieldName], fieldType, destinationType);

													return ValueToWrite<object>.ReturnValue(destinationMemberValue);
												})
											};
									})).ToArray();
		}
	}
}