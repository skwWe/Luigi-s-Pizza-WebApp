using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AuthServiceTool;
using PizzaWebApp;

using Supabase;
using YourNamespace.Services;
using Microsoft.JSInterop;
using Microsoft.Extensions.Hosting;
using PizzaWebApp.Models;
using PizzaWebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Настройка HttpClient по умолчанию
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Настройка Supabase
builder.Services.AddSingleton<Client>(sp =>
{
    var supabaseUrl = "https://qfpyxrvpedzwdtmifthl.supabase.co";
    var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFmcHl4cnZwZWR6d2R0bWlmdGhsIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczNzczNTgsImV4cCI6MjA2Mjk1MzM1OH0.YQiPwj5aPWLXFhDMKf5polnpL2dCd5MP7HxjaDDYE5Q";

    var client = new Supabase.Client(supabaseUrl, supabaseKey, new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = false
    });

    client.InitializeAsync().Wait();

    return client;
});
builder.Services.AddScoped(sp => new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
});


// Регистрация AuthService с внедрением Supabase клиента
builder.Services.AddScoped<AuthService>();

// Program.cs
builder.Services.AddScoped<ProfileService>(); // Добавьте эту строку
// Добавляем в DI-контейнер
builder.Services.AddSingleton<ISupabaseMenuWrapper, SupabaseMenuWrapper>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddScoped<ISupabaseCartWrapper, SupabaseCartWrapper>();
builder.Services.AddScoped<CartService>();



await builder.Build().RunAsync();



