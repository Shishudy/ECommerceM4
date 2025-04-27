using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using Microsoft.EntityFrameworkCore;
using StoreLibrary.DTOs.Campaigns;
using StoreLibrary.Models;

namespace StoreAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CampaignController : ControllerBase
	{
		private readonly StoreDbContext _context;

		public CampaignController(StoreDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDTO dto)
		{
			if (dto.DateStart == default || dto.DateEnd == default || string.IsNullOrWhiteSpace(dto.Name))
			{
				return BadRequest("Invalid data.");
			}

			var campaign = new Campaign
			{
				Name = dto.Name,
				DateStart = dto.DateStart,
				DateEnd = dto.DateEnd
			};

			_context.Campaigns.Add(campaign);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Campaign created successfully!", id = campaign.PkCampaign });
		}


		[HttpGet]
		public async Task<IActionResult> GetCampaigns()
		{
			var campaigns = await _context.Campaigns.ToListAsync();
			return Ok(campaigns);
		}

		[HttpGet("active")]
		public async Task<IActionResult> GetActiveCampaigns()
		{
			var today = DateOnly.FromDateTime(DateTime.Today);

			var activeCampaigns = await _context.Campaigns
				.Where(c => c.DateEnd >= today)
				.Select(c => new CampaignResponseDTO
				{
					PkCampaign = c.PkCampaign,
					Name = c.Name,
					DateStart = c.DateStart,
					DateEnd = c.DateEnd
				})
				.ToListAsync();

			return Ok(activeCampaigns);
		}

		[HttpPost("AssociateProducts")]
		public async Task<IActionResult> AssociateProducts([FromBody] AssociateProductsDTO dto)
		{
			if (!_context.Campaigns.Any(c => c.PkCampaign == dto.CampaignId))
				return NotFound($"Campaign not found: {dto.CampaignId}");

			var today = DateOnly.FromDateTime(DateTime.Today);
			var associations = new List<CampaignProduct>();

			foreach (var item in dto.Products)
			{
				bool inActiveCampaign = await _context.CampaignProducts
					.AnyAsync(cp =>
						cp.FkProduct == item.ProductId &&
						cp.FkCampaignNavigation.DateEnd >= today);

				if (inActiveCampaign)
				{
					return BadRequest($"The product with ID {item.ProductId} is already associated with an active campaign.");
				}

				var exists = await _context.CampaignProducts
					.AnyAsync(cp =>
						cp.FkCampaign == dto.CampaignId &&
						cp.FkProduct == item.ProductId);

				if (!exists)
				{
					var association = new CampaignProduct
					{
						FkCampaign = dto.CampaignId,
						FkProduct = item.ProductId,
						Discount = (double)item.Discount
					};

					associations.Add(association);
				}
			}

			if (associations.Count > 0)
			{
				_context.CampaignProducts.AddRange(associations);
				await _context.SaveChangesAsync();
				return Ok(new { message = "Products successfully associated!", total = associations.Count });
			}
			else
			{
				return BadRequest("No products were associated. They might already be in the selected campaign or in another active campaign.");
			}
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
					ImageUrl = p.FkImage.ToString(), 
					Category = p.FkCategories.FirstOrDefault() != null ? p.FkCategories.First().Name : "Uncategorized"
				})
				.ToListAsync();

			return Ok(products);
		}


	}
}
