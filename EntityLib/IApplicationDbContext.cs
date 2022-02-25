using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLib
{
    public interface IApplicationDbContext
    {
        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<DogParkFacility> DogParkFacilities { get; set; }
        public DbSet<DogParkRating> DogParkRatings { get; set; }

        public Task SaveChangesAsync();
    }
}
