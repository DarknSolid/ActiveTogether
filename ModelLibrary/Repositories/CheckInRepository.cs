using EntityLib;
using EntityLib.Entities;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
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
        public Task<PaginationResult<CheckInListDTO>> GetCheckIns(CheckInListDTOPaginationRequest dto);
        public Task<bool> HasCheckedOutBeforeAsync(int userId, int placeId);
        public Task<CheckInStatisticsDetailedDTO> GetStatisticsAsync(int placeId, int lookBackDays);
    }

    public class CheckInRepository : RepositoryBase, ICheckInRepository
    {

        public CheckInRepository(IApplicationDbContext context) : base(context)
        {
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

            //Can't check in to an invalid place or invalid dogs
            if (await _context.Places.AnyAsync(p => p.Id == dto.PlaceId) == false ||
                await DogsExists(dto.DogsToCheckIn) == false)
            {
                return new(ResponseType.NotFound, -1);
            }

            // TODO can we make all of this in a single query to the database?
            var checkInEntity = new CheckIn
            {
                PlaceId = dto.PlaceId,
                UserId = userId,
                CheckInDate = DateTime.UtcNow,
                Mood = dto.Mood
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

        public async Task<PaginationResult<CheckInListDTO>> GetCheckIns(CheckInListDTOPaginationRequest request)
        {
            var query = _context.CheckIns
                .Include(c => c.DogCheckIns)
                .Include(c => c.User)
                .Where(c => c.PlaceId == request.PlaceId);

            if (request.OnlyActiveCheckIns)
            {
                query = query.Where(c => c.CheckOutDate == null);
            }

            var orderByFunc = (IQueryable<CheckIn> query) =>
                query.OrderByDescending(c => c.CheckInDate);

            var keyOffsetFunc = (IQueryable<CheckIn> query) =>
                query.Where(c => c.CheckInDate < request.PaginationRequest.LastDate);

            var (paginationResult, paginatedQuery) = await RepositoryUtils.GetPaginationQuery(query, orderByFunc, keyOffsetFunc, request.PaginationRequest);

            var result = await paginatedQuery.Select(c => new CheckInListDTO
            {
                Id = c.Id,
                CheckedInDate = c.CheckInDate,
                CheckedOutDate = c.CheckOutDate,
                Mood = c.Mood,
                User = new UserListDTO
                {
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    ProfilePictureUrl = c.User.ProfileImageUrl,
                    Id = c.UserId
                },
                Dogs = _context.Dogs
                    .Include(d => d.Race)
                    .Where(d => c.DogCheckIns
                    .Any(dc => dc.DogId == d.Id))
                    .Select(d => new DogListDTO
                    {
                        Id = d.Id,
                        Birth = d.Birth,
                        Race = d.Race,
                        IsGenderMale = d.IsGenderMale,
                        Name = d.Name
                    }).ToList()
            }).ToListAsync();

            return new PaginationResult<CheckInListDTO>
            {
                Result = result,
                LastId = result.LastOrDefault()?.Id ?? -1,
                CurrentPage = paginationResult.CurrentPage,
                HasNext = paginationResult.HasNext,
                Total = paginationResult.Total

            };
        }

        public async Task<CurrentlyCheckedInDTO?> GetCurrentlyCheckedIn(int userId)
        {
            var entity = await _context.CheckIns
                .Include(c => c.DogCheckIns)
                .Include(c => c.Place)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CheckOutDate == null);

            if (entity == null)
            {
                return null;
            }

            var dogs = await _context.Dogs
                .Include(d => d.Race)
                .Include(d => d.CheckIns)
                .Where(d => d.CheckIns.Any(c => c.CheckInId == entity.Id))
                .Select(d => new DogListDTO
                {
                    Birth = d.Birth,
                    Id = d.Id,
                    Race = d.Race,
                    Name = d.Name,
                    IsGenderMale = d.IsGenderMale
                }).ToListAsync();

            return new CurrentlyCheckedInDTO
            {
                Dogs = dogs,
                PlaceId = entity.PlaceId,
                FacilityType = entity.Place.FacilityType,
                CheckInDate = entity.CheckInDate,
                Mood = entity.Mood
            };
        }

        private async Task<bool> DogsExists(List<int> dogIds)
        {
            // TODO this may create N queries to the database.. Optimize the performance plz
            return dogIds.All(id => _context.Dogs.Any(d => d.Id == id));
        }

        private async Task<bool> AreDogsNotOwnedByUser(int userId, List<int> dogIds)
        {
            return await _context.Dogs.AnyAsync(d => dogIds.Contains(d.Id) && d.UserId != userId);
        }

        public async Task<bool> HasCheckedOutBeforeAsync(int userId, int placeId)
        {
            return await _context.CheckIns.AnyAsync(p => p.PlaceId == placeId && p.CheckOutDate != null && p.UserId == userId);
        }

        public async Task<CheckInStatisticsDetailedDTO> GetStatisticsAsync(int placeId, int lookBackDays)
        {
            var startDate = DateTime.UtcNow.AddDays(-lookBackDays);
            var result = await _context.CheckIns
                .Include(c => c.DogCheckIns)
                .Where(c => c.PlaceId == placeId && c.CheckInDate > startDate)
                .Select(c => new CheckInStatisticsSelect
                {
                    CheckInDate = c.CheckInDate,
                    Dogs = c.DogCheckIns.Count()
                })
                .ToListAsync();

            var totalDogs = result.Sum(c => c.Dogs);
            var totalPersons = result.Count();


            var groupedByDayOfWeek = result.GroupBy(r => r.CheckInDate.DayOfWeek);

            // group results into a nested grouping: day of week, then by hour:
            var dayOfWeekGrouping =
                from checkIn in result
                group checkIn by checkIn.CheckInDate.DayOfWeek into dayOfWeekGroup
                from hourGrouping in (
                    from checkIn in dayOfWeekGroup
                    group checkIn by checkIn.CheckInDate.Hour
                )
                group hourGrouping by dayOfWeekGroup.Key;

            Dictionary<DayOfWeek, Dictionary<int, CheckInStatisticsHourTupleDTO>> intraDayStatistics = new();

            //fill dict with actual statistics:
            foreach (var dayGroups in dayOfWeekGrouping)
            {
                Dictionary<int, CheckInStatisticsHourTupleDTO> hourlyStatistics = new();
                foreach (var hourGroup in dayGroups.ToArray())
                {
                    var peopleCheckIns = hourGroup.Count();
                    var dogCheckIns = hourGroup.Sum(c => c.Dogs);
                    hourlyStatistics.Add(hourGroup.Key,
                        new CheckInStatisticsHourTupleDTO
                        {
                            People = peopleCheckIns,
                            Dogs = dogCheckIns
                        });
                }
                intraDayStatistics.Add(dayGroups.Key, hourlyStatistics);
            }

            return new CheckInStatisticsDetailedDTO
            {
                IntraDayStatistics = intraDayStatistics,
                DogCheckIns = totalDogs,
                PeopleCheckIns = totalPersons
            };
        }
    }

    /// <summary>
    /// Used to only select the relevant fields from checkins by the statistics calculation method
    /// </summary>
    internal class CheckInStatisticsSelect
    {
        public DateTime CheckInDate { get; set; }
        public int Dogs { get; set; }
    }
}
