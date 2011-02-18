using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
		private static readonly ConcurrentDictionary<MemberInfo, List<KeyValuePair<string, Type>>> MemberFieldsDescription;

		/// <summary>
		/// Initializes the <see cref="DataAttributeManager"/> class.
		/// </summary>
		static DataAttributeManager()
		{
			MemberFieldsDescription = new ConcurrentDictionary<MemberInfo, List<KeyValuePair<string, Type>>>();
		}

		/// <summary>
		/// Gets the fields description.
		/// </summary>
		/// <param name="memberInfo">The member info.</param>
		/// <returns>
		/// The fields description.
		/// </returns>
		public static List<KeyValuePair<string, Type>> GetDataMemberDefinition(MemberInfo memberInfo)
		{
			return MemberFieldsDescription.GetOrAdd(memberInfo, mi =>
			{
				var attributes = Attribute.GetCustomAttributes(memberInfo, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>();

				return (from attribute in attributes
								let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? mi.Name : attribute.FieldName
								let fieldType = attribute.FieldType ?? ((PropertyInfo)mi).PropertyType
								select new KeyValuePair<string, Type>(fieldName, fieldType)).ToList();
			});
		}
	}
}