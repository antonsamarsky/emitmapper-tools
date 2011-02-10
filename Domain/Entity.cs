using System;
using MappingDefinition;

namespace Domain
{
	[DataContainer(TableName = "Order")]
	public class Entity
	{
		[DataMember(FieldName = "order_id")]
		public Guid Id { get; set; }

		[DataMember(FieldName = "order_name")]
		public string Name { get; set; }

		[DataMember(FieldName = "order_number")]
		[DataMember(FieldName = "order_number_2")]
		public int Number { get; set; }

		[DataMember(FieldName = "order_price")]
		public decimal Price { get; set; }

		public string UserName { get; set; }
	}
}