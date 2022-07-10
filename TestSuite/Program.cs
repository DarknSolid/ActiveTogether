using EntityLib;
using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using NetTopologySuite.Geometries;
using TestSuite;
using WebApp.Entities;


var connectionString = "Host=localhost;Port=6666;Database=postgres;Username=postgres;Password=0206hampus;SslMode=Disable";
var services = new ServiceCollection();
services.AddDbContext<IApplicationDbContext,ApplicationDbContext>(options => 
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
services.AddScoped<IDogParkRepository, DogParkRepository>();
services.AddScoped<IReviewsRepository, ReviewsRepository>();

var serviceProvider = services.BuildServiceProvider();
var dogParkRepo = serviceProvider.GetService<IDogParkRepository>();
var reviewsRepo = serviceProvider.GetService<IReviewsRepository>();

var context = serviceProvider.GetService<IApplicationDbContext>();
var passwordHasher = new PasswordHasher<ApplicationUser>();

Console.WriteLine("Clearing Database Entities...");
context.Users.RemoveRange(context.Users);
context.Reviews.RemoveRange(context.Reviews);
context.DogParks.RemoveRange(context.DogParks);
await context.SaveChangesAsync();
Console.WriteLine("done");

var maxUsers = 3;
var maxDogParks = 400;
var testUserId = 1;

var devUser = Util.CreateDeveloperUser("developer_user@hotmail.com", "Test123!", passwordHasher);
context.Users.Add(devUser);

var dummyUsers = Util.CreateUsers(maxUsers);
context.Users.AddRange(dummyUsers);
await context.SaveChangesAsync();

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
                RevieweeId = id,
                ReviewType = EntityLib.Entities.Enums.ReviewType.DogPark,
                Title = $"Review {i + 1}:{j + 1}",
                Description = "some funky description right here",
                Rating = ((i + j) % 5) + 1
            });
        }
        progressBar.Report((double)i / maxDogParks);
    }
}


Console.WriteLine("done");


