using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using EmitMapper;
using EmitMapper.Mappers;
using LightDataAccess.MappingConfigs;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace LightDataAccess
{
	public static partial class CommandBuilder
	{
		public static bool BuildUpdateOperator(
			this DbCommand cmd,
			object obj,
			string tableName,
            string[] idFieldNames,
			DbSettings dbSettings
		)
		{
			return BuildUpdateCommand(cmd, obj, tableName, idFieldNames, null, null, null, dbSettings);
		}

		public static bool BuildUpdateCommand(
			this DbCommand cmd,
			object obj,
			string tableName,
			IEnumerable<string> idFieldNames,
			IEnumerable<string> includeFields,
			IEnumerable<string> excludeFields,
			ObjectsChangeTracker changeTracker,
			DbSettings dbSettings
		)
		{
            if(idFieldNames == null)
            {
                idFieldNames = new string[0];
            }
            idFieldNames = idFieldNames.Select (n => n.ToUpper()).ToArray();

			if (changeTracker != null)
			{
				var changedFields = changeTracker.GetChanges(obj);
				if (changedFields != null)
				{
					if (includeFields == null)
					{
						includeFields = changedFields.Select(c => c.name).ToArray();
					}
					else
					{
						includeFields = includeFields.Intersect(changedFields.Select(c => c.name)).ToArray();
					}
				}
			}
			if (includeFields != null)
			{
				includeFields = includeFields.Concat(idFieldNames);
			}
			IMappingConfigurator config = new AddDbCommandsMappingConfig(
					dbSettings,
					includeFields,
					excludeFields,
					"updateop_inc_" + includeFields.ToCSV("_") + "_exc_" + excludeFields.ToCSV("_")
			);

			var mapper = ObjectMapperManager.DefaultInstance.GetMapperImpl(
				obj.GetType(),
				typeof(DbCommand),
				config
			);

            string[] fields = mapper
				.StroredObjects
				.OfType<SrcReadOperation>()
				.Select( m => m.Source.MemberInfo.Name )
				.Where( f => !idFieldNames.Contains(f) )
				.ToArray();

			if (fields.Length == 0)
			{
				return false;
			}

			var cmdStr =
				"UPDATE " + 
				tableName +
				" SET " + 
				fields
					.Select(
						f => dbSettings.GetEscapedName(f) + "=" + dbSettings.GetParamName(f)
					)
					.ToCSV(",") +
				" WHERE " +
                idFieldNames.Select(fn => dbSettings.GetEscapedName(fn) + "=" + dbSettings.GetParamName(fn)).ToCSV(" AND ")
				;
			cmd.CommandText = cmdStr;
			cmd.CommandType = System.Data.CommandType.Text;

			mapper.Map(obj, cmd, null);
			return true;
		}
	}
}
