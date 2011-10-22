using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Diagnostics;
using MappingDefinitions;

namespace DomainMappingConfiguration
{
	/// http://msdn.microsoft.com/en-us/library/08h86h00.aspx
	/// <summary>
	/// The reflection util class.
	/// </summary>
	public static class ReflectionUtils
	{
		/// <summary>
		/// The converters collection.
		/// </summary>
		private static readonly ConcurrentDictionary<Type, TypeConverter> TypeConverters = new ConcurrentDictionary<Type, TypeConverter>();

		/// <summary>
		/// The collection of not IConvertible types.
		/// </summary>
		private static readonly List<Tuple<Type, Type>> TypesNotIConvertible = new List<Tuple<Type, Type>>();

		/// <summary>
		/// Converts the value.
		/// </summary>
		/// <param name="sourceValue">The source value.</param>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <returns>The converted value.</returns>
		public static object ConvertValue(object sourceValue, Type sourceType, Type destinationType)
		{
			if (sourceValue == null || sourceType == null || destinationType == null)
			{
				return null;
			}

			if (sourceType == destinationType || destinationType.IsAssignableFrom(sourceType))
			{
				return sourceValue;
			}

			if (sourceValue is IConvertible && !TypesNotIConvertible.Contains(Tuple.Create(sourceType, destinationType)))
			{
			  try
			  {
			    return Convert.ChangeType(sourceValue, destinationType);
			  }
			  catch
			  {
			    TypesNotIConvertible.Add(Tuple.Create(sourceType, destinationType));
			  }			
			}

			object result = null;

			var typeConverter = TypeConverters.GetOrAdd(destinationType, TypeDescriptor.GetConverter);
			if (typeConverter != null && typeConverter.CanConvertFrom(sourceType))
			{
				result = typeConverter.ConvertFrom(sourceValue);
			}
			else
			{
				typeConverter = TypeConverters.GetOrAdd(sourceType, TypeDescriptor.GetConverter);
				if (typeConverter != null && typeConverter.CanConvertTo(destinationType))
				{
					result = typeConverter.ConvertTo(sourceValue, destinationType);
				}
			}

			return result;
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
			Assert.ArgumentNotNull(type, "type");

			var fieldsDescription = from member in EmitMapper.Utils.ReflectionUtils.GetPublicFieldsAndProperties(type)
															let membersDefinitions = GetDataMemberDefinition(member)
															from fieldDescription in membersDefinitions
															let fieldName = fieldDescription.Item1
															let fieldType = fieldDescription.Item2
															select new KeyValuePair<string, Tuple<MemberInfo, Type>>(fieldName, Tuple.Create(member, fieldType));

			return new ConcurrentDictionary<string, Tuple<MemberInfo, Type>>(fieldsDescription);
		}

		/// <summary>
		/// Gets the fields description.
		/// </summary>
		/// <param name="memberInfo">The member info.</param>
		/// <returns>
		/// The fields description.
		/// </returns>
		public static IEnumerable<Tuple<string, Type>> GetDataMemberDefinition(MemberInfo memberInfo)
		{
			Assert.ArgumentNotNull(memberInfo, "memberInfo");

			return from attribute in Attribute.GetCustomAttributes(memberInfo, typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>()
						 let fieldName = string.IsNullOrEmpty(attribute.FieldName) ? memberInfo.Name : attribute.FieldName
						 let memberType = EmitMapper.Utils.ReflectionUtils.GetMemberType(memberInfo)
						 let fieldType = attribute.FieldType ?? memberType
						 select Tuple.Create(fieldName, fieldType);
		}
	}
}