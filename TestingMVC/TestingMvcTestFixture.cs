using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestingMVC
{
	public class TestingMvcTestFixture<TStartup>: WebApplicationTestFixture<TStartup> where TStartup : class
	{
		public TestingMvcTestFixture ( )
		    : base(/*"src/TestingMvc"*/ ) { }

		public HttpClient Client { get; internal set; }
	}
}