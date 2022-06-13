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
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddMauiBlazorWebView();

            #if DEBUG
                        builder.Services.AddBlazorWebViewDeveloperTools();
            #endif

            builder.Services.AddScoped<HttpClient>();
            //var apiClient = new ApiClient(new HttpClient(), "http://localhost:5078/");
            var apiClient = new ApiClient(new HttpClient(), "http://10.0.2.2:5078/");
            builder.Services.AddSingleton(apiClient);
            builder.Services.AddSingleton<IApiClient>(apiClient);
            builder.Services.AddScoped<MapSearcher, MapSearcherClient>();
            builder.Services.AddMudServices();
            
            return builder.Build();
        }
    }
}