using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.Utils;

namespace MappingDefinitions
{
	/// <summary>
	/// The helper class for data attributes.
	/// </summary>
	public static class DataAttributeManager
	{
		/// <summary>
		/// The collection of attributes values.
		/// </summary>
		private static readonly ConcurrentDictionary<MemberInfo, List<Tuple<string, Type>>> MemberFieldsDescription = new ConcurrentDictionary<MemberInfo, List<Tuple<string, Type>>>();

		/// <summary>
		/// The type data container description.
		/// </summary>
		private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Tuple<MemberInfo, Type>>> TypeDataContainerDescriptions = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Tuple<MemberInfo, Type>>>();

		/// <summary>
		/// Gets the fields description.
		/// </summary>
		/// <param name="memberInfo">The member info.</param>
		/// <returns>
		/// The fields description.
		/// </returns>
		public static List<Tuple<string, Type>> GetDataMemberDefinition(MemberInfo memberInfo)
		{
			return MemberFieldsDescription.GetOrAdd(memberInfo, mi => (from attribute in Attribute.GetCustomAttributes(mi, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>()
																																 let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? mi.Name : attribute.FieldName
																																 let memberType = mi is PropertyInfo ? ((PropertyInfo)mi).PropertyType : (mi is FieldInfo ? ((FieldInfo)mi).FieldType : null)
																																 let fieldType = attribute.FieldType ?? memberType
																																 select Tuple.Create(fieldName, fieldType)).ToList());
		}

		/// <summary>
		/// Gets the type's data container.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>The fields description. </returns>
		public static ConcurrentDictionary<string, Tuple<MemberInfo, Type>> GetTypeDataContainerDescription<T>()
		{
			return GetTypeDataContainerDescription(typeof(T));
		}

		/// <summary>
		/// Gets the type's data container.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The fields description.</returns>
		public static ConcurrentDictionary<string, Tuple<MemberInfo, Type>> GetTypeDataContainerDescription(Type type)
		{
			if (type == null)
			{
				return null;
			}

			return TypeDataContainerDescriptions.GetOrAdd(type, t =>
			{
				var members = ReflectionUtils.GetPublicFieldsAndProperties(t);

				var fieldsDescription = from member in members
																let membersDefinitions = GetDataMemberDefinition(member)
																from fieldDescription in membersDefinitions
																let fieldName = fieldDescription.Item1
																let fieldType = fieldDescription.Item2
																select new KeyValuePair<string, Tuple<MemberInfo, Type>>(fieldName, Tuple.Create(member, fieldType));

				return new ConcurrentDictionary<string, Tuple<MemberInfo, Type>>(fieldsDescription);
			});
		}
	}
}