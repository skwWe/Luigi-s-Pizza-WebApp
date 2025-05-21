using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AuthServiceTool;
using PizzaWebApp;
using Supabase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Настройка HttpClient по умолчанию
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Настройка Supabase
builder.Services.AddSingleton(sp =>
{
    var supabaseUrl = "https://qfpyxrvpedzwdtmifthl.supabase.co";
    var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFmcHl4cnZwZWR6d2R0bWlmdGhsIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczNzczNTgsImV4cCI6MjA2Mjk1MzM1OH0.YQiPwj5aPWLXFhDMKf5polnpL2dCd5MP7HxjaDDYE5Q";

    var client = new Supabase.Client(supabaseUrl, supabaseKey, new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    });

    return client;
});

// Регистрация AuthService с внедрением Supabase клиента
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();



