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

        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<Review> DogParkReviews { get; set; }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DogPark>().HasData(
                CreateDogPark(1),
                CreateDogPark(2),
                CreateDogPark(3)
            );

            builder.Entity<ApplicationUser>().HasData(
                CreateApplicationUser(1),
                CreateApplicationUser(2)
            );

            builder.Entity<Review>().HasData(
                GenerateReview(1, ReviewType.DogPark, 1),
                GenerateReview(2, ReviewType.DogPark, 1),
                GenerateReview(1, ReviewType.DogPark, 2)
                );
        }

        private Review GenerateReview(int reviewerId, ReviewType reviewType, int revieweeId)
        {
            return new Review
            {
                ReviewerId = reviewerId,
                RevieweeId = revieweeId,
                ReviewType = reviewType,
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
                UserName = "test"+1 + "user"+1,
                PasswordHash = "",
                FirstName = "test"+id,
                LastName = "user"+id
            };
        }

        private DogPark CreateDogPark(int id)
        {
            return new DogPark { Id = id, Name = "", Description = "", Location = CreateLocation() };
        }

        private Point CreateLocation()
        {
            return new Point(new Coordinate());
        }


    }
}
