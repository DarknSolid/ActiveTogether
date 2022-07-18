using Microsoft.Extensions.DependencyInjection;
using MobileBlazor.Mocks;
using MobileBlazor.Models;
using MobileBlazor.Utils;
using ModelLib.DTOs.CheckIns;
using MudBlazor.Services;
using RazorLib.AbstractClasses;
using RazorLib.Interfaces;
using RazorLib.Models;

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

            var doMockApiClient = true;
            if (doMockApiClient)
            {
                var mockedApiClient = new MockedApiClient();
                builder.Services.AddSingleton(mockedApiClient);
                builder.Services.AddSingleton<IMobileApiClient>(mockedApiClient);
                builder.Services.AddSingleton<IApiClient>(mockedApiClient);
            }
            else
            {
                builder.Services.AddScoped<HttpClient>();
                var apiClient = new ApiClient(new HttpClient(), "http://10.0.2.2:5078/"); // used on emulator device
#if WINDOWS
                    apiClient = new ApiClient(new HttpClient(), "http://localhost:5078/");
#endif
                builder.Services.AddSingleton(apiClient);
                builder.Services.AddSingleton<IMobileApiClient>(apiClient);
                builder.Services.AddSingleton<IApiClient>(apiClient);
            }

            builder.Services.AddSingleton<ITopicBroker, TopicBroker>();
            builder.Services.AddScoped<MapSearcher, MapSearcherClient>();
            builder.Services.AddMudServices();

            //Settings these below as singletons will crash the app with NullReferenceException errors when using JSRuntim
            builder.Services.AddScoped<ISessionStorage, SessionStorage>();

            builder.Services.AddScoped<IStorageManager<CurrentlyCheckedInDTO>, StorageManager<CurrentlyCheckedInDTO>>();

            return builder.Build();
        }
    }
}