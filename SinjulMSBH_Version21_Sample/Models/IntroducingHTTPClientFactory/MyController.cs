using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory
{
	public class MyController: Controller
	{
		private IHttpClientFactory _httpClientFactory;
		private IGitHubApi _gh;

		public MyController ( IHttpClientFactory httpClientFactory , IGitHubApi gh )
		{
			_httpClientFactory = httpClientFactory;
			_gh=gh;
		}

		public async Task<IActionResult> Index ( )
		{
			var client = _httpClientFactory.CreateClient();
			var result = client.GetStringAsync("http://myurl/");

			var defaultClient = _httpClientFactory.CreateClient();
			var gitHubClient = _httpClientFactory.CreateClient("github");

			var user = await _gh.GetUser("glennc");
			return View( user );
		}
	}
}