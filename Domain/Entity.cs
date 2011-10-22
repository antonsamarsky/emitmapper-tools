using System;
using MappingDefinitions;

namespace Domain
{
	[DataContainer(TableName = "Order")]
	public class Entity
	{
		[DataMember(FieldName = "order_id", FieldType = typeof(string))]
		public Guid Id { get; set; }

		[DataMember(FieldName = "order_name")]
		public string Name { get; set; }

		[DataMember(FieldName = "order_number", FieldType = typeof(double))]
		[DataMember(FieldName = "order_number_2", FieldType = typeof(double))]
		public int Number { get; set; }

		[DataMember(FieldName = "order_price")]
		public decimal Price { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember(FieldType = typeof(string))]
		public DateTime Time { get; set; }
	}
}