using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample_Demo.Models;

namespace Sample_Demo.Controllers
{
	using AspNetCoreHero.ToastNotification.Abstractions;
	using DataStore;
	using Microsoft.Extensions.Configuration;

	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IDataStore _dataStore;
		private List<OrderSalesServiceModel> _orderSalesServiceModels;
		private readonly INotyfService _toastNotification;

		public HomeController( IConfiguration configuration, ILogger<HomeController> logger, INotyfService toastNotification )
		{
			_logger = logger;
			_dataStore = new DataStore( configuration, logger );
			_toastNotification = toastNotification;
		}

		public IActionResult Index( )
		{
			_orderSalesServiceModels = new List<OrderSalesServiceModel>( );
			return View( _orderSalesServiceModels );
		}

		[HttpGet]
		public JsonResult GetServiceTagsData( string serviceTags )
		{
			List<OrderSalesServiceModel> models = _dataStore.GetServiceTagsFromDb( serviceTags );
			return Json( models );
		}

		public IActionResult OrderNumber( )
		{
			return View( new List<OrderNumberModel>() );
		}

		[HttpGet]
		public JsonResult GetOrderNumbersData( string orderNumbers )
		{
			List<OrderNumberModel> models = new List<OrderNumberModel>();
			try
			{
				if ( string.IsNullOrEmpty( orderNumbers ) )
				{
					_toastNotification.Information( "Please enter order number" );
				}
				else
				{
					models = _dataStore.GetOrderNumbersFromDb( orderNumbers );
				}
				
			}
			catch ( Exception e )
			{
				_toastNotification.Error( e.Message );
			}

			return Json( models );
		}

		[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
		public IActionResult Error( )
		{
			return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
		}
	}
}
