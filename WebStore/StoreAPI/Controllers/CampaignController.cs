using Microsoft.AspNetCore.Mvc;
using StoreLibrary.DbModels;
using Microsoft.EntityFrameworkCore;
using StoreLibrary.DTOs.Campaigns;
using StoreLibrary.DTOs.Product;


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

			int nextId = (_context.Campaigns.Any() ? _context.Campaigns.Max(c => c.PkCampaign) : 0) + 1;

			var campaign = new Campaign
			{
				PkCampaign = nextId,
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
			var campanhas = await _context.Campaigns.ToListAsync();
			return Ok(campanhas);
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
				return NotFound($"Campanha não encontrada: {dto.CampaignId}");

			var today = DateOnly.FromDateTime(DateTime.Today);
			var associations = new List<CampaignProduct>();

			foreach (var item in dto.Products)
			{
				bool inActiveCampaign = await _context.CampaignProducts
					.AnyAsync(cp =>
						cp.FkProduct == item.ProductId &&
						cp.FkCampaignNavigation.DateStart <= today &&
						cp.FkCampaignNavigation.DateEnd >= today);

				if (inActiveCampaign)
				{
					return BadRequest($"O produto com ID {item.ProductId} já está em uma campanha ativa.");
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
				return Ok(new { message = "Produtos associados com sucesso!", total = associations.Count });
			}
			else
			{
				return BadRequest("Nenhum produto foi associado. Pode já estar na campanha ou em campanha ativa.");
			}
		}

		[HttpGet("/api/product")]
		public async Task<IActionResult> GetAllProducts()
		{
			var products = await _context.Products
				.Include(p => p.FkCategories)
				.Include(p => p.FkImageNavigation)
				.Select(p => new ProductListDTO
				{
					PkProduct = p.PkProduct,
					Name = p.Name,
					Ean = p.Ean,
					ImageUrl = p.FkImageNavigation.PathImg,

					Categories = p.FkCategories
						.Select(c => new ProductCategoryDTO
						{
							CategoryId = c.PkCategory,
							Name = c.Name
						}).ToList()
				})
				.ToListAsync();

			return Ok(products);
		}
	}
}
