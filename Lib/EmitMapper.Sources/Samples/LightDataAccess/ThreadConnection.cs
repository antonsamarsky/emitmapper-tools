using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace LightDataAccess
{
	public class ThreadConnection: IDisposable
	{
		[ThreadStatic]
		private static DbConnection connection;

		[ThreadStatic]
		private static int entriesCount;

		public ThreadConnection(Func<DbConnection> connectionCreator)
		{
            if (connection == null || connection.State == System.Data.ConnectionState.Broken)
			{
				connection = connectionCreator();
			}
			entriesCount++;
		}

		public DbConnection Connection
		{
			get
			{
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
				return connection;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (entriesCount <= 1)
			{
                if (connection != null)
                {
                    connection.Close();
                }
				connection = null;
				entriesCount = 0;
			}
			else
			{
				entriesCount--;
			}
		}

		#endregion
	}
}
