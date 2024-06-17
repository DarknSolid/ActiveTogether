using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.CheckIns;
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

            //Can't check in to an invalid place or invalid dogs
            if (await _context.Places.AnyAsync(p => p.Id == dto.PlaceId) == false)
            {
                return new(ResponseType.NotFound, -1);
            }

            var checkInEntity = new CheckIn
            {
                PlaceId = dto.PlaceId,
                UserId = userId,
                CheckInDate = DateTime.UtcNow,
                Mood = dto.Mood
            };

            _context.CheckIns.Add(checkInEntity);
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
                .Include(c => c.Place)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CheckOutDate == null);

            if (entity == null)
            {
                return null;
            }

            return new CurrentlyCheckedInDTO
            {
                PlaceId = entity.PlaceId,
                FacilityType = entity.Place.FacilityType,
                CheckInDate = entity.CheckInDate,
                Mood = entity.Mood
            };
        }

        public async Task<bool> HasCheckedOutBeforeAsync(int userId, int placeId)
        {
            return await _context.CheckIns.AnyAsync(p => p.PlaceId == placeId && p.CheckOutDate != null && p.UserId == userId);
        }

        public async Task<CheckInStatisticsDetailedDTO> GetStatisticsAsync(int placeId, int lookBackDays)
        {
            var startDate = DateTime.UtcNow.AddDays(-lookBackDays);
            var result = await _context.CheckIns
                .Where(c => c.PlaceId == placeId && c.CheckInDate > startDate)
                .Select(c => new CheckInStatisticsSelect
                {
                    CheckInDate = c.CheckInDate,
                })
                .ToListAsync();

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
                    hourlyStatistics.Add(hourGroup.Key,
                        new CheckInStatisticsHourTupleDTO
                        {
                            People = peopleCheckIns,
                        });
                }
                intraDayStatistics.Add(dayGroups.Key, hourlyStatistics);
            }

            return new CheckInStatisticsDetailedDTO
            {
                IntraDayStatistics = intraDayStatistics,
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
    }
}
