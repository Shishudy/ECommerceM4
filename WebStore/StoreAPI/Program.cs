using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreAPI.Data;

namespace StoreAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Connection Strings
			var identityConn = builder.Configuration.GetConnectionString("IdentityContextConnection")
							   ?? throw new InvalidOperationException("Missing Identity connection string.");

			// Identity DB
			builder.Services.AddDbContext<IdentityContext>(options =>
				options.UseSqlServer(identityConn));

			builder.Services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<IdentityContext>()
				.AddDefaultTokenProviders();

			// CORS
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
					policy.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader());
			});

			// JWT
			var jwtSettings = builder.Configuration.GetSection("JwtSettings");
			var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};
			});

			builder.Services.AddAuthorization();
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			app.UseCors("AllowAll");

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
