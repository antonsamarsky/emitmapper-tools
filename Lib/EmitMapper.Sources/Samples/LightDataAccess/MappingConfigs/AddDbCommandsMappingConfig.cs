using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration;
using System.Data.Common;

namespace LightDataAccess.MappingConfigs
{
	class AddDbCommandsMappingConfig : IMappingConfigurator
	{
		DbSettings _dbSettings;
		IEnumerable<string> _includeFields;
		IEnumerable<string> _excludeFields;
		string _configName;

		public AddDbCommandsMappingConfig(
			DbSettings dbSettings,
			IEnumerable<string> includeFields,
			IEnumerable<string> excludeFields,
			string configName)
		{
			_dbSettings = dbSettings;
			_includeFields = includeFields;
			_excludeFields = excludeFields;
			_configName = configName;

			if (_includeFields != null)
			{
				_includeFields = _includeFields.Select(f => f.ToUpper());
			}

			if (_excludeFields != null)
			{
				_excludeFields = _excludeFields.Select(f => f.ToUpper());
			}

		}


		#region IMappingConfigurator Members
		public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
		{
			return null;
		}

		public IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			var members = ReflectionUtils.GetPublicFieldsAndProperties(from);
			if (_includeFields != null)
			{
				members = members
					.Where( m => _includeFields.Contains(m.Name.ToUpper()) )
					.ToArray();
			}

			if (_excludeFields != null)
			{
				members = members
					.Where( m => !_excludeFields.Contains(m.Name.ToUpper()) )
					.ToArray();
			}

			return members
				.Select(
					m => new SrcReadOperation
					{
						Source = new MemberDescriptor(new[] { m }),
						Setter = (obj, v, s) => ((DbCommand)obj).AddParam(_dbSettings.paramPrefix + m.Name, v)
					}
				)
				.ToArray();
		}

		public string GetConfigurationName()
		{
			return _configName;
		}


		#endregion

		public StaticConvertersManager GetStaticConvertersManager()
		{
			return null;
		}
	}
}
