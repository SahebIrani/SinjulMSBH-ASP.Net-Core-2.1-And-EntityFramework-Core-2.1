//// Copyright (c) .NET Foundation. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;

//using SinjulMSBH_Version21_Sample.Data;

//namespace SinjulMSBH_Version21_Sample.Areas.Identity.Pages.Account
//{
//	//[AllowAnonymous]
//	//[IdentityDfaultUI( typeof( RegisterModel<> ) )]
//	public abstract class RegisterModel: PageModel
//	{
//		[BindProperty]
//		public InputModel Input { get; set; }

//		public string ReturnUrl { get; set; }

//		public class InputModel
//		{
//			[Required]
//			[DataType( DataType.Text )]
//			[Display( Name = "Full name" )]
//			public string Name { get; set; }

//			[Required]
//			[Range( 0 , 199 , ErrorMessage = "Age must be between 0 and 199 years" )]
//			[Display( Name = "Age" )]
//			public string Age { get; set; }

//			[Required]
//			[EmailAddress]
//			[Display( Name = "Email" )]
//			public string Email { get; set; }

//			[Required]
//			[StringLength( 100 , ErrorMessage = "The {0} must be at least {2} and at max {1} characters long." , MinimumLength = 6 )]
//			[DataType( DataType.Password )]
//			[Display( Name = "Password" )]
//			public string Password { get; set; }

//			[DataType( DataType.Password )]
//			[Display( Name = "Confirm password" )]
//			[Compare( "Password" , ErrorMessage = "The password and confirmation password do not match." )]
//			public string ConfirmPassword { get; set; }
//		}

//		public virtual void OnGet ( string returnUrl = null ) => throw new NotImplementedException( );

//		public virtual Task<IActionResult> OnPostAsync ( string returnUrl = null ) => throw new NotImplementedException( );
//	}

//	internal class RegisterModel<TUser>: RegisterModel where TUser : ApplicationUser, new()
//	{
//		//private readonly SignInManager<TUser> _signInManager;
//		private readonly SignInManager<ApplicationUser> _signInManager2;

//		//private readonly UserManager<TUser> _userManager;
//		private readonly UserManager<ApplicationUser> _userManager2;

//		private readonly ILogger<RegisterModel> _logger;
//		private readonly IEmailSender _emailSender;

//		public RegisterModel (
//		    //UserManager<TUser> userManager ,
//		    //SignInManager<TUser> signInManager ,
//		    ILogger<RegisterModel> logger ,
//		    IEmailSender emailSender ,
//		    UserManager<ApplicationUser> userManager2 ,
//		    SignInManager<ApplicationUser> signInManager2 )
//		{
//			//_userManager = userManager;
//			//_signInManager = signInManager;
//			_logger = logger;
//			_emailSender = emailSender;
//			_userManager2=userManager2;
//			_signInManager2=signInManager2;
//		}

//		public override void OnGet ( string returnUrl = null )
//		{
//			ReturnUrl = returnUrl;
//		}

//		public override async Task<IActionResult> OnPostAsync ( string returnUrl = null )
//		{
//			returnUrl = returnUrl ?? Url.Content( "~/" );
//			if ( ModelState.IsValid )
//			{
//				var userORG = new TUser { UserName = Input.Email, Email = Input.Email };
//				var user = new ApplicationUser()
//				{
//					Name = Input.Name,
//					Age = Input.Age,
//					UserName = Input.Email,
//					Email = Input.Email
//				};
//				var result = await _userManager2.CreateAsync(user, Input.Password);
//				if ( result.Succeeded )
//				{
//					_logger.LogInformation( "User created a new account with password." );

//					var code = await _userManager2.GenerateEmailConfirmationTokenAsync(user);
//					var callbackUrl = Url.Page(
//						"/Account/ConfirmEmail",
//						pageHandler: null,
//						values: new { userId = user.Id, code = code },
//						protocol: Request.Scheme);

//					await _emailSender.SendEmailAsync( Input.Email , "Confirm your email" ,
//					    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode( callbackUrl )}'>clicking here</a>." );

//					await _signInManager2.SignInAsync( user , isPersistent: false );
//					return LocalRedirect( returnUrl );
//				}
//				foreach ( var error in result.Errors )
//				{
//					ModelState.AddModelError( string.Empty , error.Description );
//				}
//			}

//			// If we got this far, something failed, redisplay form
//			return Page( );
//		}
//	}
//}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SinjulMSBH_Version21_Sample.Data;

namespace SinjulMSBH_Version21_Sample.Areas.Identity.Pages.Account
{
	public class RegisterModel: PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public RegisterModel (
		    UserManager<ApplicationUser> userManager ,
		    SignInManager<ApplicationUser> signInManager ,
		    ILogger<RegisterModel> logger ,
		    IEmailSender emailSender )
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display( Name = "Email" )]
			public string Email { get; set; }

			[Required]
			[StringLength( 100 , ErrorMessage = "The {0} must be at least {2} and at max {1} characters long." , MinimumLength = 6 )]
			[DataType( DataType.Password )]
			[Display( Name = "Password" )]
			public string Password { get; set; }

			[DataType( DataType.Password )]
			[Display( Name = "Confirm password" )]
			[Compare( "Password" , ErrorMessage = "The password and confirmation password do not match." )]
			public string ConfirmPassword { get; set; }

			[Required]
			[DataType( DataType.Text )]
			[Display( Name = "Full name" )]
			public string Name { get; set; }

			[Required]
			[Range( 0 , 199 , ErrorMessage = "Age must be between 0 and 199 years" )]
			[Display( Name = "Age" )]
			public int Age { get; set; }
		}

		public void OnGet ( string returnUrl = null )
		{
			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync ( string returnUrl = null )
		{
			returnUrl = returnUrl ?? Url.Content( "~/" );
			if ( ModelState.IsValid )
			{
				var user = new ApplicationUser
				{
					UserName = Input.Email,
					Email = Input.Email,
					Name = Input.Name,
					Age = Input.Age
				};

				var result = await _userManager.CreateAsync(user, Input.Password);
				if ( result.Succeeded )
				{
					_logger.LogInformation( "User created a new account with password." );

					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { userId = user.Id, code = code },
						protocol: Request.Scheme);

					await _emailSender.SendEmailAsync( Input.Email , "Confirm your email" ,
					    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode( callbackUrl )}'>clicking here</a>." );

					await _signInManager.SignInAsync( user , isPersistent: false );
					return LocalRedirect( returnUrl );
				}
				foreach ( var error in result.Errors )
				{
					ModelState.AddModelError( string.Empty , error.Description );
				}
			}

			// If we got this far, something failed, redisplay form
			return Page( );
		}
	}
}