using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21
{
	public class Rider
	{
		public int Id { get; set; }

		[Column( TypeName = "nvarchar(24)" )]
		public EquineBeast Mount { get; set; }
	}

	public enum EquineBeast
	{
		Donkey,
		Mule,
		Horse,
		Unicorn
	}
}