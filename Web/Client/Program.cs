using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Web.Client;
using FisSst.BlazorMaps.DependencyInjection;
using FisSst.BlazorMaps;
using MudBlazor;
using RazorLib.Models;
using RazorLib.Models.MapSearch;
using RazorLib.Interfaces;
using ModelLib.DTOs.CheckIns;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using ModelLib.Repositories;

// ============= ENVIRONMENT VARIABLES =============
var useDevelopmentApi = false;
// ============= ===================== =============

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// services:
builder.Services.AddScoped(options =>
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7015/");
    if (useDevelopmentApi)
    {
        httpClient.BaseAddress = new Uri("https://doggoworld-dev-web-app.azurewebsites.net/");
    }
    return httpClient;
});

builder.Services.AddScoped<IApiClient, ApiClient>();

builder.Services.AddSingleton<ITopicBroker, TopicBroker>();
builder.Services.AddScoped<InstructorMapSearcher>();
builder.Services.AddScoped<DogParkMapSearcher>();

//Settings these below as singletons will crash the app with NullReferenceException errors when using JSRuntim
builder.Services.AddScoped<ISessionStorage, SessionStorage>();

builder.Services.AddScoped<IStorageManager<CurrentlyCheckedInDTO>, StorageManager<CurrentlyCheckedInDTO>>();
builder.Services.AddScoped<IStorageManager<MapOptions>, StorageManager<MapOptions>>();
builder.Services.AddScoped<IStorageManager<UserDetailedDTO>, StorageManager<UserDetailedDTO>>();
builder.Services.AddScoped<IStorageManager<SearchAreaDTO>, StorageManager<SearchAreaDTO>>();
builder.Services.AddScoped<CloudClient>();

// third party services:
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
});
builder.Services.AddBlazorLeafletMaps();

await builder.Build().RunAsync();
