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
        public DbSet<Place> Places { get; set; }
        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DogBreed> DogBreeds { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<DogCheckIn> DogCheckIns { get; set; }


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
                .HasKey(d => new { d.UserId, d.PlaceId });

            builder.Entity<Review>()
                .HasOne(r => r.Place)
                .WithMany(u => u.Reviews);
            #endregion

            #region DogPark

            #endregion

            #region CheckIn
            builder.Entity<CheckIn>()
                .HasMany(c => c.DogCheckIns)
                .WithOne(dc => dc.CheckIn);
            builder.Entity<CheckIn>()
                .HasOne(c => c.User)
                .WithMany(u => u.CheckIns);
            #endregion

            #region DogCheckIn
            builder.Entity<DogCheckIn>()
                .HasKey(d => new { d.DogId, d.CheckInId });
            builder.Entity<DogCheckIn>()
                .HasOne(dc => dc.Dog).WithMany(d => d.CheckIns);
            #endregion

            base.OnModelCreating(builder);
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync(default);
        }

    }
}