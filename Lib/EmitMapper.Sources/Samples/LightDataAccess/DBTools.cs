using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace LightDataAccess
{
	public static class DBTools
	{
        public static int ExecuteNonQuery(DbConnection conn, string commandText, CmdParams cmdParams)
        {
            using (var cmd = CreateCommand(conn, commandText, cmdParams))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(DbConnection conn, string commandText, CmdParams cmdParams)
        {
            using (var cmd = CreateCommand(conn, commandText, cmdParams))
            {
                return cmd.ExecuteScalar();
            }
        }

        public static T ExecuteScalar<T>(DbConnection conn, string commandText, CmdParams cmdParams)
        {
            object result = ExecuteScalar(conn, commandText, cmdParams);
            if (typeof(T) == typeof(Guid))
            {
                if(result == null)
                {
                    return (T)((object)Guid.Empty);
                }
                return (T)((object)new Guid(result.ToString()));
            }
            if (result is DBNull || result == null)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }

		public static DbDataReader ExecuteReader(DbConnection conn, string commandText, CmdParams cmdParams)
		{
			using (var cmd = CreateCommand(conn, commandText, cmdParams))
			{
				return cmd.ExecuteReader();
			}
		}

        public static T ExecuteReader<T>(DbConnection conn, string commandText, CmdParams cmdParams, Func<DbDataReader, T> func) where T:class
        {
            using (var cmd = CreateCommand(conn, commandText, cmdParams))
            using (var reader = cmd.ExecuteReader())
            {
				if (reader.Read())
				{
					return func(reader);
				}
				else
				{
					return null;
				}
            }
        }

        public static T ExecuteReaderStruct<T>(DbConnection conn, string commandText, CmdParams cmdParams, Func<DbDataReader, T> func) where T : struct 
        {
            using (var cmd = CreateCommand(conn, commandText, cmdParams))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return func(reader);
                }
                else
                {
                    return default(T);
                }
            }
        }

        public static IEnumerable<T> ExecuteReaderEnum<T>(DbConnection conn, string commandText, CmdParams cmdParams, Func<DbDataReader, T> func)
        {
            using (var cmd = CreateCommand(conn, commandText, cmdParams))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

		public static IEnumerable<T> ReadCollection<T>(
			DbConnection conn,
			string commandText,
			CmdParams cmdParams,
			string[] excludeFields)
		{
			return ReadCollection<T>(conn, commandText, cmdParams, excludeFields, null);
		}

        public static IEnumerable<T> ReadCollection<T>(
			DbConnection conn, 
			string commandText, 
			CmdParams cmdParams, 
			string[] excludeFields,
			ObjectsChangeTracker changeTracker)
        {
            using (var cmd = CreateCommand(conn, commandText, cmdParams))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
					yield return reader.ToObject<T>(null, excludeFields, changeTracker);
                }
            }
        }

        public static DbCommand CreateCommand(DbConnection conn, string commandText)
        {
            var result = CreateCommand(conn, commandText, null);
            return result;
        }

		public static DbCommand AddParam(this DbCommand cmd, string paramName, object paramValue)
		{
			if (paramValue is Guid)
			{
				paramValue = ((Guid)paramValue).ToGuidStr();
			}

			if (paramValue == null)
			{
				paramValue = DBNull.Value;
			}

			var par = cmd.CreateParameter();
			par.ParameterName = paramName;
			par.Value = paramValue;
			cmd.Parameters.Add(par);
			return cmd;
		}

        public static DbCommand CreateCommand(DbConnection conn, string commandText, CmdParams cmdParams)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            var result = conn.CreateCommand();
            result.CommandText = commandText;
            result.CommandType = CommandType.Text;
            if (cmdParams != null)
            {
                foreach (var param in cmdParams)
                {
                    object value;
                    if (param.Value is Guid)
                    {
                        value = ((Guid)param.Value).ToGuidStr();
                    }
                    else if (param.Value is bool)
                    {
                        value = ((bool)param.Value).ToShort();
                    }
                    else
                    {
                        value = param.Value;
                    }
                    result.AddParam(param.Key, value);
                }
            }

            return result;
        }

        public static DbCommand CreateStoredProcedureCommand(DbConnection conn, string spName)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            var result = conn.CreateCommand();
            result.CommandText = spName;
            result.CommandType = CommandType.StoredProcedure;
            return result;
        }

        public static string ToCSV<T>(this IEnumerable<T> collection, string delim)
        {
			if (collection == null)
			{
				return "";
			}

            StringBuilder result = new StringBuilder();
            foreach (T value in collection)
            {
                result.Append(value);
                result.Append(delim);
            }
            if (result.Length > 0)
            {
                result.Length -= delim.Length;
            }
            return result.ToString();
        }

		public static T ToObject<T>(this DbDataReader reader)
		{
			return reader.ToObject<T>(null, null, null);
		}

		public static T ToObject<T>(this DbDataReader reader, string readerName)
		{
			return reader.ToObject<T>(readerName, null, null);
		}

		public static T ToObject<T>(this DbDataReader reader, string[] excludeFields)
		{
			return reader.ToObject<T>(null, excludeFields, null);
		}

		public static T ToObject<T>(this DbDataReader reader, string readerName, string[] excludeFields, ObjectsChangeTracker changeTracker)
		{
			T result = new DataReaderToObjectMapper<T>(readerName, null, excludeFields).MapUsingState(reader, reader);
			if (changeTracker != null)
			{
				changeTracker.RegisterObject(result);
			}
			return result;
		}

		public static IEnumerable<T> ToObjects<T>(this DbDataReader reader)
		{
			return reader.ToObjects<T>(null, null, null);
		}

		public static IEnumerable<T> ToObjects<T>(this DbDataReader reader, string readerName)
		{
			return reader.ToObjects<T>(readerName, null, null);
		}

		public static IEnumerable<T> ToObjects<T>(this DbDataReader reader, string[] excludeFields)
		{
			return reader.ToObjects<T>(null, excludeFields, null);
		}

		public static IEnumerable<T> ToObjects<T>(this DbDataReader reader, string readerName, string[] excludeFields, ObjectsChangeTracker changeTracker)
		{
			if (string.IsNullOrEmpty(readerName))
			{
				var mappingKeyBuilder = new StringBuilder();
				for (int i = 0; i < reader.FieldCount; ++i)
				{
					mappingKeyBuilder.Append(reader.GetName(i));
					mappingKeyBuilder.Append(' ');
				}
				readerName = mappingKeyBuilder.ToString();
			}
			return new DataReaderToObjectMapper<T>(readerName, null, excludeFields).ReadCollection(reader, changeTracker);
		}

        public static void InsertObject(
            DbConnection conn,
            object obj,
            string tableName,
            DbSettings dbSettings,
            string[] includeFields,
            string[] excludeFields
        )
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.BuildInsertCommand(obj, tableName, dbSettings, includeFields, excludeFields);
                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertObject(
            DbConnection conn,
            object obj,
            string tableName,
            DbSettings dbSettings
        )
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.BuildInsertCommand(obj, tableName, dbSettings);
                cmd.ExecuteNonQuery();
            }
        }

		public static void UpdateObject(
			DbConnection conn,
			object obj,
			string tableName,
			string[] idFieldNames,
			ObjectsChangeTracker changeTracker,
			DbSettings dbSettings
		)
		{
			UpdateObject(conn, obj, tableName, idFieldNames, null, null, changeTracker, dbSettings);
		}

		public static void UpdateObject(
			DbConnection conn,
			object obj,
			string tableName,
			string[] idFieldNames,
			string[] includeFields,
			string[] excludeFields,
			ObjectsChangeTracker changeTracker,
			DbSettings dbSettings
		)
        {
            using (var cmd = conn.CreateCommand())
            {
				if (
					cmd.BuildUpdateCommand(
						obj, 
						tableName, 
						idFieldNames, 
						includeFields, 
						excludeFields, 
						changeTracker, 
						dbSettings
					)
				)
				{
					cmd.ExecuteNonQuery();
				}
            }
        }

        public static void UpdateObject(
			DbConnection conn,
			object obj,
			string tableName,
			string[] idFieldNames,
			DbSettings dbSettings
		)
        {
			UpdateObject(conn, obj, tableName, idFieldNames, null, null, null, dbSettings);
        }
	}
}
