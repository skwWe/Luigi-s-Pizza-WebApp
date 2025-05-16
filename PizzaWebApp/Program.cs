using System;
using Supabase;
using Supabase.Gotrue;
using Microsoft.Extensions.Configuration;
using PizzaWebApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


public class Program
{

    public class SupabaseService
    {
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private Supabase.Client _supabaseClient;

        public SupabaseService(IConfiguration configuration) // Получаем настройки из appsettings.json
        {
            // Загрузка из конфигурации (рекомендуется)
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:Key"];

            // **Внимание для Blazor WebAssembly:**  Здесь нельзя делать Console.WriteLine,
            // так как этот код выполняется в браузере!  Используйте ILogger, если нужна отладка.
        }

        public async Task InitializeSupabase()
        {
            var options = new SupabaseOptions
            {
                // Можете добавить здесь дополнительные настройки, если необходимо.
            };

            _supabaseClient = new Supabase.Client(_supabaseUrl, _supabaseKey, options);

            try
            {
                await _supabaseClient.InitializeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error initializing Supabase: {e.Message}");
                // Обработка ошибки инициализации
            }
        }

        public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Регистрируйте сервисы здесь, например:
        // builder.Services.AddScoped<MyService>();

        await builder.Build().RunAsync();
    }
    }
}
