using EntityLib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.Repositories
{
    public interface IPlacesRepository
    {
    }

    public class PlacesRepository : IPlacesRepository
    {

        private readonly IApplicationDbContext _context;

        public PlacesRepository(IApplicationDbContext context)
        {
            _context = context;
        }
    }
}
