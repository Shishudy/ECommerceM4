using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreLibrary.DbModels;
using StoreLibrary.Models;

namespace StoreAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly StoreDbContext _context;

		public ProductsController(StoreDbContext context)
		{
			_context = context;
		}


		[HttpGet("category/{category?}")]
		public async Task<ActionResult<List<ProductDTO>>> GetProductDtoList(string? category, [FromQuery] FilterDTO? filter, string? search)
		{
			var query = _context.Products
				.Where(p => p.Toggle == true)
				.Include(p => p.FkCategories)
				.Include(p => p.FkImages)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(p =>
					p.Name.Contains(search) ||
					p.Description.Contains(search) ||
					p.Ean.Contains(search)
				);
			}

			if (!string.IsNullOrWhiteSpace(category))
			{
				if (category != null)
				{
					query = query.Where(p => p.FkCategories.Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)));
				}

				if (filter.MaxPrice != null)
				{
					query = query.Where(p => p.Price <= filter.MaxPrice);
				}
			}

			var products = await query
				.Select(p => new ProductDTO
				{
					ProductId = p.PkProduct,
					Ean = p.Ean,
					Name = p.Name,
					Description = p.Description,
					Price = p.Price,
					InStock = p.Stock > 0,
					IsFavorite = false,
					MainImage = p.FkImages
						.Select(img => new ImageDTO
						{
							ImageId = img.PkImage,
							PathImg = img.PathImg,
							Name = img.Name
						})
						.FirstOrDefault() ?? new ImageDTO()
				})
				.ToListAsync();

			return Ok(products);
		}

		[HttpGet("product/{ean}")]
		public async Task<ActionResult<ProductPageDTO>> GetProductPage(string ean)
		{
			var query = _context.Products
				.Where(p => p.Toggle == true)
				.Where(p => p.Ean == ean)
				.AsQueryable();

			DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

			ProductPageDTO? product = await query
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
					IsFavorite = false,
					MainImage = p.FkImages
						.Select(img => new ImageDTO
						{
							ImageId = img.PkImage,
							PathImg = img.PathImg,
							Name = img.Name
						})
						.FirstOrDefault() ?? new ImageDTO(),
					ImageDTOList = p.FkImages
						.Where(img => img.PkImage != p.FkImage)
						.Select(img => new ImageDTO
						{
							ImageId = img.PkImage,
							PathImg = img.PathImg,
							Name = img.Name
						})
						.ToList(),
					ReviewDTOList = p.PurchaseProducts
						.Where(pp => pp.FkReviewNavigation != null)
						.Select(pp => new ReviewDTO
						{
							ReviewId = pp.FkReviewNavigation.PkReview,
							ReviewDate = pp.FkReviewNavigation.DataReview,
							Stars = pp.FkReviewNavigation.Stars,
							Comment = pp.FkReviewNavigation.Comment,
							ImageDTOList = pp.FkReviewNavigation.FkImages
								.Select(img => new ImageDTO
								{
									ImageId = img.PkImage,
									PathImg = img.PathImg,
									Name = img.Name
								})
								.ToList()
						})
						.Distinct()
						.ToList()
				})
				.FirstOrDefaultAsync();

			//if (product == null)
			//	return NotFound();

			//var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			//if (!string.IsNullOrEmpty(userId))
			//{
			//	product.IsFavorite = await _context.Favourites
			//		.AnyAsync(f => f.FkUser == userId && f.FkProduct == product.ProductId);
			//}

			return Ok(product);
		}


		
		[HttpGet("/api/product")]
		public async Task<IActionResult> GetAllProducts()
		{
			var products = await _context.Products
				.Include(p => p.FkCategories)
				.Select(p => new ProductDTO
				{
					ProductId = p.PkProduct,
					Ean = p.Ean,
					Name = p.Name,
					Description = p.Description,
					Price = p.Price,
					Discount = 0,
					InStock = p.Stock > 0,
					IsFavorite = false,
					MainImage = new ImageDTO
					{
						ImageId = p.FkImage,
						PathImg = "no-image.png",
						Name = "Default Image"
					},
					Category = p.FkCategories.FirstOrDefault() != null ? p.FkCategories.First().Name : "Uncategorized"
				})
				.ToListAsync();

			return Ok(products);
		}
    }
}
