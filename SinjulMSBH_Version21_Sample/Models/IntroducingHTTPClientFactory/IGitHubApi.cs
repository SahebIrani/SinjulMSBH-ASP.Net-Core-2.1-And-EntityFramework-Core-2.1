using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory
{
	public interface IGitHubApi
	{
		[Get( "/users/{user}" )]
		Task<User> GetUser ( string user );
	}

	public class User
	{
		public string SinjulMSBH { get; set; }
	}

	public class GitHubApi: IGitHubApi
	{
		public Task<User> GetUser ( string user )
		{
			var me = new User{ SinjulMSBH=user};
			return GetUser( me.SinjulMSBH );
		}
	}
}