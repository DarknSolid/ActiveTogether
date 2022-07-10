using EntityLib.Entities;
using EntityLib.Entities.Identity;
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
        public DbSet<Review> Reviews { get; set; }

        public DbSet<ApplicationUser> Users {get;set;}

        public Task SaveChangesAsync();
    }
}
