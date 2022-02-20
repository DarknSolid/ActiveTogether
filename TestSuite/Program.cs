using EntityLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModelLib;
using NetTopologySuite.Geometries;
using WebApp.Entities;


var connectionString = "Host=localhost;Port=5555;Database=postgres;Username=postgres;Password=0206hampus;SslMode=Disable";
var services = new ServiceCollection();
services.AddDbContext<IApplicationDbContext,ApplicationDbContext>(options => options.UseNpgsql(connectionString));
services.AddScoped<IDogParkRepository, DogParkRepository>();

var serviceProvider = services.BuildServiceProvider();
var repo = serviceProvider.GetService<IDogParkRepository>();


var upperLeft = new Point(new Coordinate(1,2));
var lowerRight = new Point(new Coordinate(2, 1));

var result = await repo.GetInAreaAsync(upperLeft, lowerRight);
Console.WriteLine("done");
