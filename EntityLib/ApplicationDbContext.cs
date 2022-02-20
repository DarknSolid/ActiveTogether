using EntityLib;
using EntityLib.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebApp.Entities
{
    public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
    {

        static ApplicationDbContext() 
        {
        }

        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }

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