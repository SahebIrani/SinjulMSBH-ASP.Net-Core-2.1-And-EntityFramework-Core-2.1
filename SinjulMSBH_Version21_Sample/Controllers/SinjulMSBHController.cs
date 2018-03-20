using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SinjulMSBH_Version21_Sample.Data;
using SinjulMSBH_Version21_Sample.Models.EntityFrameworkCore21;

namespace SinjulMSBH_Version21_Sample.Controllers
{
	public class SinjulMSBHController: Controller
	{
		private readonly ApplicationDbContext _context;

		public SinjulMSBHController ( ApplicationDbContext context )
		{
			_context = context;
		}

		public async Task<IActionResult> Index ( )
		{
			return View( await _context.Products.ToListAsync( ) );
		}

		// GET: SinjulMSBH
		public async Task<IActionResult> ProductListUsingTagHelpers ( )
		{
			return View( await _context.Products.ToListAsync( ) );

			//  .GroupBy( o => new { o.CustomerId , o.EmployeeId } )
			//  .Select( g => new
			//  {
			// 	 g.Key.CustomerId ,
			// 	 g.Key.EmployeeId ,
			// 	 Sum = g.Sum( o => o.Amount ) ,
			// 	 Min = g.Min( o => o.Amount ) ,
			// 	 Max = g.Max( o => o.Amount ) ,
			// 	 Avg = g.Average( o => Amount )
			//  } );
		}

		// GET: SinjulMSBH/Details/5
		public async Task<IActionResult> Details ( int? id )
		{
			if ( id == null )
			{
				return NotFound( );
			}

			var product = await _context.Products
		    .FirstOrDefaultAsync(m => m.Id == id);
			if ( product == null )
			{
				return NotFound( );
			}

			return View( product );
		}

		// GET: SinjulMSBH/Create
		public IActionResult Create ( )
		{
			return View( );
		}

		// POST: SinjulMSBH/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create ( [Bind( "Id,ProductName,UnitPrice,Number,Description,HomePage" )] Product product )
		{
			if ( ModelState.IsValid )
			{
				_context.Add( product );
				await _context.SaveChangesAsync( );
				return RedirectToAction( nameof( Index ) );
			}
			return View( product );
		}

		// GET: SinjulMSBH/Edit/5
		public async Task<IActionResult> Edit ( int? id )
		{
			if ( id == null )
			{
				return NotFound( );
			}

			var product = await _context.Products.FindAsync(id);
			if ( product == null )
			{
				return NotFound( );
			}
			return View( product );
		}

		// POST: SinjulMSBH/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit ( int id , [Bind( "Id,ProductName,UnitPrice,Number,Description,HomePage" )] Product product )
		{
			if ( id != product.Id )
			{
				return NotFound( );
			}

			if ( ModelState.IsValid )
			{
				try
				{
					_context.Update( product );
					await _context.SaveChangesAsync( );
				}
				catch ( DbUpdateConcurrencyException )
				{
					if ( !ProductExists( product.Id ) )
					{
						return NotFound( );
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction( nameof( Index ) );
			}
			return View( product );
		}

		// GET: SinjulMSBH/Delete/5
		public async Task<IActionResult> Delete ( int? id )
		{
			if ( id == null )
			{
				return NotFound( );
			}

			var product = await _context.Products
		    .FirstOrDefaultAsync(m => m.Id == id);
			if ( product == null )
			{
				return NotFound( );
			}

			return View( product );
		}

		// POST: SinjulMSBH/Delete/5
		[HttpPost, ActionName( "Delete" )]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed ( int id )
		{
			var product = await _context.Products.FindAsync(id);
			_context.Products.Remove( product );
			await _context.SaveChangesAsync( );
			return RedirectToAction( nameof( Index ) );
		}

		private bool ProductExists ( int id )
		{
			return _context.Products.Any( e => e.Id == id );
		}
	}
}