using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using StoreLibrary.DbModels;
using StoreLibrary.Models;

namespace StoreAPI.Controllers
{
	[Route("api/products")]
	[ApiController]
	public class ProductsController: ControllerBase
	{
		private readonly StoreDbContext _context;

		public ProductsController(StoreDbContext context)
		{
			_context = context;
		}

		// GET: api/products/category/tecnologia
		[HttpGet("category/{category}")]
		public async Task<ActionResult<IEnumerable<Product>>> GetProductsList(string category, [FromQuery] Filter filter)
		{
			var query = _context.Products
				.Where(p => p.Toggle == true)
				.Include(p => p.FkCategories)
				.Include(p => p.FkImageNavigation)
				.AsQueryable();

			query = query
				   .Where(p => string.IsNullOrEmpty(category) || p.FkCategories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)))
				   .Where(p => !filter.MinPrice.HasValue || p.Price >= filter.MinPrice)
				   .Where(p => !filter.MaxPrice.HasValue || p.Price <= filter.MaxPrice)
				   .Where(p => !filter.InStock.HasValue || (filter.InStock.Value && p.Stock > 0));

			List<Product> productList = await query.ToListAsync();
			
			if (productList == null)
				return NotFound();

			return Ok(productList);
		}

		// GET: api/products/product/534536341234
		[HttpGet("product/{ean}")]
		public async Task<ActionResult<ProductPage>> GetProductPage(string ean)
		{
			// puxar também as reviews do produto
			var product = await _context.Products
			.Where(p => p.Ean == ean)
			.Select(p => new ProductPage
			{
				ProductId = p.PkProduct,
				Ean = p.Ean,
				Name = p.Name,
				Description = p.Description,
				Price = p.Price,
				InStock = p.Stock > 0,
				Discount = p.CampaignProducts
							.Where(cp => cp.FkProduct == p.PkProduct)
							.Select(cp => cp.Discount)
							.FirstOrDefault(),
				Category = p.FkCategories
							.Select(c => c.Name)
							.FirstOrDefault() ?? string.Empty,

				IsFavorite = false,
				ImageList = new List<Image> {
					p.FkImageNavigation
				},
				ReviewList = p.PurchaseProducts
							.Where(pp => pp.FkReview != null)
							.Select(pp => new Review
							{
								PkReview = pp.FkReviewNavigation.PkReview,
								Stars = pp.FkReviewNavigation.Stars,
								Comment = pp.FkReviewNavigation.Comment,
								Toggle = pp.FkReviewNavigation.Toggle,
								FkImages = pp.FkReviewNavigation.FkImages.ToList()
							})
							.Distinct()
							.ToList()
			})
			.FirstOrDefaultAsync();

			if (product == null)
				return NotFound();

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (userId != null)
				product.IsFavorite = await _context.Favourites.AnyAsync(f => f.FkUser == userId && f.FkProduct == product.ProductId);

			return Ok(product);
		}
	}
}
