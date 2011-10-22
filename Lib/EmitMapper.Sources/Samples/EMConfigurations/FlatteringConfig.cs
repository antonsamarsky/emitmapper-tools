using System;
using System.Collections.Generic;
using System.Linq;
using EmitMapper;
using System.Reflection;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EMConfigurations
{
	public class FlatteringConfig : DefaultMapConfig 
	{
		protected Func<string, string, bool> nestedMembersMatcher;

		public FlatteringConfig()
		{
			nestedMembersMatcher = (m1, m2) => m1.StartsWith(m2);
		}

		public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			var destinationMembers = GetDestinationMemebers(to);
			var sourceMembers = GetSourceMemebers(from);
			var result = new List<IMappingOperation>();
			foreach (var dest in destinationMembers)
			{
				var matchedChain = GetMatchedChain(dest.Name, sourceMembers).ToArray();
				if (matchedChain == null || matchedChain.Length == 0)
				{
					continue;
				}
				result.Add(
					new ReadWriteSimple
					{
						Source = new MemberDescriptor(matchedChain),
						Destination = new MemberDescriptor(new[] { dest })
					}
				);
			}
			return result.ToArray();
		}

		public DefaultMapConfig MatchNestedMembers(Func<string, string, bool> nestedMembersMatcher)
		{
			this.nestedMembersMatcher = nestedMembersMatcher;
			return this;
		}

		private List<MemberInfo> GetMatchedChain(string destName, List<MemberInfo> sourceMembers)
		{
            var matches = sourceMembers.Where(s => MatchMembers(destName, s.Name) || nestedMembersMatcher(destName, s.Name));
			int len = 0;
			MemberInfo match = null;
			foreach (var m in matches)
			{
				if (m.Name.Length > len)
				{
					len = m.Name.Length;
					match = m;
				}
			}
			if (match == null)
			{
				return null;
			}
			var result = new List<MemberInfo> { match };
            if (!MatchMembers(destName, match.Name))
			{
				result.AddRange(
					GetMatchedChain(destName.Substring(match.Name.Length), GetDestinationMemebers(match))
				);
			}
			return result;
		}

		private static List<MemberInfo> GetSourceMemebers(Type t)
		{
			return GetMemebers(t)
				.Where(
					m => 
						m.MemberType == MemberTypes.Field || 
						m.MemberType == MemberTypes.Property ||
						m.MemberType == MemberTypes.Method
				)
				.ToList();
		}

		private static List<MemberInfo> GetDestinationMemebers(MemberInfo mi)
		{
			Type t;
			if (mi.MemberType == MemberTypes.Field)
			{
				t = mi.DeclaringType.GetField(mi.Name).FieldType;
			}
			else
			{
				t = mi.DeclaringType.GetProperty(mi.Name).PropertyType;
			}
			return GetDestinationMemebers(t);
		}

		private static List<MemberInfo> GetDestinationMemebers(Type t)
		{
			return GetMemebers(t).Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property).ToList();
		}

		private static List<MemberInfo> GetMemebers(Type t)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			return t.GetMembers(bindingFlags).ToList();
		}
	}
}
