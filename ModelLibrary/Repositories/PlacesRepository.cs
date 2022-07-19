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

    public class PlacesRepository : RepositoryBase, IPlacesRepository
    {
        public PlacesRepository(IApplicationDbContext context) : base(context)
        {
        }
    }
}
