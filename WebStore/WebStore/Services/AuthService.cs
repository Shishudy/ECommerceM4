using WebStore.Services.CostumeAuthStateProvider;
using WebStore.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
namespace WebStore.Services.AuthService
{
	public class AuthService
	{
		private readonly HttpClient _http;
		private readonly CustomAuthStateProvider _authProvider;

		public AuthService(IHttpClientFactory factory, CustomAuthStateProvider authProvider)
		{
			_http = factory.CreateClient("API");
			_authProvider = authProvider;
		}

		public async Task<string> Login(LoginModel model)
		{
			var response = await _http.PostAsJsonAsync("api/auth/login", model);

			if (response.IsSuccessStatusCode)
			{
				var token = await response.Content.ReadAsStringAsync();
				_authProvider.SetToken(token);
				return "success";
			}

			return "fail";
		}

		public async Task<object> Register(RegisterModel model)
		{
			var response = await _http.PostAsJsonAsync("api/auth/register", model);

			if (response.IsSuccessStatusCode)
				return "success";

			var content = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
			return content != null && content.ContainsKey("errors")
				? content["errors"]
				: new List<string> { "Erro desconhecido." };
		}

		public async Task<AuthenticationState> GetAuthState()
		{
			return await _authProvider.GetAuthenticationStateAsync();
		}

		public async Task Logout()
		{
			_authProvider.Logout();
			await Task.CompletedTask;
		}



	}
}

