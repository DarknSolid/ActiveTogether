using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using DatabaseSeeding;
using DatabaseSeeding.Dataextractors;
using DatabaseSeeding.Dataextractors.Dogparks;
using Entities;
using EntityLib;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModelLib.Repositories;
using ModelLib.Utils;

var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

var connectionString = "";
switch (EnvironmentHandler.GetEnvironment(config))
{
    case EnvironmentHandler.Environment.Local:
        connectionString = config["POSTGRESQLCONNSTR_LOCAL"];
        break;
    case EnvironmentHandler.Environment.Development:
        connectionString = config["POSTGRESQLCONNSTR_AZURE_DEVELOP"];
        break;
}
var osmFilePath = config["OsmFilePath"];
var saveFolderPath = config["OsmSaveFolderPath"];
var saveFilePath = config["OsmSaveFilePath"];

var services = new ServiceCollection();

services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options => 
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
services.AddDefaultIdentity<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
services.AddScoped<IDogParkRepository, DogParkRepository>();
services.AddScoped<IReviewsRepository, ReviewsRepository>();
services.AddScoped<IDogRepository, DogRepository>();
services.AddScoped<ICheckInRepository, CheckInRepository>();
services.AddScoped<IPlacesRepository, PlacesRepository>();
services.AddScoped<DatabaseSeeder>();
services.AddSingleton(options =>
{
    var client = new Uri(config["AZURE_STORAGE_ACCOUNT_DEV"]);
    return new DataLakeServiceClient(client, new DefaultAzureCredential());
});

services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>();


var serviceProvider = services.BuildServiceProvider();

var openStreetMapExtractor = new OpenStreetMapExtractor();
//var ways = openStreetMapExtractor.Run(osmFilePath, saveFolderPath);
var ways = openStreetMapExtractor.LoadPreviousExtraction(saveFilePath);

var dogParkCreateDTOs = DTOConverter.ConvertToDogParkCreateDTO(ways);

// take all of the first x bottom right parks:
dogParkCreateDTOs = dogParkCreateDTOs
    .OrderByDescending(dto => dto.Point.X)
    .ThenByDescending(dto => dto.Point.Y)
    .Take(300)
    .ToList();

var seeder = serviceProvider.GetRequiredService<DatabaseSeeder>();
await seeder.SeedDatabase(dogParkCreateDTOs);
