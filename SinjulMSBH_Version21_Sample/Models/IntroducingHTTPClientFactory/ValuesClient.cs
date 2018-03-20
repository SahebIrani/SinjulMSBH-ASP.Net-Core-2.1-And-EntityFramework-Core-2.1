using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory
{
	public class ValuesClient
	{
		//In this case we are encapsulating the HttpClient,
		//but it could be exposed if that is desired.
		private HttpClient _client;

		private ILogger<ValuesClient> _logger;

		public ValuesClient ( HttpClient client , ILogger<ValuesClient> logger )
		{
			_client = client;
			_logger = logger;
		}

		public async Task<IEnumerable<string>> GetValues ( )
		{
			try
			{
				//Here we are making the assumption that our HttpClient instance
				//has already had its base address set.
				var response = await _client.GetAsync("api/values");
				response.EnsureSuccessStatusCode( );

				//NOTE: The Content.ReadAsAsync method comes from the Microsoft.AspNet.WebApi.Client package. You will need to add that to your application if you want to use it.
				return await response.Content.ReadAsAsync<IEnumerable<string>>( );
			}
			catch ( HttpRequestException ex )
			{
				_logger.LogError( $"An error occured connecting to values API {ex.ToString( )}" );
				return Enumerable.Empty<string>( );
			}
		}
	}
}