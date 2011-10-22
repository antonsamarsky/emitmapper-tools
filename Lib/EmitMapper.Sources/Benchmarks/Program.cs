using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper;
using System.Diagnostics;
using EmitMapper.MappingConfiguration;
using System.Data.SqlClient;
using LightDataAccess;
using System.Data;
using System.Data.Common;

namespace Benchmarks
{
	public class Program
	{
		public class test
		{
			public int col1;
			public int col2;
			public int col3;
			public int col4;
			public int? col5;
			public int? col6;
		}

		public class BenchSource
		{
			public class Int1
			{
				public string str1 = "1";
				public string str2 = null;
				public int i = 10;
			}

			public class Int2
			{
				public Int1 i1 = new Int1();
				public Int1 i2 = new Int1();
				public Int1 i3 = new Int1();
				public Int1 i4 = new Int1();
				public Int1 i5 = new Int1();
				public Int1 i6 = new Int1();
				public Int1 i7 = new Int1();
			}

			public Int2 i1 = new Int2();
			public Int2 i2 = new Int2();
			public Int2 i3 = new Int2();
			public Int2 i4 = new Int2();
			public Int2 i5 = new Int2();
			public Int2 i6 = new Int2();
			public Int2 i7 = new Int2();
			public Int2 i8 = new Int2();

			public int n2;
			public long n3;
			public byte n4;
			public short n5;
			public uint n6;
			public int n7;
			public int n8;
			public int n9;

			public string s1 = "1";
			public string s2 = "2";
			public string s3 = "3";
			public string s4 = "4";
			public string s5 = "5";
			public string s6 = "6";
			public string s7 = "7";

		}

		public class BenchDestination
		{
			public class Int1
			{
				public string str1;
				public string str2;
				public int i;
			}

			public class Int2
			{
				public Int1 i1;
				public Int1 i2;
				public Int1 i3;
				public Int1 i4;
				public Int1 i5;
				public Int1 i6;
				public Int1 i7;
			}

			public Int2 i1 { get; set; }
			public Int2 i2 { get; set; }
			public Int2 i3 { get; set; }
			public Int2 i4 { get; set; }
			public Int2 i5 { get; set; }
			public Int2 i6 { get; set; }
			public Int2 i7 { get; set; }
			public Int2 i8 { get; set; }

			public long n2 = 2;
			public long n3 = 3;
			public long n4 = 4;
			public long n5 = 5;
			public long n6 = 6;
			public long n7 = 7;
			public long n8 = 8;
			public long n9 = 9;

			public string s1;
			public string s2;
			public string s3;
			public string s4;
			public string s5;
			public string s6;
			public string s7;
		}

		public class A1
		{
			public class Int
			{
				public string str2;
			}

			public string str1;
			public Int i;
		}

		public class B1
		{
			public class Int
			{
				public string str2 = "B1::Int::str2";
			}

			public string str1 = "B1::str1";
			public Int i = new Int();
		}

		#region Hndwritten mapper
		static BenchDestination.Int1 Map(BenchSource.Int1 s, BenchDestination.Int1 d)
		{
			if (s == null)
			{
				return null;
			}
			if (d == null)
			{
				d = new BenchDestination.Int1();
			}
			d.i = s.i;
			d.str1 = s.str1;
			d.str2 = s.str2;
			return d;
		}
		static BenchDestination.Int2 Map(BenchSource.Int2 s, BenchDestination.Int2 d)
		{
			if (s == null)
			{
				return null;
			}

			if (d == null)
			{
				d = new BenchDestination.Int2();
			}
			d.i1 = Map(s.i1, d.i1);
			d.i2 = Map(s.i2, d.i2);
			d.i3 = Map(s.i3, d.i3);
			d.i4 = Map(s.i4, d.i4);
			d.i5 = Map(s.i5, d.i5);
			d.i6 = Map(s.i6, d.i6);
			d.i7 = Map(s.i7, d.i7);

			return d;
		}
		static BenchDestination Map(BenchSource s, BenchDestination d)
		{
			if (s == null)
			{
				return null;
			}
			if (d == null)
			{
				d = new BenchDestination();
			}
			d.i1 = Map(s.i1, d.i1);
			d.i2 = Map(s.i2, d.i2);
			d.i3 = Map(s.i3, d.i3);
			d.i4 = Map(s.i4, d.i4);
			d.i5 = Map(s.i5, d.i5);
			d.i6 = Map(s.i6, d.i6);
			d.i7 = Map(s.i7, d.i7);
			d.i8 = Map(s.i8, d.i8);

			d.n2 = s.n2;
			d.n3 = s.n3;
			d.n4 = s.n4;
			d.n5 = s.n5;
			d.n6 = s.n6;
			d.n7 = s.n7;
			d.n8 = s.n8;
			d.n9 = s.n9;

			d.s1 = s.s1;
			d.s2 = s.s2;
			d.s3 = s.s3;
			d.s4 = s.s4;
			d.s5 = s.s5;
			d.s6 = s.s6;
			d.s7 = s.s7;

			return d;
		}
		#endregion

		static long BenchHandwrittenMapper(int mappingsCount)
		{
			var s = new BenchSource();
			var d = new BenchDestination();
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				d = Map(s, d);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long BenchEmitMapper(int mappingsCount)
		{
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<BenchSource, BenchDestination>();
			var s = new BenchSource();
			var d = new BenchDestination();

			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				mapper.Map(s, d);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long BenchAutoMapper(int mappingsCount)
		{
			AutoMapper.Mapper.CreateMap<BenchSource.Int1, BenchDestination.Int1>();
			AutoMapper.Mapper.CreateMap<BenchSource.Int2, BenchDestination.Int2>();
			AutoMapper.Mapper.CreateMap<BenchSource, BenchDestination>();

			var s = new BenchSource();
			var d = new BenchDestination();

			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				AutoMapper.Mapper.Map(s, d);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		public class A2
		{
			public string str1;
			public string str2;
			public string str3;
			public string str4;
			public string str5;
			public string str6;
			public string str7;
			public string str8;
			public string str9;

			public int n1;
			public int n2;
			public int n3;
			public int n4;
			public int n5;
			public int n6;
			public int n7;
		}

		public class B2
		{
			public string str1 = "str1";
			public string str2 = "str2";
			public string str3 = "str3";
			public string str4 = "str4";
			public string str5 = "str5";
			public string str6 = "str6";
			public string str7 = "str7";
			public string str8 = "str8";
			public string str9 = "str9";

			public int n1 = 1;
			public long n2 = 2;
			public short n3 = 3;
			public byte n4 = 4;
			public decimal? n5 = null;
			public float n6 = 6;
			public int n7 = 7;

		}

		static long BenchBLToolkit_Simple(int mappingsCount)
		{
			var s = new B2();
			var d = new A2();

			d = BLToolkit.Mapping.Map.ObjectToObject<A2>(s);

			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				d = BLToolkit.Mapping.Map.ObjectToObject<A2>(s);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long EmitMapper_Simple(int mappingsCount)
		{
			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>();
			var s = new B2();
			var d = new A2();

			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				mapper.Map(s, d);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long BenchBLToolkit_List(int mappingsCount)
		{
			var list = new List<B2>();

			for (var i = 0; i < mappingsCount; i++)
				list.Add(new B2());

			var sw = new Stopwatch();
			sw.Start();

			var dest = BLToolkit.Mapping.Map.ListToList<A2>(list);

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long EmitMapper_List(int mappingsCount)
		{
			var list = new List<B2>();

			for (var i = 0; i < mappingsCount; i++)
				list.Add(new B2());

			var sw = new Stopwatch();
			sw.Start();

			var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
				new DefaultMapConfig().NullSubstitution<decimal?, int>(state => 42)
				);
			var dest = new List<A2>(list.Count);

			foreach (var item in list)
				dest.Add(mapper.Map(item));

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

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


		static long BLToolkit_DB(int mappingsCount)
		{
			BLToolkit.Data.DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var db = new BLToolkit.Data.DbManager())
				{
					var list = db
						.SetCommand("SELECT * FROM Customers")
						.ExecuteList<Customer>();
				}
			}

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long BLToolkit_DB2(int mappingsCount)
		{
			BLToolkit.Data.DbManager.AddConnectionString("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1");

			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var db = new BLToolkit.Data.DbManager())
				{
					var list = db
						.SetCommand("SELECT * FROM test")
						.ExecuteList<test>();
				}
			}

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long EmitMapper_DB(int mappingsCount)
		{
			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var con = new SqlConnection("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
				using (var cmd = con.CreateCommand())
				{
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = "SELECT * FROM Customers";

					using (var reader = cmd.ExecuteReader())
					{
						var list = reader.ToObjects<Customer>("reader1").ToList();
					}
				}
			}

			sw.Stop();

			return sw.ElapsedMilliseconds;
		}

		static long BenchBLToolkit(int mappingsCount)
		{
			var s = new B1();
			var d = new A1();

			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				d = BLToolkit.Mapping.Map.ObjectToObject<A1>(s);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long BenchBLToolkit2(int mappingsCount)
		{
			var s = new BenchSource();
			var d = new BenchDestination();

			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < mappingsCount; ++i)
			{
				BLToolkit.Mapping.Map.ObjectToObject<BenchDestination>(s);
				d = BLToolkit.Mapping.Map.ObjectToObject<BenchDestination>(s);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static long EmitMapper_DB2(int mappingsCount)
		{
			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < mappingsCount; i++)
			{
				using (var con = new SqlConnection("Data Source=acer\\sqlserver;Initial Catalog=Northwind;Persist Security Info=True;User Id=sa;Password=1"))
				using (var cmd = con.CreateCommand())
				{
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.CommandText = "SELECT * FROM test";

					using (var reader = cmd.ExecuteReader())
					{
						var list = reader.ToObjects<test>().ToList();
					}
				}
			}

			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		static void Main(string[] args)
		{
			//int mappingsCount = 100000;
			int mappingsCount = 40000;

			//Console.WriteLine("Emit Mapper (simple): {0} milliseconds", EmitMapper_List(mappingsCount));
			//Console.WriteLine("BLToolkit (simple): {0} milliseconds", BenchBLToolkit_List(mappingsCount));

			Console.WriteLine("Emit Mapper (simple): {0} milliseconds", EmitMapper_DB2(mappingsCount));
			//Console.WriteLine("BLToolkit (simple): {0} milliseconds", BLToolkit_DB2(mappingsCount));
			//Console.WriteLine("Emit Mapper (simple): {0} milliseconds", EmitMapper_DB(mappingsCount));
			//Console.WriteLine("BLToolkit (simple): {0} milliseconds", BLToolkit_DB(mappingsCount));

			return;
			Console.WriteLine("Handwritten Mapper: {0} milliseconds", BenchHandwrittenMapper(mappingsCount));
			Console.WriteLine("Emit Mapper: {0} milliseconds", BenchEmitMapper(mappingsCount));
			Console.WriteLine("Auto Mapper: {0} milliseconds", BenchAutoMapper(mappingsCount));
		}
	}
}

/*
Handwritten Mapper: 475 milliseconds
Emit Mapper: 469 milliseconds
Auto Mapper: 205256 milliseconds
*/