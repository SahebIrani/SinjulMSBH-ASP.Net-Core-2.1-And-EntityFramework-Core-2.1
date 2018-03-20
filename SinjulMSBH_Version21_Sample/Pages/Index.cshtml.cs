using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory;

namespace SinjulMSBH_Version21_Sample.Pages
{
	public class IndexModel: PageModel
	{
		//https://msdnshared.blob.core.windows.net/media/2018/02/DiagnosticLogs.png
		//		An incoming request in to localhost:5001 in this case this is the browser navigating to my Razor Pages page.
		//MVC selecting a handler for the request , the OnGetAsync method of my PageModel.
		//The beginning of an outgoing HTTP request, this marks the start of the outgoing pipeline that we will discuss in the next section.
		//We send a HTTP request with the given verb.
		//Recieve the request back in 439.6606 ms, with a status of OK.
		//End the outgoing HTTP pipeline.
		//End and return from our handler.

		//If you set the LogLevel to at least Debug then we will also log header information. In the following screenshot I added an accept header to my request, and you can see the response headers:
		//https://msdnshared.blob.core.windows.net/media/2018/02/DebugLogs.png

		//public async Task OnGetAsync ( [FromServices]ValuesClient client )
		//{
		//	var values = await client.GetValues();
		//}

		//private ValuesClient _valuesClient;
		//private GitHubService _ghService;

		//private ValuesService _valuesService;

		public IndexModel ( /*ValuesClient valuesClient *//* GitHubService ghService*/ /*, ValuesService valuesService */)
		{
			//_valuesClient = valuesClient;
			//_ghService=ghService;
			//_valuesService=valuesService;
		}

		//public IEnumerable<string> Values;

		public async Task OnGetAsync ( )
		{
			//var valuesme = await _ghService.GetValues();
			//var result = await _ghService.Client.GetStringAsync("/orgs/octokit/repos");
			//Values = await _valuesClient.GetValues( );
		}
	}
}