using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory
{
	public class ServiceDiscoveryMessageHandler: DelegatingHandler
	{
		private readonly IServiceRegistry _serviceRegistry;

		public ServiceDiscoveryMessageHandler ( IServiceRegistry serviceRegistry )
		{
			_serviceRegistry = serviceRegistry;
		}

		protected override async Task<HttpResponseMessage> SendAsync (
										HttpRequestMessage request , CancellationToken cancellationToken )
		{
			var currentUri = request.RequestUri;

			//var serviceInstance = await _serviceRegistry.GetServiceInstanceAsync(currentUri.Host);
			//if ( serviceInstance == null )
			//{
			//	throw new NoServiceInstanceFoundException( currentUri.Host , null );
			//}

			//var uriBuilder = new UriBuilder(currentUri)
			//{
			//	Host = serviceInstance.Address,
			//	Port = serviceInstance.Port
			//};
			//request.RequestUri = uriBuilder.Uri;

			return await base.SendAsync( request , cancellationToken );
		}
	}

	public interface IServiceRegistry
	{
		Task GetServiceInstanceAsync ( string host );
	}

	[Serializable]
	public class NoServiceInstanceFoundException: Exception
	{
		public NoServiceInstanceFoundException ( )
		{
		}

		public NoServiceInstanceFoundException ( string message ) : base( message )
		{
		}

		public NoServiceInstanceFoundException ( string message , Exception innerException ) : base( message , innerException )
		{
		}

		protected NoServiceInstanceFoundException ( SerializationInfo info , StreamingContext context ) : base( info , context )
		{
		}
	}
}