using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EmitMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LightDataAccess;
using System.Data.Common;
using System.Configuration;
using System.Transactions;

namespace SamplesTests
{
	public class Customer
	{
		public string CustomerID;
		public string CompanyName;
		public string ContactName;
		public string ContactTitle;
		public string Address;
		public string City;
		public string Region;
		public string PostalCode;
		public string Country;
		public string Phone;
		public string Fax;
	}

	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class CustormTests
	{
		public CustormTests()
		{
			_connectionConfig = ConfigurationManager.ConnectionStrings["NorthWind"];
			_factory = DbProviderFactories.GetFactory(_connectionConfig.ProviderName);
		}
		DbProviderFactory _factory;
		ConnectionStringSettings _connectionConfig;

		private DbConnection CreateConnection()
		{
			var result = _factory.CreateConnection();
			result.ConnectionString = _connectionConfig.ConnectionString;
			result.Open();
			return result;
		}

		[TestMethod]
		public void GetCustomers()
		{
			Customer[] customers;
			using(var connection = CreateConnection())
			using(var cmd = _factory.CreateCommand())
			{
				cmd.Connection = connection;
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.CommandText = "select * from [dbo].[Customers]";
				using (var reader = cmd.ExecuteReader())
				{
					customers = reader.ToObjects<Customer>("reader1").ToArray();
				}
			}
		}

		[TestMethod]
		public void UpdateCustomer()
		{
			ObjectMapperManager objMan = new ObjectMapperManager();

			Guid guid = Guid.NewGuid();

			using (var ts = new TransactionScope())
			using (var connection = CreateConnection())
			{
				var customer = DBTools.ExecuteReader(
					connection,
					"select top 1 * from [dbo].[Customers]",
					null,
					r => r.ToObject<Customer>()
				);

				var tracker = new ObjectsChangeTracker();
				tracker.RegisterObject(customer);
				customer.Address = guid.ToString();

				DBTools.UpdateObject(
					connection,
					customer,
					"Customers",
					new[] { "CustomerID" },
					tracker,
					DbSettings.MSSQL
				);
			}
		}
		[TestMethod]
		public void InsertTest()
		{
			using (var ts = new TransactionScope())
			using (var connection = CreateConnection())
			{
				DBTools.InsertObject(connection, new { col1 = 10, col2 = 11, col3 = 12, col4 = 13, col5 = 1, col6 = 2 }, "test", DbSettings.MSSQL);
				ts.Complete();
			}
		}
	}
}
