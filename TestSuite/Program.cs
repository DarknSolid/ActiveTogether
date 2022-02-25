using EntityLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModelLib;
using ModelLib.DTOs.DogPark;
using NetTopologySuite.Geometries;
using TestSuite;
using WebApp.Entities;


var connectionString = "Host=localhost;Port=6666;Database=postgres;Username=postgres;Password=0206hampus;SslMode=Disable";
var services = new ServiceCollection();
services.AddDbContext<IApplicationDbContext,ApplicationDbContext>(options => options.UseNpgsql(connectionString));
services.AddScoped<IDogParkRepository, DogParkRepository>();

var serviceProvider = services.BuildServiceProvider();
var repo = serviceProvider.GetService<IDogParkRepository>();


for (int i = 0; i < 400; i++)
{
    var park = Util.GenerateDogPark();
    var id = await repo.Create(park);
    Console.WriteLine($"{i}");
}

Console.WriteLine("done");


