using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21
{
	public static class PocoLoadingExtensions
	{
		public static TRelated Load<TRelated> (
		    this Action<object , string> loader ,
		    object entity ,
		    ref TRelated navigationField ,
		    [CallerMemberName] string navigationName = null )
		    where TRelated : class
		{
			loader?.Invoke( entity , navigationName );

			return navigationField;
		}
	}
}