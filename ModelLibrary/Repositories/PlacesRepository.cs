using EntityLib;
using EntityLib.Entities;
using EntityLib.Entities.AbstractClasses;
using FisSst.BlazorMaps;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs;
using ModelLib.DTOs.AbstractDTOs;
using ModelLib.DTOs.Places;
using ModelLib.Utils;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;
using static ModelLib.Repositories.RepositoryEnums;
using Point = NetTopologySuite.Geometries.Point;

namespace ModelLib.Repositories
{
    public interface IPlacesRepository
    {
        public IQueryable<T> GetPlacesInArea<T>(IQueryable<T> query, SearchAreaDTO searchArea) 
            where T : EntityWithLocation;
        public Task<DistancePaginationResult<U>> GetPlacesByDistance<T,U>(IQueryable<T> query, Func<IQueryable<T>, IQueryable<U>> select, DistancePaginationRequest request) 
            where T : EntityWithLocation
            where U : DTOWithLocation;
        Task<(ResponseType, PlaceListDTO?)> GetPlaceAsync(int id);
    }

    public class PlacesRepository : RepositoryBase, IPlacesRepository
    {
        public PlacesRepository(IApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<T> GetPlacesInArea<T>(IQueryable<T> query, SearchAreaDTO searchArea) where T : EntityWithLocation
        {
            var bounds = Geographics.SphereToBounds(searchArea.Center, searchArea.RadiusKilometers * 1000);
            Coordinate[] boundingBox = new Coordinate[]
           { // first east/west then north/south due to north/south being the y coordinates
                new Coordinate(bounds.GetEast(), bounds.GetNorth()),
                new Coordinate(bounds.GetWest(), bounds.GetNorth()),
                new Coordinate(bounds.GetWest(), bounds.GetSouth()),
                new Coordinate(bounds.GetEast(), bounds.GetSouth()),
                // close the geometry:
                new Coordinate(bounds.GetEast(), bounds.GetNorth())
           };
            var bb = new GeometryFactory().CreatePolygon(boundingBox);

            var nearbyPlaces = query.Where(p => p.Location.Intersects(bb));

            return nearbyPlaces;
        }

        public async Task<DistancePaginationResult<U>> GetPlacesByDistance<T,U>(IQueryable<T> query, Func<IQueryable<T>, IQueryable<U>> select, DistancePaginationRequest request)
            where T : EntityWithLocation
            where U : DTOWithLocation
        {
            query = GetPlacesInArea(query, request.SearchArea);

            var point = new Point(x: request.SearchArea.Center.Lng, y: request.SearchArea.Center.Lat);

            var pagination = await RepositoryUtils.PaginateByDistanceAsync(query, select, request);

            return pagination;
        }

        public async Task<(ResponseType, PlaceListDTO?)> GetPlaceAsync(int id)
        {
            var entity = await _context.Places
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity is null)
            {
                return (ResponseType.NotFound, null);
            }

            return (ResponseType.Ok, new PlaceListDTO
            {
                Name = entity.Name,
                Facility = entity.FacilityType,
                Location = new(entity.Location.X, entity.Location.Y)
            });
        }
    }
}
