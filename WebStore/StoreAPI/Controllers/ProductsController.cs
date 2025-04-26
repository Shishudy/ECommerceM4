using System;
using System.Collections.Generic;
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
		[HttpGet("category/{category?}")]
		public async Task<ActionResult<List<ProductDTO>>> GetProductDtoList(string? category, [FromQuery] FilterDTO filter)
		{
			var query = _context.Products
				.Where(p => p.Toggle == true)
				.Include(p => p.FkCategories)
				.Include(p => p.FkImageNavigation)
				.AsQueryable();

			query = query
				.Where(p => string.IsNullOrEmpty(category) ||
							p.FkCategories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)))
				.Where(p => !filter.MinPrice.HasValue || p.Price >= filter.MinPrice.Value)
				.Where(p => !filter.MaxPrice.HasValue || p.Price <= filter.MaxPrice.Value)
				.Where(p => !filter.InStock.HasValue || (filter.InStock.Value && p.Stock > 0));

			DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

			List<ProductDTO> productDtoList = await query
				.Select(p => new ProductDTO
				{
					ProductId = p.PkProduct,
					Ean = p.Ean,
					Name = p.Name,
					Category = p.FkCategories
						.OrderBy(c => c.PkCategory)
						.Select(c => c.Name)
						.FirstOrDefault() ?? string.Empty,
					Description = p.Description,
					Price = p.Price,
					Discount = p.CampaignProducts
						.Where(cp =>
							cp.FkCampaignNavigation.DateStart <= today &&
							cp.FkCampaignNavigation.DateEnd >= today)
						.OrderByDescending(cp => cp.FkCampaignNavigation.DateStart)
						.Select(cp => cp.Discount)
						.FirstOrDefault(),
					InStock = p.Stock > 0,
					IsFavorite = false ,
					ImageUrl = p.FkImageNavigation.PathImg
				})
				.ToListAsync();

			if (productDtoList == null)
				return NotFound();
			
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (userId != null)
			{
				List<int> favoriteProductIds = await _context.Favourites
					.Where(f => f.FkUser == userId)
					.Select(f => f.FkProduct)
					.ToListAsync();

				foreach (ProductDTO productDto in productDtoList)
				{
					productDto.IsFavorite = favoriteProductIds.Contains(productDto.ProductId);
				}
			}

			return Ok(productDtoList);
		}

		[HttpGet("product/{ean}")]
		public async Task<ActionResult<ProductPageDTO>> GetProductPage(string ean)
		{
			var query = _context.Products
				.Where(p => p.Toggle == true)
				.Where(p => p.Ean == ean)
				.AsQueryable();
			
			DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

			ProductPageDTO product = await query
				.Select(p => new ProductPageDTO
				{
					ProductId = p.PkProduct,
					Ean = p.Ean,
					Name = p.Name,
					Description = p.Description,
					Price = p.Price,
					InStock = p.Stock > 0,
					Discount = p.CampaignProducts
						.Where(cp =>
							cp.FkCampaignNavigation.DateStart <= today &&
							cp.FkCampaignNavigation.DateEnd >= today)
						.OrderByDescending(cp => cp.FkCampaignNavigation.DateStart)
						.Select(cp => cp.Discount)
						.FirstOrDefault(),
					Category = p.FkCategories
						.OrderBy(c => c.PkCategory)
						.Select(c => c.Name)
						.FirstOrDefault() ?? string.Empty,
					IsFavorite = false, // Will be updated later
					ImagePathList = new List<string>
					{
						p.FkImageNavigation.PathImg
					},
					ReviewList = p.PurchaseProducts
						.Where(pp => pp.FkReview != null && pp.FkReviewNavigation != null)
						.Select(pp => new ReviewDTO
						{
							ReviewId = pp.FkReviewNavigation.PkReview,
							ReviewDate = pp.FkReviewNavigation.DataReview,
							Stars = pp.FkReviewNavigation.Stars,
							Comment = pp.FkReviewNavigation.Comment,
							ReviewImagesPath = pp.FkReviewNavigation.FkImages != null
								? pp.FkReviewNavigation.FkImages.Select(img => img.PathImg).ToList()
								: new List<string>()
						})
						.Distinct()
						.ToList()
				})
				.FirstOrDefaultAsync();

			if (product == null)
				return NotFound();

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (!string.IsNullOrEmpty(userId))
			{
				product.IsFavorite = await _context.Favourites
					.AnyAsync(f => f.FkUser == userId && f.FkProduct == product.ProductId);
			}

			return Ok(product);
		}
	}
}
