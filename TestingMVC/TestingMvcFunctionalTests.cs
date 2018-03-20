using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

//using SinjulMSBH_Version21_Sample;

namespace TestingMVC
{
	public class TestingMvcFunctionalTests: IClassFixture<TestingMvcTestFixture<Startup>>
	{
		public TestingMvcFunctionalTests ( TestingMvcTestFixture<Startup> fixture )
		{
			Client = fixture.Client;
			Client.BaseAddress = new Uri( "https://localhost" );
		}

		public HttpClient Client { get; }

		[Fact]
		public async Task GetHomePage ( )
		{
			// Arrange & Act
			var response = await Client.GetAsync("/");

			// Assert
			Assert.Equal( HttpStatusCode.OK , response.StatusCode );
		}
	}
}