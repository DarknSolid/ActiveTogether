using EntityLib;
using EntityLib.Entities;
using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebApp.Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext
    {

        static ApplicationDbContext() 
        {
        }

        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DogBreed> DogBreeds { get; set; }
        public DbSet<Dog> Dogs { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("postgis");

            #region DogParkFacility
            builder.Entity<DogParkFacility>()
                .Property(f => f.FacilityType)
                .HasConversion<int>();
            builder.Entity<DogParkFacility>()
                .HasKey(f => new { f.DogParkId, f.FacilityType });
            #endregion

            #region Review
            builder.Entity<Review>()
                .Property(r => r.ReviewType)
                .HasConversion<int>();
            builder.Entity<Review>()
                .HasKey(d => new { d.ReviewerId, d.RevieweeId, d.ReviewType });
            #endregion

            #region DogPark

            #endregion

            base.OnModelCreating(builder);
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync(default);
        }

    }
}