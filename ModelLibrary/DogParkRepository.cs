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

namespace ModelLib
{
    public interface IDogParkRepository
    {
        public Task<List<DogParkDTO>> GetInAreaAsync(Point upperLeft, Point lowerRight);
        public Task Create(DogParkCreateDTO dogParkCreateDTO);
    }

    public class DogParkRepository : IDogParkRepository
    {
        private readonly IApplicationDbContext _context;

        public DogParkRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(DogParkCreateDTO dogParkCreateDTO)
        {
            //TODO check for duplicate parks
            var entity = new DogPark
            {
                Name = dogParkCreateDTO.Name,
                Description = dogParkCreateDTO.Description,
                Facilities = dogParkCreateDTO.Facilities,
                Location = new Point(new Coordinate(dogParkCreateDTO.Latitude, dogParkCreateDTO.Longitude))
            };
            _context.DogParks.Add(entity);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<List<DogParkDTO>> GetInAreaAsync(Point upperLeft, Point lowerRight)
        {
            Coordinate[] boundingBox = new Coordinate[]
            {
                new Coordinate (upperLeft.X, upperLeft.Y),
                new Coordinate (lowerRight.X, upperLeft.Y),
                new Coordinate(lowerRight.X, lowerRight.Y),
                new Coordinate(upperLeft.X, lowerRight.Y),
                // close the geometry:
                new Coordinate (upperLeft.X, upperLeft.Y)
            };
            var bb = new GeometryFactory().CreatePolygon(boundingBox);

            var nearbyEntities = _context.DogParks.Where(c => c.Location.Intersects(bb)).Select(p => p);
            var nearbyDogParks = nearbyEntities.Select(p => new DogParkDTO
            {
                Name = p.Name,
                Latitude = (float)p.Location.X,
                Longitude = (float)p.Location.Y
            }).ToList();
            return nearbyDogParks;
        }
    }
}
