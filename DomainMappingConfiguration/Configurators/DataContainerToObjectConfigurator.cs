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
	public class DataContainerToObjectConfigurator : DefaultMapConfig
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityToDataContainerPropertyMappingConfigurator"/> class.
		/// </summary>
		public DataContainerToObjectConfigurator()
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
			//this.FilterOperations(from, to, DataAttributeManager.GetTypeDataContainerDescription(to)
			//                                 .Select(sourceMember => (IMappingOperation)new SrcReadOperation
			//                                 {
			//                                   Source = new MemberDescriptor(sourceMember),
			//                                   Setter = (destination, value, state) =>
			////            {

			//                                   int i = 0;
			//                                 })).ToArray();

			//this.FilterOperations(from, to, ReflectionUtils.GetPublicFieldsAndProperties(from)
			//          .Where(member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) && ((PropertyInfo)member).GetGetMethod() != null)
			//          .Select(sourceMember => (IMappingOperation)new SrcReadOperation
			//          {
			//            Source = new MemberDescriptor(sourceMember),
			//            Setter = (destination, value, state) =>
			//            {
			//              if (destination == null || value == null || !(destination is DataContainer))
			//              {
			//                return;
			//              }

			//              var sourceType = sourceMember is PropertyInfo ? ((PropertyInfo) sourceMember).PropertyType : ((FieldInfo)sourceMember).FieldType;
			//              var fieldsDescription = DataAttributeManager.GetDataMemberDefinition(sourceMember);
			//              ConvertSourcePropertyToFields(value, sourceType, (DataContainer)destination, fieldsDescription);
			//            }
			//          })).ToArray();

			return null;
		}
	}
}