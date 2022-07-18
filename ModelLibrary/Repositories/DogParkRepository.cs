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

namespace ModelLib.Repositories
{
    public interface IDogParkRepository
    {
        public Task<DogParkDetailedDTO?> GetAsync(int userId, int id);
        public Task<List<DogParkListDTO>> GetInAreaAsync(BoundsDTO bounds);
        public Task<int> CreateAsync(DogParkCreateDTO dogParkCreateDTO);
    }

    public class DogParkRepository : IDogParkRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly ICheckInRepository _checkInRepository;

        public DogParkRepository(IApplicationDbContext context, ICheckInRepository checkInRepository)
        {
            _context = context;
            _checkInRepository = checkInRepository;
        }

        public async Task<int> CreateAsync(DogParkCreateDTO dogParkCreateDTO)
        {
            //TODO check for duplicate parks
            var placeEntity = new Place
            {
                Name = dogParkCreateDTO.Name,
                Description = dogParkCreateDTO.Description,
                Location = new Point(new Coordinate(dogParkCreateDTO.Longitude, dogParkCreateDTO.Latitude))
            };


            _context.Places.Add(placeEntity);
            await _context.SaveChangesAsync();

            var dogparkEntity = new DogPark
            {
                PlaceId = placeEntity.Id,
                Facilities = dogParkCreateDTO.Facilities,
            };
            _context.DogParks.Add(dogparkEntity);
            await _context.SaveChangesAsync();
            return dogparkEntity.PlaceId;
        }

        public async Task<DogParkDetailedDTO?> GetAsync(int userId, int id)
        {
            var reviews = _context.Reviews.Where(r => r.PlaceId == id);

            var reviewStatus = DogParkDetailedDTO.ReviewStatus.MustCheckIn;
            if (await reviews.AnyAsync(r => r.UserId == userId)) {
                reviewStatus = DogParkDetailedDTO.ReviewStatus.CanUpdateReview;
            }
            else if (await _checkInRepository.HasCheckedOutBeforeAsync(userId, id))
            {
                reviewStatus = DogParkDetailedDTO.ReviewStatus.CanReview;
            }

            var entityQuery = _context.DogParks
                .Include(p => p.Facilities)
                .Include(p => p.Place)
                .Where(x => x.PlaceId == id)
                .Select(p => new DogParkDetailedDTO
                {
                    Rating = reviews
                        .Sum(r => r.Rating) / reviews.Count(),
                    Description = p.Place.Description,
                    Name = p.Place.Name,
                    Facilities = p.Facilities.Select(f => f.FacilityType),
                    Latitude = (float)p.Place.Location.Y,
                    Longitude = (float)p.Place.Location.X,
                    TotalReviews = reviews.Count(),
                    Id = id,
                    CurrentReviewStatus = reviewStatus
                    
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

            var nearbyEntities = await _context.DogParks.Include(p => p.Place).Where(c => c.Place.Location.Intersects(bb)).Select(p => p).ToListAsync();
            var nearbyDogParks = nearbyEntities.Select(p => new DogParkListDTO
            {
                Id = p.PlaceId,
                Name = p.Place.Name,
                Latitude = (float)p.Place.Location.Y,
                Longitude = (float)p.Place.Location.X
            }).ToList();
            return nearbyDogParks;
        }
    }
}
