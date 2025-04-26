using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Azure.Core.Serialization;
using StoreLibrary.DbModels;
using StoreLibrary.Models;

namespace WebStore.Services
{
	public class ProductService
	{
		private readonly HttpClient _http;

		public ProductService(IHttpClientFactory factory)
		{
			_http = factory.CreateClient("API");
		}

		public async Task<List<ProductDTO>> GetProductsListAsync(FilterDTO filter, string? category = null)
		{
			string url = "api/products/category";

			if (category != null)
				url = url + $"/{category}";

			var queryParams = new List<string>();

			if (filter.MinPrice != null)
				queryParams.Add($"minPrice={filter.MinPrice.Value}");

			if (filter.MaxPrice != null)
				queryParams.Add($"maxPrice={filter.MaxPrice.Value}");

			if (filter.InStock != null)
				queryParams.Add($"inStock={filter.InStock.Value.ToString().ToLower()}");

			if (queryParams.Any())
			{
				url += "?" + string.Join("&", queryParams);
			}

			var response = await _http.GetAsync(url);

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<List<ProductDTO>>();

			return null;
		}

		public async Task<ProductPageDTO> GetProductAsync(string ean)
		{
			var response = await _http.GetAsync($"api/products/product/{ean}");

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<ProductPageDTO>();

			return null;
		}

		public async Task<string> UpdateFavourite(int productId)
		{
			var response = await _http.GetAsync($"api/favourites/toggle/{productId}");

			if (response.IsSuccessStatusCode)
				return await response.Content.ReadFromJsonAsync<string>();

			return "Erro na operação, tente novamente!";
		}

		//public async Task AddProductAsync(Product product)
		//{
		//	await _http.PostAsJsonAsync("api/products", product);
		//}

		//public async Task UpdateProductAsync(Product product)
		//{
		//	await _http.PutAsJsonAsync($"api/products/{product.Ean}", product);
		//}

		//public async Task RemoveUpdateAsync(Product product)
		//{
		//	await _http.DeleteAsync($"api/products/{product.Ean}");
		//}
	}
}
