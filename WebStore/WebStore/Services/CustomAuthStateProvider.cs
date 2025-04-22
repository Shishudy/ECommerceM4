using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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

		public async void SetToken(string token)
		{
			await _localStorage.SetAsync(TokenKey, token);
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public async void Logout()
		{
			await _localStorage.DeleteAsync(TokenKey);
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			AuthenticationState? result = null;

			try
			{
				var storedToken = await _localStorage.GetAsync<string>(TokenKey);
				var token = storedToken.Success ? storedToken.Value : null;

				var identity = string.IsNullOrWhiteSpace(token)
					? new ClaimsIdentity()
					: new ClaimsIdentity(new JwtSecurityTokenHandler().ReadJwtToken(token).Claims, "jwt");

				result = new AuthenticationState(new ClaimsPrincipal(identity));
			}
			catch (InvalidOperationException)
			{
				result = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}
			return result;
		}

	}
}
