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
	public class FavouriteService
	{
		private readonly HttpClient _http;

		public FavouriteService(IHttpClientFactory factory)
		{
			_http = factory.CreateClient("API");
		}

		public async Task<string?> UpdateFavourite(int productId)
		{
			var response = await _http.PostAsync($"api/favourites/toggle/{productId}", null);

			return await response.Content.ReadFromJsonAsync<string>();
		}
	}
}
