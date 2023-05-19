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
			try
			{
				orderNumbers = Helper.ConvertInputToSingleQuotes( orderNumbers );
				string[ ] ordersArray = orderNumbers.Split( ',' );

				foreach ( string orderNum in ordersArray )
				{
					OrderNumberModel orderNumber = new OrderNumberModel( )
					{
						OrderNumber = orderNum.Replace( "'", "" )
					};

					string query = @"select * from df.dee_notification where event_type = 'ENTITLEMENT' and
									  pk_id IN (select pk_id from ee.entitlement_dnt where order_number = " + orderNum + ");";

					DbOperation dbOperation = IsOrderNumberPresent( query );

					switch ( dbOperation )
					{
						case DbOperation.Update:
							string updateQuery = @"update df.dee_notification set status = 'NEW' where event_type = 'ENTITLEMENT' and
													pk_id IN (select pk_id from ee.entitlement_dnt where order_number = " + orderNum + ");";
							OrderNumberModel updateResult = DbCommit( DbOperation.Update, updateQuery, orderNumber );
							models.Add(updateResult);
							break;
						case DbOperation.Insert:
							string insertQuery = @"insert into df.dee_notification();";
							OrderNumberModel insertResult = DbCommit( DbOperation.Update, insertQuery, orderNumber );
							models.Add(insertResult);
							break;
						case DbOperation.None:
							break;
						default:
							throw new ArgumentOutOfRangeException( );
					}
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( e );
				throw;
			}
			return models;
		}

		private DbOperation IsOrderNumberPresent( string query )
		{
			using OracleConnection dbConnection = new OracleConnection( _sqlConnections.EngReadonly + _sqlConnections.EngReadonlyUserPassword );
			using OracleCommand com = new OracleCommand( query, dbConnection );
			try
			{
				dbConnection.Open( );
				using OracleDataReader rdr = com.ExecuteReader( );
				if ( rdr.HasRows && rdr.Read( ) )
				{
					return DbOperation.Update;
				}
				else
				{
					return DbOperation.Insert;
				}
			}
			catch ( Exception ex )
			{
				_logger.LogError( ex, "Exception in checking order number is present in DB." );
			}
			finally
			{
				dbConnection.Close( );
			}

			return DbOperation.None;
		}

		private OrderNumberModel DbCommit( DbOperation dbOperation ,string query, OrderNumberModel orderNumberModel )
		{
			using OracleConnection dbConnection = new OracleConnection( _sqlConnections.EngReadonly + _sqlConnections.EngReadonlyUserPassword );
			using OracleCommand com = new OracleCommand( query, dbConnection );
			try
			{
				orderNumberModel.Status = dbOperation == DbOperation.Update ? "UPDATE" : "NEW";
				dbConnection.Open( );
				com.ExecuteNonQuery( );
				orderNumberModel.StatusText = "Success";
			}
			catch ( Exception ex )
			{
				_logger.LogError( ex, "Exception in DB commit method." );
				orderNumberModel.StatusText = ex.Message;
			}
			finally
			{
				dbConnection.Close( );
			}

			return orderNumberModel;
		}

		public List<OrderSalesServiceModel> GetServiceTagsFromDb( string serviceTags )
		{
			throw new NotImplementedException( );
		}

		private enum DbOperation
		{
			None = 0,
			Update = 1,
			Insert = 2,
		}
	}
}
