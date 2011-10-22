using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightDataAccess
{
	public static class ConvertUtils
	{
		public static Guid ToGuid(this string str)
		{
			if (str == null)
			{
				return Guid.Empty;
			}
			return new Guid(str);
		}

		public static string ToGuidStr(this string str)
		{
			if (str == null)
			{
				return null;
			}
			return str.ToUpper();
		}

		public static string ToGuidStr(this Guid guid)
		{
			return guid.ToString().ToUpper();
		}

		public static string ToGuidStr(this Guid? guid)
		{
			if (guid == null)
			{
				return null;
			}
			return guid.Value.ToString().ToUpper();
		}

		public static bool ToBool(this short s)
		{
			return s != 0;
		}

		public static short ToShort(this bool b)
		{
			return b ? (short)1 : (short)0;
		}

		public static bool ToBool(this short? s)
		{
			return s != 0;
		}
	}
}
