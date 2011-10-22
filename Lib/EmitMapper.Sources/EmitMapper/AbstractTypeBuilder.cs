using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper
{
	static class AbstractTypeBuilder
	{
		private static Dictionary<string, Type> _typesCache;

		public static Type BuildType(Type type)
		{
			var typeBuilder = DynamicAssemblyManager.DefineType(type.ToString() + "_"
		}
	}
}
