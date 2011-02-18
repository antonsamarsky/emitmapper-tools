using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace DomainMappingConfiguration
{
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
		/// Converts the value.
		/// </summary>
		/// <param name="sourceValue">The source value.</param>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <returns>The converted value.</returns>
		public static object ConvertValue(object sourceValue, Type sourceType, Type destinationType)
		{
			object result = null;

			TypeConverter typeConverter = TypeCoverters.GetOrAdd(destinationType, TypeDescriptor.GetConverter);
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