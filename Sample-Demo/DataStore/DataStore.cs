namespace Sample_Demo.DataStore
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using Controllers;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Models;
	using Oracle.ManagedDataAccess.Client;

	public class DataStore : IDataStore
	{
		private readonly SqlConnections _sqlConnections;
		private readonly ILogger<HomeController> _logger;
		public DataStore( IConfiguration configuration, ILogger<HomeController> logger )
		{
			_sqlConnections = new SqlConnections( )
			{
				EngReadonly = configuration.GetValue<string>( "SqlConnections:User" ),
				EngReadonlyUserPassword = configuration.GetValue<string>( "SqlConnections:Password" )
			};
			_logger = logger;
		}

		public List<OrderNumberModel> GetOrderNumbersFromDb( string orderNumbers )
		{
			List<OrderNumberModel> models = new List<OrderNumberModel>( );
			OrderNumberModel orderNumber = null;
			try
			{
				orderNumbers = Helper.ConvertInputToSingleQuotes( orderNumbers );
				string[ ] ordersArray = orderNumbers.Split( ',' );

				foreach ( string orderNum in ordersArray )
				{
					string query = @"select * from df.dee_notification where event_type = 'ENTITLEMENT' and
									  pk_id IN (select pk_id from ee.entitlement_dnt where order_number = " + orderNum + ");";
					//string query = @"SELECT TicketId, AgentId FROM [AtHelp].[NiceInContactSessions] WHERE Id in (" + orderNumbers + ")";

					orderNumber = new OrderNumberModel( )
					{
						OrderNumber = orderNum.Replace("'","")
					};

					using ( SqlConnection voiceEngDbConnection = new SqlConnection( _sqlConnections.EngReadonly + _sqlConnections.EngReadonlyUserPassword ) )
					using ( SqlCommand com = new SqlCommand( query, voiceEngDbConnection ) )
					{
						voiceEngDbConnection.Open( );
						using ( SqlDataReader rdr = com.ExecuteReader( ) )
						{
							if ( rdr.HasRows )
							{
								while ( rdr.HasRows && rdr.Read( ) )
								{
									voiceEngDbConnection.Close( );
									try
									{
										orderNumber.Status = "UPDATE";
										string updateQuery = @"update df.dee_notification set status = 'NEW' where event_type = 'ENTITLEMENT' and
															pk_id IN (select pk_id from ee.entitlement_dnt where order_number = " + orderNum + ");";
										using ( SqlCommand com1 = new SqlCommand( updateQuery, voiceEngDbConnection ) )
										{
											try
											{
												voiceEngDbConnection.Open( );
												com1.ExecuteNonQuery( );
												orderNumber.StatusText = "Success";
											}
											catch ( Exception e )
											{
												orderNumber.StatusText = e.Message;
											}
										}
										models.Add( orderNumber );
										voiceEngDbConnection.Close( );
									}
									catch ( Exception ex )
									{
										_logger.LogError( ex, "Exception parsing service tags result from DB." );
										orderNumber.StatusText = ex.Message;
									}
								}
							}
							else
							{
								try
								{
									orderNumber.Status = "NEW";
									string insertQuery = @"insert into df.dee_notification();";
									using ( SqlCommand com1 = new SqlCommand( insertQuery, voiceEngDbConnection ) )
									{
										try
										{
											voiceEngDbConnection.Open( );
											com1.ExecuteNonQuery( );
											orderNumber.StatusText = "Success";
										}
										catch ( Exception e )
										{
											orderNumber.StatusText = e.Message;
										}
									}
									models.Add( orderNumber );
									voiceEngDbConnection.Close( );
								}
								catch ( Exception ex )
								{
									_logger.LogError( ex, "Exception parsing service tags result from DB." );
									orderNumber.StatusText = ex.Message;
								}
							}
						}
					}
				}
			}
			catch ( Exception e )
			{
				_logger.LogError( e, "Exception reading order numbers from DB." );
				throw;
			}

			//models = new List<OrderNumberModel>
			//{
			//	new OrderNumberModel( ) { OrderNumber = 123456, Status = "Update", StatusText = "Success"},
			//	new OrderNumberModel( ) { OrderNumber = 123456, Status = "Error", StatusText = "Error"},
			//	new OrderNumberModel( ) { OrderNumber = 123456, Status = "New", StatusText = "Success"},
			//};
			return models;
		}

		public List<OrderNumberModel> GetOrderNumbersFromOracleDb( string orderNumbers )
		{
			List<OrderNumberModel> models = new List<OrderNumberModel>( );
			try
			{
				orderNumbers = Helper.ConvertInputToSingleQuotes( orderNumbers );
				string[ ] ordersArray = orderNumbers.Split( ',' );

				foreach ( string orderNum in ordersArray )
				{
					string query = @"select * from df.dee_notification where event_type = 'ENTITLEMENT' and
									  pk_id IN (select pk_id from ee.entitlement_dnt where order_number = " + orderNum + ");";
					//string query = @"SELECT TicketId, AgentId FROM [AtHelp].[NiceInContactSessions] WHERE Id in (" + orderNumbers + ")";

					OrderNumberModel orderNumber = new OrderNumberModel( )
					{
						OrderNumber = orderNum.Replace( "'", "" )
					};

					using ( OracleConnection voiceEngDbConnection = new OracleConnection( _sqlConnections.EngReadonly + _sqlConnections.EngReadonlyUserPassword ) )
					using ( OracleCommand com = new OracleCommand( query, voiceEngDbConnection ) )
					{
						voiceEngDbConnection.Open( );
						using ( OracleDataReader rdr = com.ExecuteReader( ) )
						{
							if ( rdr.HasRows )
							{
								while ( rdr.HasRows && rdr.Read( ) )
								{
									voiceEngDbConnection.Close( );
									try
									{
										orderNumber.Status = "UPDATE";
										string updateQuery = @"update df.dee_notification set status = 'NEW' where event_type = 'ENTITLEMENT' and
															pk_id IN (select pk_id from ee.entitlement_dnt where order_number = " + orderNum + ");";
										using ( OracleCommand com1 = new OracleCommand( updateQuery, voiceEngDbConnection ) )
										{
											try
											{
												voiceEngDbConnection.Open( );
												com1.ExecuteNonQuery( );
												orderNumber.StatusText = "Success";
											}
											catch ( Exception e )
											{
												orderNumber.StatusText = e.Message;
											}
										}
										models.Add( orderNumber );
										voiceEngDbConnection.Close( );
									}
									catch ( Exception ex )
									{
										_logger.LogError( ex, "Exception parsing service tags result from DB." );
										orderNumber.StatusText = ex.Message;
									}
								}
							}
							else
							{
								try
								{
									orderNumber.Status = "NEW";
									string insertQuery = @"insert into df.dee_notification();";
									using ( OracleCommand com1 = new OracleCommand( insertQuery, voiceEngDbConnection ) )
									{
										try
										{
											voiceEngDbConnection.Open( );
											com1.ExecuteNonQuery( );
											orderNumber.StatusText = "Success";
										}
										catch ( Exception e )
										{
											orderNumber.StatusText = e.Message;
										}
									}
									models.Add( orderNumber );
									voiceEngDbConnection.Close( );
								}
								catch ( Exception ex )
								{
									_logger.LogError( ex, "Exception parsing service tags result from DB." );
									orderNumber.StatusText = ex.Message;
								}
							}
						}
					}
				}
			}
			catch ( Exception e )
			{
				_logger.LogError( e, "Exception reading order numbers from DB." );
				throw;
			}

			//models = new List<OrderNumberModel>
			//{
			//	new OrderNumberModel( ) { OrderNumber = 123456, Status = "Update", StatusText = "Success"},
			//	new OrderNumberModel( ) { OrderNumber = 123456, Status = "Error", StatusText = "Error"},
			//	new OrderNumberModel( ) { OrderNumber = 123456, Status = "New", StatusText = "Success"},
			//};
			return models;
		}

		public List<OrderSalesServiceModel> GetServiceTagsFromDb( string serviceTags )
		{
			List<OrderSalesServiceModel> models = new List<OrderSalesServiceModel>( );
			try
			{
				serviceTags = Helper.ConvertInputToSingleQuotes( serviceTags );
				//string query = @"SELECT service_Tag, cust_sales_order_no FROM LKM.lk_md_oem_product_keys WHERE service_Tag in (" + serviceTags + ")";
				string query = @"SELECT TicketId, AgentId FROM [AtHelp].[NiceInContactSessions] WHERE Id in (" + serviceTags + ")";
				using ( SqlConnection voiceEngDbConnection = new SqlConnection( _sqlConnections.EngReadonly + _sqlConnections.EngReadonlyUserPassword ) )
				using ( SqlCommand com = new SqlCommand( query, voiceEngDbConnection ) )
				{
					voiceEngDbConnection.Open( );
					using ( SqlDataReader rdr = com.ExecuteReader( ) )
					{
						while ( rdr.HasRows && rdr.Read( ) )
						{
							try
							{
								OrderSalesServiceModel salesServiceModel = new OrderSalesServiceModel( )
								{
									ServiceTag = rdr.IsDBNull( 0 ) ? -1 : rdr.GetInt32( 0 ),
									SalesOrderNumber = rdr.IsDBNull( 1 ) ? -1 : rdr.GetInt32( 1 )
								};
								models.Add( salesServiceModel );
							}
							catch ( Exception ex )
							{
								_logger.LogError( ex, "Exception parsing service tags result from DB." );
							}
						}
					}
				}
			}
			catch ( Exception e )
			{
				_logger.LogError( e, "Exception reading service tags from DB." );
			}

			models = new List<OrderSalesServiceModel>
			{
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 }
			};
			return models;
		}

		public List<OrderSalesServiceModel> GetServiceTagsFromOracleDb( string serviceTags )
		{
			List<OrderSalesServiceModel> models = new List<OrderSalesServiceModel>( );
			try
			{
				serviceTags = Helper.ConvertInputToSingleQuotes( serviceTags );
				string query = @"SELECT service_Tag, cust_sales_order_no FROM LKM.lk_md_oem_product_keys WHERE service_Tag in (" + serviceTags + ")";
				using ( OracleConnection voiceEngDbConnection = new OracleConnection( _sqlConnections.EngReadonly + _sqlConnections.EngReadonlyUserPassword ) )
				using ( OracleCommand com = new OracleCommand( query, voiceEngDbConnection ) )
				{
					voiceEngDbConnection.Open( );
					using ( OracleDataReader rdr = com.ExecuteReader( ) )
					{
						while ( rdr.HasRows && rdr.Read( ) )
						{
							try
							{
								OrderSalesServiceModel salesServiceModel = new OrderSalesServiceModel( )
								{
									ServiceTag = rdr.IsDBNull( 0 ) ? -1 : rdr.GetInt32( 0 ),
									SalesOrderNumber = rdr.IsDBNull( 1 ) ? -1 : rdr.GetInt32( 1 )
								};
								models.Add( salesServiceModel );
							}
							catch ( Exception ex )
							{
								_logger.LogError( ex, "Exception parsing service tags result from DB." );
							}
						}
					}
				}
			}
			catch ( Exception e )
			{
				_logger.LogError( e, "Exception reading service tags from DB." );
			}

			models = new List<OrderSalesServiceModel>
			{
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 },
				new OrderSalesServiceModel( ) { ServiceTag = 123456, SalesOrderNumber = 87654 }
			};
			return models;
		}
	}
}
