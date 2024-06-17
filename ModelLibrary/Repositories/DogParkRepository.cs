using EntityLib;
using ModelLib.DTOs.DogPark;
using NetTopologySuite.Geometries;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using NpgsqlTypes;
using static ModelLib.Repositories.RepositoryEnums;
using ModelLib.ApiDTOs.Pagination;
using static EntityLib.Entities.Enums;
using ModelLib.DTOs.Instructors;
using ModelLib.DTOs;
using static ModelLib.DTOEnums;

namespace ModelLib.Repositories
{
    public interface IDogParkRepository
    {
        public Task<DogParkDetailedDTO?> GetAsync(int? userId, int id);
        public Task<(ResponseType, int)> CreateAsync(DogParkCreateDTO dogParkCreateDTO, int? authorId = null);
        public Task<(ResponseType, int)> RequestAsync(int requesterId, DogParkRequestCreateDTO dogParkRequestDTO);
        public Task<(ResponseType, int)> ApproveDogParkRequestAsync(int dogParkRequestId);
        public Task<DateTimePaginationResult<DogParkRequestDetailedDTO>> GetRequestsAsync(int requesterId, DateTimePaginationRequest paginationRequest);
        public Task<DateTimePaginationResult<DogParkListDTO>> GetCreatedByAuthorAsync(int authorId, DateTimePaginationRequest paginationRequest);
        public Task<DistancePaginationResult<DogParkListDTO>> GetParksAsync(DogParksDTOPaginationRequest request);
        public Task<IEnumerable<DogParkListDTO>> GetParksInAreaAsync(SearchAreaDTO bounds);
    }

    public class DogParkRepository : RepositoryBase, IDogParkRepository
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IPlacesRepository _placesRepository;

        public DogParkRepository(IApplicationDbContext context, ICheckInRepository checkInRepository, IPlacesRepository placesRepository) : base(context)
        {
            _checkInRepository = checkInRepository;
            _placesRepository = placesRepository;
        }

        public async Task<(ResponseType, int)> ApproveDogParkRequestAsync(int dogParkRequestId)
        {
            var entity = await _context.PendingDogParks.FirstOrDefaultAsync(p => p.Id == dogParkRequestId);
            if (entity is null)
            {
                return (ResponseType.NotFound, -1);
            }

            var dto = new DogParkCreateDTO()
            {
                Bounds = entity.Bounds,
                Description = entity.Description,
                Facilities = entity.Facilities ?? new List<DogParkFacilityType>(),
                Name = entity.Name,
                Point = new NpgsqlPoint(entity.Location.X, entity.Location.Y),
                SquareKilometers = entity.SquareKilometers
            };

            var (createResponse, createId) = await CreateAsync(authorId: entity.RequesterId, dogParkCreateDTO: dto);
            if (createResponse != ResponseType.Created)
            {
                return (ResponseType.Conflict, -1);
            }

            _context.PendingDogParks.Remove(entity);
            await _context.SaveChangesAsync();
            return (createResponse, createId);
        }

        public async Task<(ResponseType, int)> CreateAsync(DogParkCreateDTO dogParkCreateDTO, int? authorId = null)
        {
            //TODO check for duplicate parks
            var placeEntity = new Place
            {
                Name = dogParkCreateDTO.Name,
                Description = dogParkCreateDTO.Description,
                Location = new Point(x: dogParkCreateDTO.Point.X, y: dogParkCreateDTO.Point.Y)
            };

            _context.Places.Add(placeEntity);
            await _context.SaveChangesAsync();

            var dogparkEntity = new DogPark
            {
                PlaceId = placeEntity.Id,
                Facilities = dogParkCreateDTO.Facilities.ToList(),
                Bounds = dogParkCreateDTO.Bounds is null ? null : new NpgsqlPolygon(dogParkCreateDTO.Bounds),
                SquareKilometers = dogParkCreateDTO.SquareKilometers,
                AuthorId = authorId
            };
            _context.DogParks.Add(dogparkEntity);
            await _context.SaveChangesAsync();
            return (ResponseType.Created, dogparkEntity.PlaceId);
        }

        public async Task<DogParkDetailedDTO?> GetAsync(int? userId, int id)
        {
            var reviews = _context.Reviews.Where(r => r.PlaceId == id);

            var reviewStatus = ReviewStatus.MustCheckIn;
            if (await reviews.AnyAsync(r => r.UserId == userId))
            {
                reviewStatus = ReviewStatus.CanUpdateReview;
            }
            else if (userId is not null && await _checkInRepository.HasCheckedOutBeforeAsync(userId.Value, id))
            {
                reviewStatus = ReviewStatus.CanReview;
            }

            var entityQuery = _context.DogParks
                .Include(p => p.Place)
                .Where(x => x.PlaceId == id)
                .Select(p => new DogParkDetailedDTO
                {
                    Rating = reviews
                        .Sum(r => r.Rating) / Math.Max(reviews.Count(), 1),
                    RatingCount = reviews.Count(),
                    Description = p.Place.Description,
                    Name = p.Place.Name,
                    Facilities = p.Facilities,
                    Location = new(p.Place.Location.X, p.Place.Location.X),
                    TotalReviews = reviews.Count(),
                    Id = id,
                    CurrentReviewStatus = reviewStatus,
                    Bounds = p.Bounds == null ? new() : p.Bounds.Value.ToList(),
                    SquareKilometers = p.SquareKilometers,
                    DateAdded = p.DateAdded
                });
            var dto = await entityQuery.FirstOrDefaultAsync();
            return dto;
        }

        public async Task<DateTimePaginationResult<DogParkListDTO>> GetCreatedByAuthorAsync(int authorId, DateTimePaginationRequest paginationRequest)
        {
            var query = _context.DogParks
                .Include(d => d.Place)
                .Where(d => d.AuthorId == authorId);

            var orderByFunc = (IQueryable<DogPark> query) =>
                query.OrderByDescending(r => r.DateAdded);

            var keyOffsetFunc = (IQueryable<DogPark> query) =>
                query.Where(r => r.DateAdded < paginationRequest.LastDate);


            var (paginationResult, dogParks) = await RepositoryUtils.GetPaginationQuery<DogPark>(query, orderByFunc, keyOffsetFunc, paginationRequest);
            var dogParkDTOs = await dogParks.Select(d => new DogParkListDTO
            {
                Id = d.PlaceId,
                Location = new(d.Place.Location.X, d.Place.Location.X),
                Name = d.Place.Name,
                DateAdded = d.DateAdded
            }).ToListAsync();

            return new DateTimePaginationResult<DogParkListDTO>
            {
                CurrentPage = paginationResult.CurrentPage,
                HasNext = paginationResult.HasNext,
                Total = paginationResult.Total,
                LastDate = dogParkDTOs.LastOrDefault()?.DateAdded ?? DateTime.MinValue,
                LastId = dogParkDTOs.LastOrDefault()?.Id ?? -1,
                Result = dogParkDTOs
            };
        }

        public async Task<DateTimePaginationResult<DogParkRequestDetailedDTO>> GetRequestsAsync(int requesterId, DateTimePaginationRequest paginationRequest)
        {
            var query = _context.PendingDogParks
                .Where(d => d.RequesterId == requesterId);

            var orderByFunc = (IQueryable<PendingDogPark> query) =>
                query.OrderByDescending(r => r.RequestDate);

            var keyOffsetFunc = (IQueryable<PendingDogPark> query) =>
                query.Where(r => r.RequestDate < paginationRequest.LastDate);

            var (paginationResult, pendingDogParks) = await RepositoryUtils.GetPaginationQuery<PendingDogPark>(query, orderByFunc, keyOffsetFunc, paginationRequest);

            var dogParkDTOs = await pendingDogParks.Select(d => new DogParkRequestDetailedDTO
            {
                RequestDate = d.RequestDate,
                Bounds = d.Bounds,
                Description = d.Description,
                Facilities = d.Facilities,
                Name = d.Name,
                Point = new NpgsqlPoint(d.Location.X, d.Location.Y),
                SquareKilometers = d.SquareKilometers,
                Id = d.Id
            }).ToListAsync();

            return new DateTimePaginationResult<DogParkRequestDetailedDTO>
            {
                CurrentPage = paginationResult.CurrentPage,
                HasNext = paginationResult.HasNext,
                Total = paginationResult.Total,
                LastDate = dogParkDTOs.LastOrDefault()?.RequestDate ?? DateTime.MinValue,
                LastId = dogParkDTOs.LastOrDefault()?.Id ?? -1,
                Result = dogParkDTOs
            };
        }

        public async Task<(ResponseType, int)> RequestAsync(int requesterId, DogParkRequestCreateDTO dogParkRequestDTO)
        {
            var entity = new PendingDogPark()
            {
                RequesterId = requesterId,
                Name = dogParkRequestDTO.Name,
                Description = dogParkRequestDTO.Description,
                Location = new Point(x: dogParkRequestDTO.Point.X, y: dogParkRequestDTO.Point.Y),
                Bounds = dogParkRequestDTO.Bounds is null ? null : new NpgsqlPolygon(dogParkRequestDTO.Bounds),
                SquareKilometers = dogParkRequestDTO.SquareKilometers,
                Facilities = dogParkRequestDTO.Facilities.ToList(),
                FacilityType = Enums.FacilityType.DogPark
            };

            _context.PendingDogParks.Add(entity);
            await _context.SaveChangesAsync();
            return (ResponseType.Created, entity.Id);
        }

        public async Task<DistancePaginationResult<DogParkListDTO>> GetParksAsync(DogParksDTOPaginationRequest request)
        {
            var query = _context.Places.Where(p => p.FacilityType == FacilityType.DogPark);
            var point = new Point(request.PaginationRequest.SearchArea.Center.Lng, request.PaginationRequest.SearchArea.Center.Lat);
            var select = (IQueryable<Place> q) => q.Select(p => new DogParkListDTO
            {
                Name = p.Name,
                Location = new(p.Location.X, p.Location.Y),
                Rating = (float)_context.Reviews.Where(r => r.PlaceId == p.Id).Average(p => p.Rating),
                RatingCount = _context.Reviews.Where(r => r.PlaceId == p.Id).Count(),
                Id = p.Id,
                Facility = p.FacilityType,
                DistanceMeters = (float)EF.Functions.Distance(p.Location, point, true)
            });
            var paginationResult = await _placesRepository.GetPlacesByDistance(query, select, request.PaginationRequest);

            return paginationResult;
        }

        public async Task<IEnumerable<DogParkListDTO>> GetParksInAreaAsync(SearchAreaDTO searchArea)
        {
            var query = _context.Places.Where(p => p.FacilityType == FacilityType.DogPark);
            var places = _placesRepository.GetPlacesInArea(query, searchArea: searchArea);
            return await places
                .Select(p => new DogParkListDTO
                {
                    Name = p.Name,
                    Location = new(p.Location.X, p.Location.Y),
                    Rating = (float)_context.Reviews.Where(r => r.PlaceId == p.Id).Average(p => p.Rating),
                    RatingCount = _context.Reviews.Where(r => r.PlaceId == p.Id).Count(),
                    Id = p.Id,
                    Facility = p.FacilityType,
                })
                .ToListAsync();
        }
    }
}
