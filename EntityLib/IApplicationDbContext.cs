using EntityLib.Entities;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntityLib
{
    public interface IApplicationDbContext
    {
        public DbSet<Place> Places { get; set; }
        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<DogBreed> DogBreeds { get; set; }

        public DbSet<Dog> Dogs { get; set; }

        public DbSet<ApplicationUser> Users {get;set;}

        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<DogCheckIn> DogCheckIns { get; set; }

        public DbSet<Friends> Friends { get; set; }

        public Task SaveChangesAsync();
    }
}
