using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using WebStore;
using WebStore.Components;
using WebStore.Services.AuthService;
using WebStore.Services.CostumeAuthStateProvider;

var builder = WebApplication.CreateBuilder(args);

// Blazor
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Radzen Services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// HTTP Client para API
builder.Services.AddHttpClient("API", client =>
{
	client.BaseAddress = new Uri("http://localhost:5125/"); 
});

// Custom Auth Provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

// Auth Service
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
