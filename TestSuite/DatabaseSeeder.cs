using EntityLib.Entities.Identity;
using EntityLib;
using Microsoft.AspNetCore.Identity;
using ModelLib.DTOs.Dogs;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using static EntityLib.Entities.Enums;
using TestSuite;
using Microsoft.EntityFrameworkCore;
using ModelLib.DTOs.CheckIns;
using ModelLib.DTOs.DogPark;

namespace DatabaseSeeding
{
    public class DatabaseSeeder
    {
        private readonly IDogParkRepository _dogParkRepository;
        private readonly IReviewsRepository _reviewsRepository;
        private readonly IDogRepository _dogRepository;
        private readonly ICheckInRepository _checkInRepository;
        private readonly IApplicationDbContext _applicationDbContext;

        public DatabaseSeeder(
            IDogParkRepository dogParkRepository,
            IReviewsRepository reviewsRepository,
            IDogRepository dogRepository,
            ICheckInRepository checkInRepository,
            IApplicationDbContext applicationDbContext)
        {
            _dogParkRepository = dogParkRepository;
            _reviewsRepository = reviewsRepository;
            _dogRepository = dogRepository;
            _checkInRepository = checkInRepository;
            _applicationDbContext = applicationDbContext;
        }

        public async Task SeedDatabase(List<DogParkCreateDTO> dogParkCreateDTOs)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            Console.WriteLine("Clearing Database Entities...");
            //await RoleManager.Roles.ForEachAsync(async (r) => await RoleManager.DeleteAsync(r));
            _applicationDbContext.Dogs.RemoveRange(_applicationDbContext.Dogs);
            _applicationDbContext.Reviews.RemoveRange(_applicationDbContext.Reviews);
            _applicationDbContext.DogParks.RemoveRange(_applicationDbContext.DogParks);
            _applicationDbContext.Places.RemoveRange(_applicationDbContext.Places);
            _applicationDbContext.Users.RemoveRange(_applicationDbContext.Users);
            await _applicationDbContext.SaveChangesAsync();
            Console.WriteLine("done");

            var maxUsers = 3;
            var maxDogParks = 400;

            Console.WriteLine("Creating Users");
            var devUser = Util.CreateDeveloperUser("developer_user@hotmail.com", "Test123!", passwordHasher);
            _applicationDbContext.Users.Add(devUser);

            var dummyUsers = Util.CreateUsers(maxUsers);
            _applicationDbContext.Users.AddRange(dummyUsers);
            await _applicationDbContext.SaveChangesAsync();

            //await RoleManager.CreateAsync(new IdentityRole(RoleConstants.ADMIN));
            //await UserManager.AddToRoleAsync(devUser, RoleConstants.ADMIN);
            //Console.WriteLine($"Gave {devUser.UserName} the \"{RoleConstants.ADMIN}\" role");
            Console.WriteLine("done");

            Console.WriteLine("Creating Dogs");
            List<int> dogBreeds = new();
            var weightClasses = Enum.GetValues<DogWeightClass>();
            foreach (var user in await _applicationDbContext.Users.ToListAsync())
            {
                await _dogRepository.CreateAsync(user.Id, new DogCreateDTO
                {
                    Name = "DOggie" + user.Id,
                    Description = "My cute dog",
                    Birth = DateTime.UtcNow,
                    Race = Enum.GetValues<DogRace>()[user.Id % Enum.GetValues<DogRace>().Length],
                    IsGenderMale = user.Id % 2 == 0,
                    WeightClass = weightClasses[user.Id % weightClasses.Length]
                });
            }
            Console.WriteLine("done");

            Console.WriteLine($"Creating {dogParkCreateDTOs.Count} DogParks with check-ins and reviews...");
            using (var progressBar = new ProgressBar())
            {
                var moods = Enum.GetValues<CheckInMood>();
                for (int i = 0; i < dogParkCreateDTOs.Count; i++)
                {
                    var park = dogParkCreateDTOs[i];
                    var (_, id) = await _dogParkRepository.CreateAsync(park);

                    for (int j = 0; j < maxUsers; j++)
                    {
                        var userId = dummyUsers[j].Id;
                        await _checkInRepository.CheckIn(userId, new CheckInCreateDTO
                        {
                            DogsToCheckIn = new(),
                            Mood = moods[userId % moods.Length],
                            PlaceId = id
                        });

                        await _checkInRepository.CheckOut(userId);

                        await _reviewsRepository.CreateReviewAsync(userId, new ReviewCreateDTO
                        {
                            Id = id,
                            Title = $"Review {i + 1}:{j + 1}",
                            Description = "some funky description right here",
                            Rating = ((i + j) % 5) + 1
                        });
                    }
                    progressBar.Report((double)i / dogParkCreateDTOs.Count);
                }
            }
            Console.WriteLine("done");
        }
    }
}
