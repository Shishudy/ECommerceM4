using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebStore.Services
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly ProtectedLocalStorage _localStorage;
		private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
		private const string TokenKey = "authToken";

		public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
		{
			_localStorage = localStorage;
		}

		public void Notify()
		{
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

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
			AuthenticationState anonymousState = new AuthenticationState(_anonymous);

			try
			{

				var result = await _localStorage.GetAsync<string>(TokenKey);

				if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
				{
					return anonymousState;
				}

				var token = result.Value;
				var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

				if (jwt.ValidTo < DateTime.UtcNow)
				{

					await _localStorage.DeleteAsync(TokenKey);
					return anonymousState;
				}

				var identity = new ClaimsIdentity(jwt.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
				var user = new ClaimsPrincipal(identity);

				return new AuthenticationState(user);
			}
			catch (InvalidOperationException)
			{

				return anonymousState;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Erro ao obter estado de autenticação: " + ex.Message);
				return anonymousState;
			}
		}
	}
}
