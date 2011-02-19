using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightDataAccess
{
	public class DbSettings
	{
		public string firstNameEscapeSymbol;
		public string secondNameEscapeSymbol;
		public string paramPrefix;

		public static DbSettings MySQL;
		public static DbSettings MSSQL;

		static DbSettings()
		{
			MySQL = new DbSettings
			{
				firstNameEscapeSymbol = "`",
				secondNameEscapeSymbol = "`",
				paramPrefix = "@p_"
			};

			MSSQL = new DbSettings
			{
				firstNameEscapeSymbol = "[",
				secondNameEscapeSymbol = "]",
				paramPrefix = "@p_"
			};

		}

		public string GetParamName(string fieldName)
		{
			return paramPrefix + fieldName;
		}

		public string GetEscapedName(string name)
		{
			return firstNameEscapeSymbol + name + secondNameEscapeSymbol;
		}

	}
}
