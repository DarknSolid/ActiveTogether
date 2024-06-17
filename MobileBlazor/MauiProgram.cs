using FisSst.BlazorMaps;
using FisSst.BlazorMaps.DependencyInjection;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.CheckIns;
using MudBlazor;
using MudBlazor.Services;
using RazorLib.Interfaces;
using RazorLib.Models;
using RazorLib.Models.MapSearch;

namespace MobileBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            // ============= ENVIRONMENT VARIABLES =============
            var useDevelopmentApi = false;
            // ============= ===================== =============

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

            builder.Services.AddScoped(options =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://10.0.2.2:7015/");
                if (useDevelopmentApi)
                {
                    client.BaseAddress = new Uri("https://doggoworld-dev-web-app.azurewebsites.net/"); // azure development 
                }
                return client;
            });
            builder.Services.AddScoped<IApiClient, ApiClient>();

            builder.Services.AddBlazorLeafletMaps();
            builder.Services.AddSingleton<ITopicBroker, TopicBroker>();
            builder.Services.AddScoped<InstructorMapSearcher>();
            builder.Services.AddScoped<DogParkMapSearcher>();
            builder.Services.AddMudServices(config =>
                {
                    //Configure Snackbar settings / notification messages
                    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                    config.SnackbarConfiguration.PreventDuplicates = false;
                    config.SnackbarConfiguration.NewestOnTop = false;
                    config.SnackbarConfiguration.ShowCloseIcon = true;
                    config.SnackbarConfiguration.VisibleStateDuration = 10000;
                    config.SnackbarConfiguration.HideTransitionDuration = 500;
                    config.SnackbarConfiguration.ShowTransitionDuration = 500;
                    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                }
            );

            //Settings these below as singletons will crash the app with NullReferenceException errors when using JSRuntim
            builder.Services.AddScoped<ISessionStorage, SessionStorage>();

            builder.Services.AddScoped<IStorageManager<CurrentlyCheckedInDTO>, StorageManager<CurrentlyCheckedInDTO>>();
            builder.Services.AddScoped<IStorageManager<MapOptions>, StorageManager<MapOptions>>();
            builder.Services.AddScoped<IStorageManager<UserDetailedDTO>, StorageManager<UserDetailedDTO>>();
            builder.Services.AddScoped<IStorageManager<SearchAreaDTO>, StorageManager<SearchAreaDTO>>();

            return builder.Build();
        }
    }
}