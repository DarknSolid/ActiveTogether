using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.CheckIns;
using ModelLib.DTOs.Dogs;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface ICheckInRepository
    {
        public Task<(ResponseType, int)> CheckIn(int userId, CheckInCreateDTO dto);
        public Task<(ResponseType, int)> CheckOut(int userId);
        public Task<CurrentlyCheckedInDTO?> GetCurrentlyCheckedIn(int userId);
        public Task<CheckInListDTOPagination> GetCheckIns(PaginationRequest paginationRequest, GetCheckInListDTO dto);
    }

    public class CheckInRepository : ICheckInRepository
    {

        private readonly IApplicationDbContext _context;
        private readonly IFacilityRepository _facilityRepository;

        public CheckInRepository(IApplicationDbContext context, IFacilityRepository facilityRepository)
        {
            _context = context;
            _facilityRepository = facilityRepository;
        }

        public async Task<(ResponseType, int)> CheckIn(int userId, CheckInCreateDTO dto)
        {
            // Can't check in if already checked in
            if (await GetCurrentlyCheckedIn(userId) != null)
            {
                return new(ResponseType.Conflict, -1);
            }

            //Can only check in with user's own dogs
            if (await AreDogsNotOwnedByUser(userId, dto.DogsToCheckIn))
            {
                return new(ResponseType.Conflict, -1);
            }

            //Can't check in to an invalid facility
            if (await _facilityRepository.FacilityExists(dto.FacilityId, dto.FacilityType) == false)
            {
                return new (ResponseType.NotFound, -1);
            }

            // TODO can we make all of this in a single query to the database?
            var checkInEntity = new CheckIn
            {
                FacilityId = dto.FacilityId,
                FacilityType = dto.FacilityType,
                UserId = userId,
                CheckInDate = DateTime.UtcNow,
            };

            _context.CheckIns.Add(checkInEntity);
            await _context.SaveChangesAsync();

            var dogCheckIns = dto.DogsToCheckIn.Select(dogId => new DogCheckIn { CheckInId = checkInEntity.Id, DogId = dogId });
            _context.DogCheckIns.AddRange(dogCheckIns);
            await _context.SaveChangesAsync();

            return new(ResponseType.Created, checkInEntity.Id);
        }

        public async Task<(ResponseType, int)> CheckOut(int userId)
        {
            var currentCheckInEntity = await _context.CheckIns.FirstOrDefaultAsync(c => c.UserId == userId && c.CheckOutDate == null);
            if (currentCheckInEntity == null)
            {
                return (ResponseType.NotFound, -1);
            }

            currentCheckInEntity.CheckOutDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (ResponseType.Updated, currentCheckInEntity.Id);
        }

        public async Task<CheckInListDTOPagination> GetCheckIns(PaginationRequest paginationRequest, GetCheckInListDTO dto)
        {
            var query = _context.CheckIns
                .Include(c => c.DogCheckIns)
                .Include(c => c.User)
                .Where(c => c.FacilityId == dto.FacilityId && c.FacilityType == dto.FacilityType);

            if (dto.OnlyActiveCheckIns)
            {
                query = query.Where(c => c.CheckOutDate == null);
            }

            var (paginationResult, paginatedQuery) = await RepositoryUtils.GetPaginationQuery(query, paginationRequest);
            
            var result = await paginatedQuery.Select(c => new CheckInListDTO
            {
                CheckedIn = c.CheckInDate,
                CheckedOut = c.CheckOutDate,
                User = new UserListDTO
                {
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    ProfilePictureUrl = c.User.ProfileImageUrl,
                    UserId = c.UserId
                },
                Dogs = _context.Dogs
                    .Include(d => d.DogBreed)
                    .Where(d => c.DogCheckIns
                    .Any(dc => dc.DogId == d.Id))
                    .Select(d => new DogListDTO
                    {
                        Id = d.Id,
                        Birth = d.Birth,
                        Breed = d.DogBreedId,
                        BreedName = d.DogBreed.Name,
                        IsGenderMale = d.IsGenderMale,
                        Name = d.Name
                    }).ToList()
            }).ToListAsync();

            return new CheckInListDTOPagination
            {
                CheckIns = result,
                PaginationResult = paginationResult

            };
        }

        public async Task<CurrentlyCheckedInDTO?> GetCurrentlyCheckedIn(int userId)
        {
            var entity = await _context.CheckIns.Include(c => c.DogCheckIns).FirstOrDefaultAsync(c => c.UserId == userId && c.CheckOutDate == null);
            if (entity == null)
            {
                return null;
            }

            var dogs = await _context.Dogs
                .Include(d => d.DogBreed)
                .Include(d => d.CheckIns)
                .Where(d => d.CheckIns.Any(c => c.CheckInId == entity.Id))
                .Select(d => new DogListDTO
            {
                    Birth = d.Birth,
                    Id = d.Id,
                    Breed = d.DogBreedId,
                    BreedName = d.DogBreed.Name,
                    Name = d.Name,
                    IsGenderMale = d.IsGenderMale
            }).ToListAsync();

            return new CurrentlyCheckedInDTO
            {
                Dogs = dogs,
                FacilityId = entity.FacilityId,
                FacilityType = entity.FacilityType,
                CheckInDate = entity.CheckInDate
            };
        }
         
        private async Task<bool> AreDogsNotOwnedByUser(int userId, List<int> dogIds)
        {
            return await _context.Dogs.AnyAsync(d => dogIds.Contains(d.Id) && d.UserId != userId);
        }
    }
}
