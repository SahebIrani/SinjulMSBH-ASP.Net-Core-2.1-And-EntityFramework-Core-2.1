using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21
{
	public class Customer
	{
		public int CustomerID { get; set; }
		public int? PersonID { get; set; }
		public int? StoreID { get; set; }
		public int? TerritoryID { get; set; }
		public string AccountNumber { get; set; }
		public Guid rowguid { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}