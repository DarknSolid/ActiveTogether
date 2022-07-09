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

namespace UnitTests
{
    public class TestDBContext : ApplicationDbContext
    {
        public TestDBContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<DogParkRating> DogParkRatings { get; set; }

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
