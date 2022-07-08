using EntityLib;
using ModelLib.DTOs.DogPark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Reviews;
using ModelLib.ApiDTOs.DogParks;

namespace ModelLib.Repositories
{
    public interface IDogParkRepository
    {
        public Task<DogParkDetailedDTO> Get(int id);
        public Task<List<DogParkListDTO>> GetInAreaAsync(BoundsDTO bounds);
        public Task<int> Create(DogParkCreateDTO dogParkCreateDTO);
        public Task<(PaginationResult, List<ReviewDetailedDTO>)> GetReviewsAsync(DogParkReviewsDTO request);
    }

    public class DogParkRepository : IDogParkRepository
    {
        private readonly IApplicationDbContext _context;

        public DogParkRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(DogParkCreateDTO dogParkCreateDTO)
        {
            //TODO check for duplicate parks
            var entity = new DogPark
            {
                Name = dogParkCreateDTO.Name,
                Description = dogParkCreateDTO.Description,
                Facilities = dogParkCreateDTO.Facilities,
                Location = new Point(new Coordinate(dogParkCreateDTO.Longitude, dogParkCreateDTO.Latitude))
            };
            _context.DogParks.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<DogParkDetailedDTO> Get(int id)
        {
            var entityQuery = _context.DogParks
                .Include(p => p.Facilities)
                .Include(p => p.Ratings)
                .Where(x => x.Id == id)
                .Select(p => new DogParkDetailedDTO
                {
                    Rating = p.Ratings.Sum(r => r.Rating) / 5,
                    Description = p.Description,
                    Name = p.Name,
                    Facilities = p.Facilities.Select(f => f.FacilityType),
                    Latitude = (float)p.Location.Y,
                    Longitude = (float)p.Location.X,
                });

            return await entityQuery.FirstOrDefaultAsync();
        }

        public async Task<List<DogParkListDTO>> GetInAreaAsync(BoundsDTO bounds)
        {
            Coordinate[] boundingBox = new Coordinate[]
            { // first east/west then north/south due to north/south being the y coordinates
                new Coordinate(bounds.East, bounds.North),
                new Coordinate(bounds.West, bounds.North),
                new Coordinate(bounds.West, bounds.South),
                new Coordinate(bounds.East, bounds.South),
                // close the geometry:
                new Coordinate(bounds.East, bounds.North)
            };
            var bb = new GeometryFactory().CreatePolygon(boundingBox);

            var nearbyEntities = await _context.DogParks.Where(c => c.Location.Intersects(bb)).Select(p => p).ToListAsync();
            var nearbyDogParks = nearbyEntities.Select(p => new DogParkListDTO
            {
                Id = p.Id,
                Name = p.Name,
                Latitude = (float)p.Location.Y,
                Longitude = (float)p.Location.X
            }).ToList();
            return nearbyDogParks;
        }

        public async Task<(PaginationResult, List<ReviewDetailedDTO>)> GetReviewsAsync(DogParkReviewsDTO request)
        {
            var query = _context.DogParkRatings.Where(r => r.DogParkId == request.DogParkId).OrderByDescending(r => r.Date);
            var (paginationResult, paginatedQuery) = await RepositoryUtils.GetPaginationQuery(query, request.PaginationRequest);
            var dtoResult = await paginatedQuery.Select(r => new ReviewDetailedDTO
            {
                AuthorFirstName = r.User.FirstName,
                AuthorLastName = r.User.LastName,
                Comment = r.Description,
                CreatorId = r.UserId,
                Date = r.Date,
                DogParkId = r.DogParkId,
                Rating = r.Rating,
                Title = r.Title
            })
                .ToListAsync();


            return (paginationResult, dtoResult);
        }
    }
}
