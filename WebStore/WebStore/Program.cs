using Radzen;
using WebStore.Components;

namespace WebStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

			// Register Radzen services
			builder.Services.AddScoped<DialogService>();
			builder.Services.AddScoped<NotificationService>();
			builder.Services.AddScoped<TooltipService>();
			builder.Services.AddScoped<ContextMenuService>();

			// Register other services
			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:11825/") });
			//builder.Services.AddScoped<>();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
