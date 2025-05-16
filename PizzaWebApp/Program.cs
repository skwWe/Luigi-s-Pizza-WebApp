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

        public SupabaseService(IConfiguration configuration) // �������� ��������� �� appsettings.json
        {
            // �������� �� ������������ (�������������)
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseKey = configuration["Supabase:Key"];

            // **�������� ��� Blazor WebAssembly:**  ����� ������ ������ Console.WriteLine,
            // ��� ��� ���� ��� ����������� � ��������!  ����������� ILogger, ���� ����� �������.
        }

        public async Task InitializeSupabase()
        {
            var options = new SupabaseOptions
            {
                // ������ �������� ����� �������������� ���������, ���� ����������.
            };

            _supabaseClient = new Supabase.Client(_supabaseUrl, _supabaseKey, options);

            try
            {
                await _supabaseClient.InitializeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error initializing Supabase: {e.Message}");
                // ��������� ������ �������������
            }
        }

        public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // ������������� ������� �����, ��������:
        // builder.Services.AddScoped<MyService>();

        await builder.Build().RunAsync();
    }
    }
}
