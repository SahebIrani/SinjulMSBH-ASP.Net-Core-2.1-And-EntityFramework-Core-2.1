using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory
{
	public class LoggingHttpMessageHandler: DelegatingHandler
	{
		private ILogger _logger;

		public LoggingHttpMessageHandler ( ILogger<LoggingHttpMessageHandler> logger )
		{
			_logger = logger;
		}

		protected async override Task<HttpResponseMessage> SendAsync (
										HttpRequestMessage request , CancellationToken cancellationToken )
		{
			var stopwatch = Stopwatch.StartNew();

			_logger.LogInformation( "Sending HTTP request {HttpMethod} {Uri}" , request.Method , request.RequestUri );

			var response = await base.SendAsync(request, cancellationToken);

			_logger.LogInformation( "Received HTTP response after {ElapsedMilliseconds}ms - {StatusCode}" ,
										Stopwatch.StartNew( ).ElapsedMilliseconds , response.StatusCode );

			return response;
		}
	}
}