using MobileBlazor.Mocks;
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
            //var apiClient = new ApiClient(new HttpClient(), "http://10.0.2.2:5078/");
            var mockedApiClient = new MockedApiClient();
            //builder.Services.AddSingleton<IMobileApiClient>(apiClient);
            builder.Services.AddSingleton(mockedApiClient);
            builder.Services.AddSingleton<IMobileApiClient>(mockedApiClient);
            builder.Services.AddSingleton<IApiClient>(mockedApiClient);
            builder.Services.AddScoped<MapSearcher, MapSearcherClient>();
            builder.Services.AddMudServices();
            
            return builder.Build();
        }
    }
}