using System.Configuration;
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

			// Read settings from appsettings.json
			var identityConnectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

			builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(identityConnectionString));

			builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityContext>();

			var storedbConnectionString = builder.Configuration.GetConnectionString("StoreDBConnection") ?? throw new InvalidOperationException("Connection string 'StoreDBConnection' not found.");

			builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(storedbConnectionString));

			//Jwt

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

			// Add Cors services

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					policy => policy.AllowAnyOrigin()
									.AllowAnyMethod()
									.AllowAnyHeader());
			});

			builder.Services.AddAuthorization();

			// Add services to the container.
			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors("AllowAll");
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
