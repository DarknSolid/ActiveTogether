using Microsoft.AspNetCore.Components.WebView.Maui;
using MobileBlazor.Data;
using MobileBlazor.Utils;
using MudBlazor.Services;
using System.Net.Http;

namespace MobileBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddBlazorWebView();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddSingleton(new ApiClient(new HttpClient(), "http://192.168.1.129:49160/"));
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddMudServices();

            return builder.Build();
        }
    }
}