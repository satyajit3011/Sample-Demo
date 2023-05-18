namespace Sample_Demo.DataStore
{
	using System.Collections.Generic;
	using Models;

	public interface IDataStore
	{
		List<OrderSalesServiceModel> GetServiceTagsFromDb( string serviceTags );
		List<OrderNumberModel> GetOrderNumbersFromDb( string orderNumbers );
	}
}
