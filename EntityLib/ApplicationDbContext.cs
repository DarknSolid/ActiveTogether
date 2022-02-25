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
        public DbSet<DogParkRating> DogParkRatings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("postgis");

            builder.Entity<DogParkFacility>()
                .Property(f => f.FacilityType)
                .HasConversion<int>();
            builder.Entity<DogParkFacility>()
                .HasKey(f => new { f.DogParkId, f.FacilityType });

            builder.Entity<DogParkRating>().HasKey(d => new { d.UserId, d.DogParkId });

            builder.Entity<DogPark>().HasMany(d => d.Ratings).WithOne(r => r.DogPark);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql(o => o.UseNetTopologySuite());
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync(default);
        }

    }
}