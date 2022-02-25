using Microsoft.AspNetCore.Components.WebView.Maui;
using MobileBlazor.Models;
using MobileBlazor.Utils;
using MudBlazor.Services;
using RazorLib.AbstractClasses;
using RazorLib.Interfaces;

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
            var apiClient = new ApiClient(new HttpClient(), "http://192.168.1.129:49166/");
            builder.Services.AddSingleton(apiClient);
            builder.Services.AddSingleton<IApiClient>(apiClient);
            builder.Services.AddScoped<MapSearcher, MapSearcherClient>();
            builder.Services.AddMudServices();
            
            return builder.Build();
        }
    }
}