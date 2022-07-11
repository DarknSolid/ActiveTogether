using EntityLib.Entities;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntityLib
{
    public interface IApplicationDbContext
    {
        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<DogBreed> DogRaces { get; set; }

        public DbSet<Dog> Dogs { get; set; }

        public DbSet<ApplicationUser> Users {get;set;}

        public Task SaveChangesAsync();
    }
}
