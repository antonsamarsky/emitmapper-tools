using System.Linq;
using System.Data.Common;
using EmitMapper;
using EmitMapper.Mappers;
using LightDataAccess.MappingConfigs;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace LightDataAccess
{
	public static partial class CommandBuilder
	{
		public static DbCommand BuildInsertCommand(
			this DbCommand cmd,
			object obj,
			string tableName,
			DbSettings dbSettings
		)
		{
			return BuildInsertCommand(cmd, obj, tableName, dbSettings, null, null);
		}

		public static DbCommand BuildInsertCommand(
			this DbCommand cmd,
			object obj,
			string tableName,
			DbSettings dbSettings,
			string[] includeFields,
			string[] excludeFields
		)
		{
			IMappingConfigurator config = new AddDbCommandsMappingConfig(
					dbSettings,
					includeFields,
					excludeFields,
					"insertop_inc_" + includeFields.ToCSV("_") + "_exc_" + excludeFields.ToCSV("_")
			);

			var mapper = ObjectMapperManager.DefaultInstance.GetMapperImpl(
				obj.GetType(), 
				typeof(DbCommand), 
				config
			);

			string[] fields = mapper.StroredObjects.OfType<SrcReadOperation>().Select(m => m.Source.MemberInfo.Name).ToArray();

			var cmdStr = 
				"INSERT INTO " 
				+ tableName + 
				"("
				+ fields
					.Select(f => dbSettings.GetEscapedName(f))
					.ToCSV(",")
				+ ") VALUES ("
				+ fields
					.Select( f => dbSettings.GetParamName(f))
					.ToCSV(",")
				+ ")"
				;
			cmd.CommandText = cmdStr;
			cmd.CommandType = System.Data.CommandType.Text;

			mapper.Map(obj, cmd, null);
			return cmd;
		}
	}
}
