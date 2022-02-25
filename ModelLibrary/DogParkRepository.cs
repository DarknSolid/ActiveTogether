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

namespace ModelLib
{
    public interface IDogParkRepository
    {
        public Task<DogParkDetailedDTO> Get(int id);
        public Task<List<DogParkListDTO>> GetInAreaAsync(Point upperLeft, Point lowerRight);
        public Task<int> Create(DogParkCreateDTO dogParkCreateDTO);
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
                Location = new Point(new Coordinate(dogParkCreateDTO.Latitude, dogParkCreateDTO.Longitude))
            };
            _context.DogParks.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<DogParkDetailedDTO> Get(int id)
        {
            var entity = _context.DogParks
                .Include(p => p.Facilities)
                .Include(p => p.Ratings)
                .Where(x => x.Id == id)
                .Select(p => new DogParkDetailedDTO {
                    Rating = p.Ratings.Sum(r => r.Rating) / 5,
                    Description = p.Description,
                    Name = p.Name,
                    Facilities = p.Facilities,
                    Latitude = (float) p.Location.Y,
                    Longitude = (float) p.Location.X,
                });

            return await entity.FirstOrDefaultAsync();
        }

        public async Task<List<DogParkListDTO>> GetInAreaAsync(Point upperLeft, Point lowerRight)
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
    }
}
