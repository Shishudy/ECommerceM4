using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Hosting.Server;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebStore.Components.Pages.FrontOffice;

namespace WebStore.Services.CostumeAuthStateProvider
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly ProtectedLocalStorage _localStorage;
		private const string TokenKey = "authToken";

		public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
		{
			_localStorage = localStorage;
		}
		public void Notify()
		{
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		//Serve para informar o Blazor que o estado de autenticação mudou(por exemplo, login ou logout).
		//Todos os componentes que usam AuthenticationStateProvider ou<AuthorizeView> vão reagir e re-renderizar.
		public async Task SetToken(string token)
		{
			await _localStorage.SetAsync(TokenKey, token);
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public async Task Logout()
		{
			await _localStorage.DeleteAsync(TokenKey);
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var storedToken = await _localStorage.GetAsync<string>(TokenKey);
				var token = storedToken.Success ? storedToken.Value : null;

				Console.WriteLine("Token armazenado: " + token);

				if (string.IsNullOrWhiteSpace(token))
				{
					Console.WriteLine("Token está vazio ou nulo.");
					return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
				}

				var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

				foreach (var claim in jwt.Claims)
				{
					Console.WriteLine($"Claim: {claim.Type} - {claim.Value}");
				}

				var identity = new ClaimsIdentity(jwt.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
				return new AuthenticationState(new ClaimsPrincipal(identity));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Erro ao obter estado de autenticação: " + ex.Message);
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}
		}

	}
}

