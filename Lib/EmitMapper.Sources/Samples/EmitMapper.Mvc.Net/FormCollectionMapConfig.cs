using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration;
using System.Web.Mvc;

namespace EmitMapper.Mvc.Net
{
	public class FormCollectionMapConfig:IMappingConfigurator
	{
		public IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			var members = ReflectionUtils.GetPublicFieldsAndProperties(to);
			return members
				.Select(
					m =>
					new DestWriteOperation()
					{
						Destination = new MemberDescriptor(m),
						Getter =
							(ValueGetter<object>)
							(
								(form, valueProviderObj) =>
								{
									var valueProvider = valueProviderObj as IDictionary<string, ValueProviderResult>;
									if (valueProvider == null)
									{
										valueProvider = ((FormCollection)form).ToValueProvider();
									}

									ValueProviderResult res;
									if(valueProvider.TryGetValue(m.Name, out res))
									{
										return ValueToWrite<object>.ReturnValue(res.ConvertTo(ReflectionUtils.GetMemberType(m)));
									}
									else
									{
										return ValueToWrite<object>.Skip();
									}
								}
							)
					}
				)
				.ToArray();
		}

		public string GetConfigurationName()
		{
			return null;
		}

		public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
		{
			return null;
		}

		public StaticConvertersManager GetStaticConvertersManager()
		{
			return null;
		}
	}
}
