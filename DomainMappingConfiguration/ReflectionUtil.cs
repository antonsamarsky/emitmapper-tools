using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace DomainMappingConfiguration
{
	/// http://msdn.microsoft.com/en-us/library/08h86h00.aspx
	/// <summary>
	/// The reflection util class.
	/// </summary>
	public static class ReflectionUtil
	{
		/// <summary>
		/// The converters collection.
		/// </summary>
		private static readonly ConcurrentDictionary<Type, TypeConverter> TypeCoverters = new ConcurrentDictionary<Type, TypeConverter>();

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

			var typeConverter = TypeCoverters.GetOrAdd(destinationType, TypeDescriptor.GetConverter);
			if (typeConverter != null && typeConverter.CanConvertFrom(sourceType))
			{
				result = typeConverter.ConvertFrom(sourceValue);
			}
			else
			{
				typeConverter = TypeCoverters.GetOrAdd(sourceType, TypeDescriptor.GetConverter);
				if (typeConverter != null && typeConverter.CanConvertTo(destinationType))
				{
					result = typeConverter.ConvertTo(sourceValue, destinationType);
				}
			}

			return result;
		}
	}
}