using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21;

namespace SinjulMSBH_Version21_Sample.Data
{
	//public class ApplicationDbContext: IdentityDbContext<ApplicationUser , ApplicationRole , Guid>

	public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
	{
		//private static readonly ILoggerFactory _loggerFactory
		//= new LoggerFactory().AddConsole((s, l) => l == LogLevel.Information && !s.EndsWith("Connection"));

		//public static long InstanceCount;

		//private readonly string _tenantId;

		//private const string connectionString = "Server=MSBH1\\MSSQLSERVER2017;Database=aspnet-SinjulMSBH_Version21_Sample;Trusted_Connection=True;MultipleActiveResultSets=true";

		//public ApplicationDbContext ( string tenant )
		//{
		//	_tenantId = tenant;
		//}

		public ApplicationDbContext ( DbContextOptions options ) : base( options )
		{ }//=> Interlocked.Increment( ref InstanceCount );

		#region DbSet

		public DbSet<Product> Products { get; set; }
		//public DbSet<Blog> Blogs { get; set; }
		//public DbSet<Post> Posts { get; set; }
		//public DbSet<Rider> Riders { get; set; }
		//public virtual DbSet<Customer> Customers { get; set; }

		#endregion DbSet

		//protected override void OnConfiguring ( DbContextOptionsBuilder optionsBuilder )
		//	  => optionsBuilder
		//	  .UseLazyLoadingProxies( )
		//	  //.UseSqlServer( connectionString )
		//	  .UseLoggerFactory( _loggerFactory );

		protected override void OnModelCreating ( ModelBuilder modelBuilder )
		{
			#region QueryFilters

			//modelBuilder.Entity<Blog>( ).Property<string>( "TenantId" ).HasField( "_tenantId" );
			// Configure entity filters
			//modelBuilder.Entity<Blog>( ).HasQueryFilter( b => EF.Property<string>( b , "TenantId" ) == _tenantId );
			//modelBuilder.Entity<Post>( ).HasQueryFilter( p => !p.IsDeleted );

			#endregion QueryFilters

			#region CompiledQueries

			//	modelBuilder.Entity<Customer>(
			//entity =>
			//{
			//	entity.ToTable( "Customer" , "Sales" );

			//	entity.HasIndex( e => e.AccountNumber )
			//		  .HasName( "AK_Customer_AccountNumber" )
			//		  .IsUnique( );

			//	entity.HasIndex( e => e.TerritoryID )
			//		  .HasName( "IX_Customer_TerritoryID" );

			//	entity.HasIndex( e => e.rowguid )
			//		  .HasName( "AK_Customer_rowguid" )
			//		  .IsUnique( );

			//	entity.Property( e => e.AccountNumber )
			//		  .IsRequired( )
			//		  .HasColumnType( "varchar(10)" )
			//		  .ValueGeneratedOnAddOrUpdate( );

			//	entity.Property( e => e.ModifiedDate )
			//		  .HasColumnType( "datetime" )
			//		  .HasDefaultValueSql( "getdate()" );

			//	entity.Property( e => e.rowguid ).HasDefaultValueSql( "newid()" );
			//} );

			#endregion CompiledQueries

			#region Parameters in entity constructors

			//modelBuilder.Entity<Blog>(
			//    b =>
			//    {
			//	    b.HasKey( "_id" );
			//	    b.Property( e => e.Author );
			//	    b.Property( e => e.Name );
			//    } );

			//modelBuilder.Entity<Post>(
			//    b =>
			//    {
			//	    b.HasKey( "_id" );
			//	    b.Property( e => e.Title );
			//	    b.Property( e => e.PostedOn );
			//    } );

			#endregion Parameters in entity constructors

			#region Value Conversions

			//modelBuilder
			//.Entity<Rider>( )
			//.Property( e => e.Mount )
			//.HasConversion(
			//    v => v.ToString( ) ,
			//    v => ( EquineBeast ) Enum.Parse( typeof( EquineBeast ) , v ) );

			//var converter = new ValueConverter<EquineBeast, string>(
			//   v => v.ToString(),
			//   v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

			//modelBuilder
			//    .Entity<Rider>( )
			//    .Property( e => e.Mount )
			//    .HasConversion( converter );

			//var converter2 = new EnumToStringConverter<EquineBeast>();

			//modelBuilder
			//    .Entity<Rider>( )
			//    .Property( e => e.Mount )
			//    .HasConversion( converter2 );

			//modelBuilder
			//.Entity<Rider>( )
			//.Property( e => e.Mount )
			//.HasConversion<string>( );

			#endregion Value Conversions

			#region Data Seeding

			modelBuilder.Entity<Product>( ).SeedData(
				new Product { Description="Description01" , HomePage="HomePage01" , Number=4 , ProductName="ProductName01" , UnitPrice=44 } ,
				new Product { Description="Description02" , HomePage="HomePage02" , Number=8 , ProductName="ProductName02" , UnitPrice=444 }
			);

			modelBuilder.Entity<Blog>( ).SeedData( new Blog { Id = 1 , Url = "http://sample.com" } );

			modelBuilder.Entity<Post>( ).SeedData(
				 new { BlogId = 1 , PostId = 1 , Title = "First post" , Content = "Test 1" } ,
				 new { BlogId = 1 , PostId = 2 , Title = "Second post" , Content = "Test 2" } );

			#endregion Data Seeding

			#region Query Types

			//modelBuilder
			//		.Query<BlogPostsCount>( ).ToTable( "View_BlogPostCounts" )
			//		.Property( v => v.BlogName ).HasColumnName( "Name" );

			//var postCounts = db.BlogPostCounts.ToList();

			//foreach ( var postCount in postCounts )
			//{
			//	Console.WriteLine( $"{postCount.BlogName} has {postCount.PostCount} posts." );
			//	Console.WriteLine( );
			//}

			#endregion Query Types

			#region Include on derived types

			//modelBuilder.Entity<School>( ).HasMany( s => s.Students ).WithOne( s => s.School );

			//context.People.Include( person => ( ( Student ) person ).School ).ToList( )
			//context.People.Include( person => ( person as Student ).School ).ToList( )
			//context.People.Include( "Student" ).ToList( )

			#endregion Include on derived types

			#region Using System.Transactions

			//using ( var scope = new TransactionScope( TransactionScopeOption.Required ,
			//	new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted } ) )
			//{
			//	var connection = new SqlConnection(connectionString);
			//	connection.Open( );

			//	try
			//	{
			//		// Run raw ADO.NET command in the transaction
			//		var command = connection.CreateCommand();
			//		command.CommandText = "DELETE FROM dbo.Blogs";
			//		command.ExecuteNonQuery( );

			//		// Run an EF Core command in the transaction
			//		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			//		.UseSqlServer(connection)
			//		.Options;

			//		using ( var context = new ApplicationDbContext( options ) )
			//		{
			//			context.Blogs.Add( new Blog { Url = "http://blogs.msdn.com/dotnet" } );
			//			context.SaveChanges( );
			//		}

			//		// Commit transaction if all commands succ	eed, transaction will auto-rollback
			//		// when disposed if either commands fails
			//		scope.Complete( );
			//	}
			//	catch ( System.Exception )
			//	{
			//		// TODO: Handle failure
			//	}
			//}

			//using ( var transaction = new CommittableTransaction(
			//	   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted } ) )
			//{
			//	var connection = new SqlConnection(connectionString);

			//	try
			//	{
			//		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			//			.UseSqlServer(connection)
			//			.Options;

			//		using ( var context = new ApplicationDbContext( options ) )
			//		{
			//			context.Database.EnlistTransaction( transaction );
			//			context.Database.OpenConnection( );

			//			// Run raw ADO.NET command in the transaction
			//			var command = connection.CreateCommand();
			//			command.CommandText = "DELETE FROM dbo.Blogs";
			//			command.ExecuteNonQuery( );

			//			// Run an EF Core command in the transaction
			//			context.Blogs.Add( new Blog { Url = "http://blogs.msdn.com/dotnet" } );
			//			context.SaveChanges( );
			//		}

			//		// Commit transaction if all commands succeed, transaction will auto-rollback
			//		// when disposed if either commands fails
			//		transaction.Commit( );
			//	}
			//	catch ( System.Exception )
			//	{
			//		// TODO: Handle failure
			//	}
			//}

			#endregion Using System.Transactions

			#region Optimization of correlated subqueries

			//var query = context.Customers.Select(
			//   c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount));

			//var query = context.Customers.Select(
			//   c => c.Orders.Where(o => o.Amount  > 100).Select(o => o.Amount).ToList());

			#endregion Optimization of correlated subqueries
		}

		//public override int SaveChanges ( )
		//{
		//	ChangeTracker.DetectChanges( );

		//	foreach ( var item in ChangeTracker.Entries( ).Where(
		//	    e =>
		//		  e.State == EntityState.Added && e.Metadata.GetProperties( ).Any( p => p.Name == "TenantId" ) ) )
		//	{
		//		item.CurrentValues[ "TenantId" ] = _tenantId;
		//	}

		//	foreach ( var item in ChangeTracker.Entries<Post>( ).Where( e => e.State == EntityState.Deleted ) )
		//	{
		//		item.State = EntityState.Modified;
		//		item.CurrentValues[ "IsDeleted" ] = true;
		//	}

		//	return base.SaveChanges( );
		//}
	}

	//public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
	//{
	//	public ApplicationDbContext CreateDbContext ( string[ ] args )
	//	{
	//		//var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
	//		//optionsBuilder.UseSqlite( "Data Source=blog.db" );

	//		//return new ApplicationDbContext( optionsBuilder.Options );

	//		IConfigurationRoot configuration = new ConfigurationBuilder()
	//			.SetBasePath(Directory.GetCurrentDirectory())
	//			.AddJsonFile("appsettings.json")
	//			.Build();

	//		var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

	//		var connectionString = configuration.GetConnectionString("DefaultConnection");
	//		/*User ID=DBProject;Password=123123*/
	//		//builder.UseSqlServer( @"Data Source=MSBH1\MSSQLSERVER2017;Initial Catalog=aspnet-SinjulMSBH_Version21_Sample;Integrated Security=True" );

	//		builder.UseSqlServer( connectionString );

	//		return new ApplicationDbContext( builder.Options );
	//	}
	//}

	#region Entities

	public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class Student: Person
	{
		public School School { get; set; }
	}

	public class School
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public List<Student> Students { get; set; }
	}

	[Owned]
	public class StreetAddress
	{
		public string Street { get; set; }
		public string City { get; set; }
	}

	public class Order
	{
		public int Id { get; set; }
		public StreetAddress ShippingAddress { get; set; }
	}

	#endregion Entities
}