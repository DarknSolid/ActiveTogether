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
    public interface IFacilityRepository
    {
        public Task<bool> FacilityExists(int facilityId, FacilityType facilityType);
    }

    public class FacilityRepository : IFacilityRepository
    {

        private readonly IApplicationDbContext _context;

        public FacilityRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FacilityExists(int facilityId, FacilityType facilityType)
        {
            IQueryable<SimplePrimaryKey> query;
            switch (facilityType)
{
                case FacilityType.DogPark:
                    query = _context.DogParks;
                    break;
                default:
                    throw new NotImplementedException($"Review type: {facilityType}");
            }

            return await query.FirstOrDefaultAsync(x => x.Id == facilityId) != null;
        }
    }
}
