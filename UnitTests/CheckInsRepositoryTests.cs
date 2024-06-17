using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.CheckIns;
using ModelLib.Repositories;
using Moq;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class CheckInsRepositoryTests : TestBase
    {
        private readonly ICheckInRepository _repo;

        public CheckInsRepositoryTests()
        {
            _repo = new CheckInRepository(_context);
        }

        [Fact]
        public async Task CheckIn_If_Checked_In_Returns_Conflict()
        {
            var userId = 2;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                PlaceId = facilityId
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(ResponseType.Conflict, response);
            Assert.Equal(-1, id);
        }


        [Fact]
        public async Task CheckIn_With_Valid_Data_Returns_Created_And_Correct_Data()
        {
            var userId = 4;
            var facilityId = 1;
            var dogId = 4;
            var mood = Enums.CheckInMood.Social;
            var dateBefore = DateTime.UtcNow;
            var createDto = new CheckInCreateDTO
            {
                PlaceId = facilityId,
                Mood = mood
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);
            var dateAfter = DateTime.UtcNow;
            var actualEntity = await _context.CheckIns.FirstAsync(c => c.Id == id);

            Assert.Equal(ResponseType.Created, response);
            Assert.Equal(userId, actualEntity.UserId);
            Assert.Equal(facilityId, actualEntity.PlaceId);
            Assert.Equal(mood, actualEntity.Mood);
            Assert.True(dateBefore < actualEntity.CheckInDate);
            Assert.True(actualEntity.CheckInDate < dateAfter);
            Assert.Null(actualEntity.CheckOutDate);
        }

        [Fact]
        public async Task CheckOut_When_Checked_In_Returns_Updated_And_Sets_Date()
        {
            var userId = 5;
            var dateBefore = DateTime.UtcNow;
            var (response, id) = await _repo.CheckOut(userId);
            var actualEntity = await _context.CheckIns.FirstAsync(c => c.Id == id);
            var dateAfter = DateTime.UtcNow;

            Assert.True(dateBefore < actualEntity.CheckOutDate);
            Assert.True(actualEntity.CheckOutDate < dateAfter);
            Assert.Equal(ResponseType.Updated, response);
        }

        [Fact]
        public async Task CheckOut_When_Not_Checked_In_Returns_Not_Found()
        {
            var userId = 6;
            var (response, id) = await _repo.CheckOut(userId);

            Assert.Equal(ResponseType.NotFound, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task GetCheckIns_Valid_Facility_All_Checkins_Returns_Expected()
        {
            var facilityId = 1;
            var onlyActiveCheckIns = false;
            var paginationRequest = new DateTimePaginationRequest { ItemsPerPage = 10, Page = 0, LastDate = DateTime.UtcNow.AddDays(1), LastId = -1 };
            var dtoRequest = new CheckInListDTOPaginationRequest
            {
                PaginationRequest = paginationRequest,
                PlaceId = facilityId,
                OnlyActiveCheckIns = onlyActiveCheckIns
            };
            var actual = await _repo.GetCheckIns(dtoRequest);
            var count = await _context.CheckIns.Where(c => c.PlaceId == facilityId).CountAsync();

            Assert.Equal(count, actual.Result.Count);
            Assert.True(actual.Result.Any(c => c.CheckedOutDate == null));
            Assert.True(actual.Result.Any(c => c.CheckedOutDate != null));
        }

        [Fact]
        public async Task GetCheckIns_Valid_Facility_Only_Active_Checkins_Returns_Expected()
        {
            var facilityId = 1;
            var onlyActiveCheckIns = true;
            var paginationRequest = new DateTimePaginationRequest { ItemsPerPage = 10, Page = 0, LastDate = DateTime.UtcNow.AddDays(1), LastId = -1 };
            var dtoRequest = new CheckInListDTOPaginationRequest
            {
                PaginationRequest = paginationRequest,
                PlaceId = facilityId,
                OnlyActiveCheckIns = onlyActiveCheckIns
            };

            var actual = await _repo.GetCheckIns(dtoRequest);

            Assert.Equal(3, actual.Result.Count);
            Assert.True(actual.Result.All(c => c.CheckedOutDate == null));
            var checkIn = actual.Result.FirstOrDefault(c => c.Id == 1);

            Assert.Null(checkIn.CheckedOutDate);
            Assert.Equal(1, checkIn.User.Id);
            Assert.Equal("test1", checkIn.User.FirstName);
            Assert.Equal("user1", checkIn.User.LastName);
            Assert.Equal("", checkIn.User.ProfilePictureUrl);
            Assert.Equal(Enums.CheckInMood.Social, checkIn.Mood);
        }

        [Fact]
        public async Task GetCurrentlyCheckedIn_When_Checked_In_Returns_Checked_In_Data()
        {
            var userId = 1;
            var actual = await _repo.GetCurrentlyCheckedIn(userId);

            Assert.NotNull(actual);
            Assert.Equal(1, actual.PlaceId);
            Assert.Equal(Enums.CheckInMood.Social, actual.Mood);

        }

        [Fact]
        public async Task GetCurrentlyCheckedIn_When_Not_Checked_In_Returns_Null()
        {
            var userId = 3;
            var actual = await _repo.GetCurrentlyCheckedIn(userId);

            Assert.Null(actual);
        }

        [Fact]
        public async Task GetStatistics_With_Existing_Check_ins_Returns_Expected()
        {
            var placeId = 1;
            var lookBackDays = 30;
            var expectedDogCheckIns = 2;
            var expectedPeopleCheckIns = 4;
            var today = DateTime.UtcNow.DayOfWeek;
            var currentHour = DateTime.UtcNow.Hour;

            CheckInStatisticsDetailedDTO actual = await _repo.GetStatisticsAsync(placeId: placeId, lookBackDays: lookBackDays);
            var actualCurrentDayAndHourResult = actual.IntraDayStatistics[today][currentHour];

            Assert.Equal(expectedDogCheckIns, actual.DogCheckIns);
            Assert.Equal(expectedPeopleCheckIns, actual.PeopleCheckIns);

            Assert.Equal(expectedPeopleCheckIns, actualCurrentDayAndHourResult.People);

            // Assert that at all days and hours there are no checkins, exluding the today and the current hour that you are reading this :P

            Assert.Single(actual.IntraDayStatistics);
            Assert.Single(actual.IntraDayStatistics[today]);
            Assert.Equal(currentHour, actual.IntraDayStatistics[today].Keys.First());
            Assert.Equal(today, actual.IntraDayStatistics.Keys.First());
        }

        [Fact]
        public async Task GetStatistics_With_Existing_Check_ins_But_Not_In_Date_Range_Returns_Empty_Results()
        {
            var placeId = 1;
            var lookBackDays = -1; // makes the repositry add 1 day instead of subtracting 1 day
            var today = DateTime.UtcNow.DayOfWeek;
            var currentHour = DateTime.UtcNow.Hour;
            var expectedDogCheckIns = 0;
            var expectedPeopleCheckIns = 0;

            CheckInStatisticsDetailedDTO actual = await _repo.GetStatisticsAsync(placeId: placeId, lookBackDays: lookBackDays);

            Assert.Equal(expectedPeopleCheckIns, actual.PeopleCheckIns);
            Assert.Equal(expectedDogCheckIns, actual.DogCheckIns);
            Assert.Empty(actual.IntraDayStatistics);
        }
    }
}
