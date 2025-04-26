using WebStore.Services;
using StoreLibrary.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
namespace WebStore.Services
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
				await _authProvider.SetToken(token);

				_authProvider.Notify();

				return "success";
			}

			return "fail";
		}




		public async Task<object> Register(RegisterModel model)
		{
			var response = await _http.PostAsJsonAsync("api/auth/register", model);

			if (response.IsSuccessStatusCode)
			{
				return "success";
			}
			var content = await response.Content.ReadFromJsonAsync<List<IdentityError>>();

			if (content != null)
			{
				var messages = content.Select(e => e.Description).ToList();
				return messages;
			}

			return new List<string> { "Erro desconhecido." };
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

