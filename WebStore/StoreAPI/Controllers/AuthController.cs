using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreAPI.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _config;

		public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_config = config;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel model)
		{
			if (model.Password != model.ConfirmPassword)
				return BadRequest(new { message = "As palavras-passe não coincidem." });

			var user = new IdentityUser { UserName = model.Email, Email = model.Email };
			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description).ToList();
				return BadRequest(new { errors });
			}

			if (!await _roleManager.RoleExistsAsync("User"))
				await _roleManager.CreateAsync(new IdentityRole("User"));

			await _userManager.AddToRoleAsync(user, "User");

			return Ok(new { message = "Utilizador registado com sucesso." });
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
				return Unauthorized("Credenciais inválidas.");

			var roles = await _userManager.GetRolesAsync(user);
			if (!roles.Contains("User"))
				return Forbid("Apenas utilizadores com o role 'User' podem iniciar sessão.");

			var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, user.Email),
		new Claim(ClaimTypes.Role, "User")
	};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["JwtSettings:Issuer"],
				audience: _config["JwtSettings:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(2),
				signingCredentials: creds
			);

			var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
			return Ok(tokenStr);
		}

	}
}