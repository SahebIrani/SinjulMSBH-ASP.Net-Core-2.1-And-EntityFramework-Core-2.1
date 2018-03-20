using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinjulMSBH_Version21_Sample.Data;
using SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21;

namespace SinjulMSBH_Version21_Sample.Controllers
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class Products3Controller: ControllerBase
	{
		private readonly ProductsRepository _repository;

		public Products3Controller ( ProductsRepository repository )
		{
			_repository = repository;
		}

		[HttpGet]
		public IEnumerable<Product> Get ( )
		{
			return _repository.GetProducts( );
		}

		//[HttpGet( "{id}" )]
		//public ActionResult<Product> Get ( int id )
		//{
		//	if ( !_repository.TryGetProduct( id , out var product ) )
		//	{
		//		return NotFound( );
		//	}
		//	return product;
		//}

		[HttpPost]
		[ProducesResponseType( 201 )]
		public ActionResult<Product> Post ( Product product )
		{
			_repository.AddProduct( product );
			return CreatedAtAction( nameof( Get ) , new { id = product.Id } , product );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////

	[Produces( "application/json" )]
	[Route( "api/Products" )]
	public class ProductController: Controller
	{
		private readonly ApplicationDbContext _context;

		public ProductController ( ApplicationDbContext context )
		{
			_context=context;
		}

		// GET: api/Products
		[HttpGet]
		public async Task<IActionResult> GetProducts ( )
		{
			var product = await _context.Products.ToListAsync();
			return Ok( product );
		}

		// GET: api/Products/5
		[HttpGet( "{id}" )]
		public async Task<IActionResult> GetProduct ( [FromRoute] int id )
		{
			if ( !ModelState.IsValid )
			{
				return BadRequest( ModelState );
			}

			var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);

			if ( product == null )
			{
				return NotFound( );
			}

			return Ok( product );
		}

		// POST: api/Products
		[HttpPost]
		public async Task<IActionResult> PostProduct ( [FromBody] Product product )
		{
			if ( !ModelState.IsValid )
			{
				return BadRequest( ModelState );
			}

			_context.Entry( product ).State = EntityState.Added;
			await _context.SaveChangesAsync( );

			return CreatedAtAction( "GetProduct" , new { id = product.Id } , product );
		}

		// PUT: api/Products/5
		[HttpPut( "{id}" )]
		public async Task<IActionResult> PutProduct ( [FromRoute] int id , [FromBody] Product product )
		{
			if ( !ModelState.IsValid )
			{
				return BadRequest( ModelState );
			}

			if ( id != product.Id )
			{
				return BadRequest( );
			}

			_context.Entry( product ).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync( );
			}
			catch ( DbUpdateConcurrencyException )
			{
				if ( !ProductExists( id ) )
				{
					return NotFound( );
				}
				throw;
			}

			return Ok( product );
		}

		// DELETE: api/Products/5
		[HttpDelete( "{id}" )]
		public async Task<IActionResult> DeleteProduct ( [FromRoute] int id )
		{
			if ( !ModelState.IsValid )
			{
				return BadRequest( ModelState );
			}

			var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
			if ( product == null )
			{
				return NotFound( );
			}

			_context.Entry( product ).State = EntityState.Deleted;
			await _context.SaveChangesAsync( );

			return NoContent( );
		}

		private bool ProductExists ( int id )
		{
			return _context.Products.Any( e => e.Id == id );
		}
	}

	/// ////////////////////////////////////////////////////////////////////////////////////

	//public class AzureAlertController: ControllerBase
	//{
	//	[AzureAlertWebHook]
	//	public IActionResult AzureAlert ( string id , string @event , AzureAlertNotification data )
	//	{
	//		if ( !ModelState.IsValid )
	//		{
	//			return BadRequest( ModelState );
	//		}

	//		// Get the notification status
	//		var status = data.Status;

	//		// Get the notification name
	//		var name = data.Context.Name;

	//		// Get the name of the metric that caused the event
	//		var author = data.Context.Condition.MetricName;

	//		return Ok( );
	//	}
	//}

	/// ////////////////////////////////////////////////////////////////////////////////////
	public class Value
	{
		public int ID { get; set; }

		public string Text { get; set; }

		public IDictionary<int , string> Status { get; } = new Dictionary<int , string>( );

		//		[
		//  { "op": "test", "path": "/text", "value": "Do" },

		//  { "op": "add", "path": "/status/1", "value": "Done!" }

		//]
		//{
		//  "id": 123,
		//  "text": "Do",
		//  "status": {
		//    "1": "Done!"
		//  }
		//}
		//[
		//  { "op": "test", "path": "/text", "value": "Do not" },
		//  { "op": "add", "path": "/status/1", "value": "Done!" }
		//]
		//{
		//  "Value": [
		//    "The current value 'Do' at path 'text' is not equal to the test value 'Do not'."
		//  ]
		//}
	}

	[Produces( "application/json" )]
	[Route( "api/[controller]" )]
	[ApiController]
	public class ProductsController: ControllerBase
	{
		private readonly ProductsRepository _repository;

		public ProductsController ( ProductsRepository repository )
		{
			_repository = repository;
		}

		[HttpPatch( "{id}" )]
		public ActionResult<Value> Patch ( int id , JsonPatchDocument<Value> patch )
		{
			var value = new Value { ID = id, Text = "Do" };

			patch.ApplyTo( value , ModelState );

			if ( !ModelState.IsValid )
			{
				return BadRequest( ModelState );
			}

			return value;
		}

		[HttpGet]
		public IEnumerable<Product> Get ( )
		{
			return _repository.GetProducts( );
		}

		[HttpGet( "test/{testId}" )]
		public IEnumerable<Product> Get ( string testId , [Required]string name )
		{
			return _repository.GetProducts( );
		}

		[HttpGet( "{id}" )]
		public ActionResult<Product> Get ( int id )
		{
			if ( !_repository.TryGetProduct( id , out var value ) )
			{
				return NotFound( );
			}
			return new Product { };
		}

		[HttpPost]
		[ProducesResponseType( 201 )]
		public ActionResult<Product> Post ( Product product )
		{
			_repository.AddProduct( product );
			return CreatedAtAction( nameof( Get ) , nameof( ProductsController ) , new { id = product.Id } , product );
		}
	}

	public class ProductsRepository
	{
		private readonly ApplicationDbContext _contextMe;

		public ProductsRepository ( ApplicationDbContext contextMe )
		{
			_contextMe=contextMe;
		}

		internal void AddProduct ( Product product )
		{
			_contextMe.Add( product );
		}

		internal IEnumerable<Product> GetProducts ( )
		{
			return _contextMe.Products.ToList( );
		}

		internal bool TryGetProduct ( int id , out object product )
		{
			throw new NotImplementedException( );
		}
	}
}