using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SinjulMSBH_Version21_Sample.Data
{
	public class ApplicationUser: IdentityUser/*<Guid>*/
	{
		[Required]
		[DataType( DataType.Text )]
		[Display( Name = "Full name" )]
		public string Name { get; set; }

		[Required]
		[Range( 0 , 199 , ErrorMessage = "Age must be between 0 and 199 years" )]
		[Display( Name = "Age" )]
		public int Age { get; set; }
	}
}