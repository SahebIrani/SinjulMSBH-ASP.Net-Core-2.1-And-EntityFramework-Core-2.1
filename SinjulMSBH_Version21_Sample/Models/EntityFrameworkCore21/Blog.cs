using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SinjulMSBH_Version21_Sample.Data;

namespace SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21
{
	public class Blog
	{
		private ICollection<Post> _posts;

		private int _id;

		public Blog ( string name , string author )
		{
			Name = name;
			Author = author;
		}

		public Blog ( int id , string name , string author , string url )
		{
			Id = id;
			Name = name;
			Author = author;
			Url=url;
		}

		private Blog ( Action<object , string> lazyLoader )
		{
			LazyLoader = lazyLoader;
		}

		private Action<object , string> LazyLoader { get; set; }

		public int Id { get; set; }

		public string TenantId { get; set; }
		public string Name { get; set; }

		public string Author { get; set; }
		public string Url { get; set; }

		public ICollection<Post> Posts
		{
			get => LazyLoader?.Load( this , ref _posts );
			set => _posts = value;
		}

		//public ICollection<Post> Posts { get; } = new List<Post>( );

		/// ///////////////////////////////////////////////////
		public Blog ( )
		{
		}

		private Blog ( ApplicationDbContext context )
		{
			Context = context;
		}

		private ApplicationDbContext Context { get; set; }

		//public ICollection<Post> Posts { get; set; }

		public int PostsCount
		    => Posts?.Count
			 ?? Context?.Set<Post>( ).Count( p => Id == EF.Property<int?>( p , "BlogId" ) )
			 ?? 0;
	}
}