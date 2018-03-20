using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21
{
	public class Product
	{
		public int Id { get; set; }
		public string ProductName { get; set; }
		public decimal UnitPrice { get; set; }
		public int Number { get; set; }
		public string Description { get; set; }
		public string HomePage { get; set; }
	}
}