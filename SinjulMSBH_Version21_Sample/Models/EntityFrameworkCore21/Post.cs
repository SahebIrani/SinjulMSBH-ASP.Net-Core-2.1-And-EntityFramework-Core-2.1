using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21
{
	public class Post
	{
		private Blog _blog;

		private int _id;

		public Post ( string title , DateTime postedOn )
		{
			Title = title;
			PostedOn = postedOn;
		}

		public Post ( int id , string title , DateTime postedOn )
		{
			Id = id;
			Title = title;
			PostedOn = postedOn;
		}

		private Post ( Action<object , string> lazyLoader )
		{
			LazyLoader = lazyLoader;
		}

		private Action<object , string> LazyLoader { get; set; }

		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime PostedOn { get; set; }
		public bool IsDeleted { get; set; }

		public Blog Blog
		{
			get => LazyLoader?.Load( this , ref _blog );
			set => _blog = value;
		}

		//public Blog Blog { get; set; }
	}
}