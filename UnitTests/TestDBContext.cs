using Entities;
using EntityLib.Entities;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EntityLib.Entities.Enums;

namespace UnitTests
{
    public class TestDBContext : ApplicationDbContext
    {
        public TestDBContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DogPark>().Ignore(d => d.Facilities);

            builder.Entity<PendingDogPark>().Ignore(d => d.Facilities);

            builder.Entity<DogTraining>().Ignore(d => d.TrainingTimes);

            builder.Entity<Place>().HasData(
                CreatePlace(1, PlaceType.DogPark),
                CreatePlace(2, PlaceType.DogPark),
                CreatePlace(3, PlaceType.DogPark),
                CreatePlace(4, PlaceType.DogInstructor)
            );

            builder.Entity<Company>().HasData(
                new Company
                {
                    ApplicationUserId = 1,
                    Email = "",
                    Phone = "",
                    PlaceId = 4,
                }
            );

            builder.Entity<InstructorCompanyCategory>().HasData(
                CreateInstructorCompanyCategory(1, InstructorCategory.Agility),
                CreateInstructorCompanyCategory(4, InstructorCategory.Agility)
            );

            builder.Entity<InstructorCompanyFacility>().HasData(
                CreateInstructorCompanyFacility(1, InstructorFacility.Indoor),
                CreateInstructorCompanyFacility(4, InstructorFacility.Indoor)
            );

            builder.Entity<DogPark>().HasData(
                CreateDogPark(1, 1, DateTime.UtcNow),
                CreateDogPark(2, 1, DateTime.UtcNow.AddHours(-1)),
                CreateDogPark(3)
            );

            builder.Entity<ApplicationUser>().HasData(
                CreateApplicationUser(1),
                CreateApplicationUser(2),
                CreateApplicationUser(3),
                CreateApplicationUser(4),
                CreateApplicationUser(5),
                CreateApplicationUser(6),
                CreateApplicationUser(7, "AB"),
                CreateApplicationUser(8, "AB"),
                CreateApplicationUser(9, "ABC"),
                CreateApplicationUser(10, "A")
            );

            builder.Entity<Review>().HasData(
                CreateReview(1, 1),
                CreateReview(2, 1),
                CreateReview(1, 2)
            );

            builder.Entity<CheckIn>().HasData(
                new CheckIn { Id = 1, CheckInDate = DateTime.UtcNow, PlaceId = 1, UserId = 1, Mood = Enums.CheckInMood.Social },
                new CheckIn { Id = 2, CheckInDate = DateTime.UtcNow, PlaceId = 1, UserId = 2, Mood = Enums.CheckInMood.Social },
                new CheckIn { Id = 3, CheckInDate = DateTime.UtcNow, PlaceId = 1, UserId = 5, Mood = Enums.CheckInMood.Social },
                new CheckIn { Id = 4, CheckInDate = DateTime.UtcNow, CheckOutDate = DateTime.UtcNow, PlaceId = 1, UserId = 2, Mood = Enums.CheckInMood.Social },
                new CheckIn { Id = 5, CheckInDate = DateTime.UtcNow, CheckOutDate = DateTime.UtcNow, PlaceId = 2, UserId = 2, Mood = Enums.CheckInMood.Social }
            );

            builder.Entity<Friendship>().HasData(
                new Friendship { RequesterId = 3, RequesteeId = 1, IsAccepted = false },
                new Friendship { RequesterId = 4, RequesteeId = 1, IsAccepted = true },
                new Friendship { RequesterId = 1, RequesteeId = 5, IsAccepted = true }
                );

            builder.Entity<PendingDogPark>().HasData(
                CreatePendingDogPark(1, 1, DateTime.UtcNow),
                CreatePendingDogPark(2, 1, DateTime.UtcNow.AddHours(-1))
            );
        }

        private InstructorCompanyCategory CreateInstructorCompanyCategory(int companyId, InstructorCategory category)
        {
            return new InstructorCompanyCategory { InstructorCompanyId = companyId, InstructorCategory = category };
        }

        private InstructorCompanyFacility CreateInstructorCompanyFacility(int companyId, InstructorFacility facility)
        {
            return new InstructorCompanyFacility { InstructorCompanyId = companyId, InstructorFacility = facility };
        }

        private Review CreateReview(int userId, int placeId)
        {
            return new Review
            {
                UserId = userId,
                PlaceId = placeId,
                Title = "Title",
                Description = "Description",
                DateTime = DateTime.Now,
                Rating = 3,
            };
        }

        private ApplicationUser CreateApplicationUser(int id, string? fullName = null)
        {
            return new ApplicationUser
            {
                Id = id,
                UserName = "test" + id + "user" + id,
                PasswordHash = "",
                FirstName = "test" + id,
                LastName = "user" + id,
                ProfileImageUrl = "",
                FullNameNormalized = fullName ?? "test" + id + " " + "user" + id
            };
        }

        private DogPark CreateDogPark(int placeId, int? authorId = null, DateTime? dateAdded = null)
        {
            return new DogPark { PlaceId = placeId, SquareKilometers = 1, AuthorId = authorId, DateAdded = dateAdded ?? DateTime.UtcNow };
        }

        private Place CreatePlace(int id, PlaceType facilityType)
        {
            return new Place
            {
                Id = id,
                Name = "",
                Description = "",
                FacilityType = facilityType,
                Location = new Point(new Coordinate(0, 0))
            };
        }

        private PendingDogPark CreatePendingDogPark(int id, int requesterId, DateTime requestDate)
        {
            return new PendingDogPark
            {
                Id = id,
                RequesterId = requesterId,
                Name = "",
                Description = "",
                FacilityType = PlaceType.DogPark,
                Location = new Point(new Coordinate(0, 0)),
                RequestDate = requestDate,
                SquareKilometers = 1
            };

        }


    }
}
