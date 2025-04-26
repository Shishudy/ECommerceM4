using System.Net.Http.Json;
using StoreLibrary.DTOs.Product;

namespace WebStore.Services
{
	public class ProductService
	{
		private readonly HttpClient _http;

		public ProductService(IHttpClientFactory factory)
		{
			_http = factory.CreateClient("API");
		}

		public async Task<List<ProductListDTO>> GetAllProductsAsync()
		{
			return await _http.GetFromJsonAsync<List<ProductListDTO>>("api/product") ?? new();
		}
	}

}
