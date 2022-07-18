using EntityLib;
using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModelLib.DTOs.Dogs;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using TestSuite;
using WebApp.Entities;
using static EntityLib.Entities.Enums;

var connectionString = "Host=localhost;Port=6666;Database=postgres;Username=postgres;Password=0206hampus;SslMode=Disable";
var services = new ServiceCollection();
services.AddDbContext<IApplicationDbContext,ApplicationDbContext>(options => 
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
services.AddScoped<IDogParkRepository, DogParkRepository>();
services.AddScoped<IReviewsRepository, ReviewsRepository>();
services.AddScoped<IDogRepository, DogRepository>();
services.AddScoped<ICheckInRepository, CheckInRepository>();
services.AddScoped<IPlacesRepository, PlacesRepository>();

var serviceProvider = services.BuildServiceProvider();
var dogParkRepo = serviceProvider.GetService<IDogParkRepository>();
var reviewsRepo = serviceProvider.GetService<IReviewsRepository>();
var dogRepository = serviceProvider.GetService<IDogRepository>();

var context = serviceProvider.GetService<IApplicationDbContext>();
var passwordHasher = new PasswordHasher<ApplicationUser>();

Console.WriteLine("Clearing Database Entities...");
context.Dogs.RemoveRange(context.Dogs);
context.DogBreeds.RemoveRange(context.DogBreeds);
context.Reviews.RemoveRange(context.Reviews);
context.DogParks.RemoveRange(context.DogParks);
context.Places.RemoveRange(context.Places);
context.Users.RemoveRange(context.Users);
await context.SaveChangesAsync();
Console.WriteLine("done");

var maxUsers = 3;
var maxDogParks = 400;
var testUserId = 1;

Console.WriteLine("Creating Users");
var devUser = Util.CreateDeveloperUser("developer_user@hotmail.com", "Test123!", passwordHasher);
context.Users.Add(devUser);

var dummyUsers = Util.CreateUsers(maxUsers);
context.Users.AddRange(dummyUsers);
await context.SaveChangesAsync();
Console.WriteLine("done");

Console.WriteLine("Creating Dogs and Dog Breeds");
List<int> dogBreeds = new();
foreach(string breed in Util.CreateDogBreeds())
{
    var id = await dogRepository.CreateDogBreedAsync(breed);
    dogBreeds.Add(id);
}
var weightClasses = Enum.GetValues<DogWeightClass>();
foreach(var user in await context.Users.ToListAsync())
{
    await dogRepository.CreateAsync(user.Id, new DogCreateDTO
    {
        Name = "DOggie"+user.Id,
        Description = "My cute dog",
        Birth = DateTime.UtcNow,
        Breed = dogBreeds[user.Id%dogBreeds.Count],
        IsGenderMale = user.Id % 2 == 0,
        WeightClass = weightClasses[user.Id % weightClasses.Length]
    });
}
Console.WriteLine("done");

Console.WriteLine("Creating DogParks with reviews...");
using (var progressBar = new ProgressBar())
{
    for (int i = 0; i < maxDogParks; i++)
    {
        var park = Util.GenerateDogPark();
        var id = await dogParkRepo.CreateAsync(park);

        for (int j = 0; j < maxUsers; j++)
        {
            var userId = dummyUsers[j].Id;
            await reviewsRepo.CreateReviewAsync(userId, new ReviewCreateDTO
            {
                PlaceId = id,
                Title = $"Review {i + 1}:{j + 1}",
                Description = "some funky description right here",
                Rating = ((i + j) % 5) + 1
            });
        }
        progressBar.Report((double)i / maxDogParks);
    }
}


Console.WriteLine("done");


