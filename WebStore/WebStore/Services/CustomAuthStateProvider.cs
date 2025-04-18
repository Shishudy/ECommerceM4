using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace WebStore.Services.CostumeAuthStateProvider
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private string _token;

		public void SetToken(string token)
		{
			_token = token;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public void Logout()
		{
			_token = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var identity = string.IsNullOrEmpty(_token)
				? new ClaimsIdentity()
				: new ClaimsIdentity(new JwtSecurityTokenHandler().ReadJwtToken(_token).Claims, "jwt");

			return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
		}
	}

}

