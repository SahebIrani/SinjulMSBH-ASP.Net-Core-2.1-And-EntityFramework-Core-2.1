using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinjulMSBH_Version21_Sample.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SinjulMSBH_Version21_Sample.Models.IntroducingHTTPClientFactory;
using Refit;
using SinjulMSBH_Version21_Sample.Models.SignalR;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SinjulMSBH_Version21_Sample
{
	public class Startup
	{
		public Startup ( IConfiguration configuration )
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices ( IServiceCollection services )
		{
			services.Configure<CookiePolicyOptions>( options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			} );

			//services.AddDbContext<ApplicationDbContext>( options =>
			//     options.UseSqlServer(
			//	   Configuration.GetConnectionString( "DefaultConnection" ) ) );

			//services.AddDbContext<ApplicationDbContext>( options =>
			//	  options.UseInMemoryDatabase( "SinjulMSBH" ) );

			//services.AddDbContext<ApplicationDbContext>( options =>
			//options.UseSqlite(
			//Configuration.GetConnectionString( "DefaultConnection" ) ,
			//sqlOptions => sqlOptions.MigrationsAssembly( "WebApplication1" ) ) );

			services.AddDbContextPool<ApplicationDbContext>( options =>
				  //options.UseLazyLoadingProxies( )
				  options.UseSqlServer( Configuration.GetConnectionString( "DefaultConnection" ) )
			//sqlOptions => sqlOptions.MigrationsAssembly( "SinjulMSBH_Version21_Sample" ) )
			//, 128
			);

			#region Startup.authschemes.cs And Startup.options1.cs

			services.AddAuthentication( "Dynamic" )
			   .AddVirtualScheme( "Dynamic" , "Dynamic" , o =>
			   {
				   o.DefaultSelector = ctx =>
				   {
					   return ctx.Request.Path.StartsWithSegments( "/api" ) ?
						 JwtBearerDefaults.AuthenticationScheme :
						 CookieAuthenticationDefaults.AuthenticationScheme;
				   };
			   } )
			   .AddCookie( )
			   //.AddBeare( )
			   ;

			//services.Configure<MyOptions , MyService , MyService2>( ( options , service , service2 ) =>
			//{
			//	options.Value = service.DoSomething( service2.DoSomethingElse( ) )
			//} );

			//public class MySetup: IConfigureOptions<Options1>, IConfigureOptions<Options2>
			//{
			//	void Configure ( Options1 o ) { … }
			//	void Configure ( Options2 o ) { … }
			//}

			#endregion Startup.authschemes.cs And Startup.options1.cs

			#region IntroducingHTTPClientFactory

			//services.AddHttpClient<GitHubService>( )
			// .AddPollyHandler( policy =>
			// {
			//	 policy.Retry( );
			// } );

			services.AddHttpClient<ValuesClient>( client => client.BaseAddress = new Uri( Configuration[ "values:uri" ] ) );
			//Code to register IServiceRegistry would go here.
			//Handlers need to be transient.
			//services.AddTransient<ServiceDiscoveryMessageHandler>( );
			//services.AddHttpClient<ValuesClient>( client => client.BaseAddress = new Uri( Configuration[ "ValuesServiceUri" ] ) )
			//	  .AddHttpMessageHandler<ServiceDiscoveryMessageHandler>( );

			//services.AddHttpClient( "github" , c =>
			//{
			//	c.BaseAddress = new Uri( "https://api.github.com/" );

			//	c.DefaultRequestHeaders.Add( "Accept" , "application/vnd.github.v3+json" ); // Github API versioning
			//	c.DefaultRequestHeaders.Add( "User-Agent" , "HttpClientFactory-Sample" ); // Github requires a user-agent
			//} );
			//OR
			//services.AddHttpClient<GitHubService>( );

			////services.AddHttpClient( "github" , c =>
			////{
			////	c.BaseAddress = new Uri( "https://api.github.com/" );

			////	c.DefaultRequestHeaders.Add( "Accept" , "application/vnd.github.v3+json" ); // Github API versioning
			////	c.DefaultRequestHeaders.Add( "User-Agent" , "HttpClientFactory-Sample" ); // Github requires a user-agent
			////} )
			////.AddTypedClient( c => Refit.RestService.For<IGitHubApi>( "https://api.github.com" ) );
			////services.AddHttpClient( );

			//var gitHubApi = RestService.For<IGitHubApi>("https://api.github.com");
			////var octocat = await gitHubApi.GetUser("octocat");
			//var vu = Configuration[ "values:uri" ];

			//services.AddHttpClient<ValuesService>( client => client.BaseAddress = new Uri( Configuration[ "values:uri" ] ) );

			#endregion IntroducingHTTPClientFactory

			#region HTTPSAndHSTS

			services.AddHttpsRedirection( options => options.HttpsPort = 5002 );

			services.AddHsts( options =>
			{
				options.MaxAge = TimeSpan.FromDays( 100 );
				options.IncludeSubDomains = true;
				options.Preload = true;
			} );

			#endregion HTTPSAndHSTS

			#region ASP.NET Core 2.1.0-preview1: Introducing compatibility version in MVC

			services.AddMvc( ).SetCompatibilityVersion( CompatibilityVersion.Version_2_1 ); // Give me the 2.1 behaviors

			services.AddMvc( )
			   .AddJsonOptions( options =>
				    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore )
			   .AddJsonOptions( options =>
				   options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All )
			   .SetCompatibilityVersion( CompatibilityVersion.Version_2_1 ) // Give me all of the 2.1 behaviors
			   .AddMvcOptions( options =>
			   {
				   options.AllowCombiningAuthorizeFilters = false; // don't combine authorize filters (keep 2.0 behavior)
			   }
			   //.AddRazorPagesOptions( options =>
			   //{
			   // options.Conventions.AuthorizeFolder( "Identity/Account/Manage" );
			   // options.Conventions.AuthorizePage( "Identity/Account/Logout" );
			   //}
			   );

			#endregion ASP.NET Core 2.1.0-preview1: Introducing compatibility version in MVC

			#region ASP.NET Core 2.1.0-preview1: Improvements for building Web APIs

			services.Configure<ApiBehaviorOptions>( options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var problemDetails = new ValidationProblemDetails(context.ModelState)
					{
						Instance = context.HttpContext.Request.Path,
						Status = StatusCodes.Status400BadRequest,
						Type = "https://asp.net/core",
						Detail = "Please refer to the errors property for additional details."
					};
					return new BadRequestObjectResult( problemDetails )
					{
						ContentTypes = { "application/problem+json" , "application/problem+xml" }
					};
				};
			} );

			#endregion ASP.NET Core 2.1.0-preview1: Improvements for building Web APIs

			#region ASP.NET Core 2.1.0-preview1: Getting started with SignalR

			services.AddSignalR( );

			#endregion ASP.NET Core 2.1.0-preview1: Getting started with SignalR

			#region ASP.NET Core 2.1.0-preview1: Introducing Identity UI as a library

			services.AddIdentity<ApplicationUser , IdentityRole /*ApplicationRole*/>( options => options.Stores.MaxLengthForKeys = 128 )
			 .AddEntityFrameworkStores<ApplicationDbContext>( )
			 .AddDefaultUI( )
			 .AddDefaultTokenProviders( );

			services.AddTransient<IEmailSender , EmailSender>( );

			#endregion ASP.NET Core 2.1.0-preview1: Introducing Identity UI as a library
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure ( IApplicationBuilder app , IHostingEnvironment env )
		{
			if ( env.IsDevelopment( ) )
			{
				app.UseBrowserLink( );
				app.UseDeveloperExceptionPage( );
				app.UseDatabaseErrorPage( );
			}
			else
			{
				app.UseExceptionHandler( "/Error" );
				app.UseHsts( );
			}

			app.UseHttpsRedirection( );
			app.UseStaticFiles( );
			app.UseCookiePolicy( );

			app.UseAuthentication( );

			app.UseMvcWithDefaultRoute( );

			app.UseSignalR( routes =>
			{
				routes.MapHub<ChatHub>( "/hubs/chat" );
			} );

			//app.Run( async ( context ) =>
			//{
			//	var processName = Process.GetCurrentProcess().ProcessName;
			//	await context.Response.WriteAsync( $"Hello World from {processName}" );
			//} );
		}
	}
}