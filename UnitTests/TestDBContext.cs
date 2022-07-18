using EntityLib;
using EntityLib.Entities;
using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Entities;
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

            builder.Entity<Place>().HasData(
                CreatePlace(1, FacilityType.DogPark),
                CreatePlace(2, FacilityType.DogPark),
                CreatePlace(3, FacilityType.DogPark)
                );

            builder.Entity<DogPark>().HasData(
                CreateDogPark(1),
                CreateDogPark(2),
                CreateDogPark(3)
            );

            builder.Entity<DogBreed>().HasData(
                new DogBreed { Id = 1, Name = "a"},
                new DogBreed { Id = 2, Name = "b"},
                new DogBreed { Id = 3, Name = "c"}
            );

            builder.Entity<ApplicationUser>().HasData(
                CreateApplicationUser(1),
                CreateApplicationUser(2),
                CreateApplicationUser(3),
                CreateApplicationUser(4),
                CreateApplicationUser(5),
                CreateApplicationUser(6)
            );

            builder.Entity<Review>().HasData(
                CreateReview(1, 1),
                CreateReview(2, 1),
                CreateReview(1, 2)
            );

            builder.Entity<Dog>().HasData(
                CreateDog(1, 1),
                CreateDog(2, 2),
                CreateDog(3, 3),
                CreateDog(4, 4)
            );

            builder.Entity<CheckIn>().HasData(
                new CheckIn { Id = 1, CheckInDate = DateTime.UtcNow, PlaceId = 1, UserId = 1 },
                new CheckIn { Id = 2, CheckInDate = DateTime.UtcNow, PlaceId = 1, UserId = 2 },
                new CheckIn { Id = 3, CheckInDate = DateTime.UtcNow, PlaceId = 1, UserId = 5 },
                new CheckIn { Id = 4, CheckInDate = DateTime.UtcNow, CheckOutDate = DateTime.UtcNow, PlaceId = 1, UserId = 2 }
            );

            builder.Entity<DogCheckIn>().HasData(
                new DogCheckIn { CheckInId=1, DogId=1},
                new DogCheckIn { CheckInId=2, DogId=2}
                );
        }
        
        private Dog CreateDog(int id, int userId)
        {
            return new Dog
            {
                Id = id,
                UserId = userId,
                Birth = DateTime.UtcNow,
                Description = "",
                Name = "dog"+id,
                DogBreedId = 1,
                IsGenderMale = false,
                WeightClass = DogWeightClass.Medium,
            };
        }

        private Review CreateReview(int userId, int placeId)
        {
            return new Review
            {
                UserId = userId,
                PlaceId = placeId,
                Title = "Title",
                Description = "Description",
                Date = DateTime.Now,
                Rating = 3,
            };
        }

        private ApplicationUser CreateApplicationUser(int id)
        {
            return new ApplicationUser
            {
                Id = id,
                UserName = "test" + 1 + "user" + 1,
                PasswordHash = "",
                FirstName = "test" + id,
                LastName = "user" + id,
                ProfileImageUrl = ""
            };
        }

        private DogPark CreateDogPark(int placeId)
        {
            return new DogPark { PlaceId = placeId };
        }

        private Place CreatePlace(int id, FacilityType facilityType)
        {
            return new Place
            {
                Id = id,
                Name = "",
                Description = "",
                FacilityType = facilityType,
                Location = new Point(new Coordinate())
            };
        }


    }
}
