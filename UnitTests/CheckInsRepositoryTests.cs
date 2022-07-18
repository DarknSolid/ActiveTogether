using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
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
        public async Task CheckIn_Invalid_Dog_Returns_NotFound()
        {
            var userId = 3;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new() { 0},
                PlaceId = facilityId
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(RepositoryEnums.ResponseType.NotFound, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task CheckIn_If_Checked_In_Returns_Conflict()
        {
            var userId = 2;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new(),
                PlaceId = facilityId
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(RepositoryEnums.ResponseType.Conflict, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task CheckIn_With_Others_Dog_Returns_Conflict()
        {
            var userId = 3;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new() { 2 },
                PlaceId = facilityId
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(RepositoryEnums.ResponseType.Conflict, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task CheckIn_With_Valid_Data_Returns_Created_And_Correct_Data()
        {
            var userId = 4;
            var facilityId = 1;
            var dogId = 4;
            var dateBefore = DateTime.UtcNow;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new() { dogId },
                PlaceId = facilityId
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);
            var dateAfter = DateTime.UtcNow;
            var actualEntity = await _context.CheckIns.Include(c => c.DogCheckIns).FirstAsync(c => c.Id == id);

            Assert.Equal(RepositoryEnums.ResponseType.Created, response);
            Assert.Equal(5, id);
            Assert.Single(actualEntity.DogCheckIns);
            Assert.Equal(dogId, actualEntity.DogCheckIns.First().DogId);
            Assert.Equal(userId, actualEntity.UserId);
            Assert.Equal(facilityId, actualEntity.PlaceId);
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
        public async Task GetCheckIns_Valid_Facility_Only_Active_Checkins_Returns_Expected()
        {
            var facilityId = 1;
            var onlyActiveCheckIns = true;
            var paginationRequest = new PaginationRequest { ItemsPerPage = 10, Page = 0 };
            var dtoRequest = new GetCheckInListDTO
            {
                PaginationRequest = paginationRequest,
                PlaceId = facilityId,
                OnlyActiveCheckIns = onlyActiveCheckIns
            };

            var actual = await _repo.GetCheckIns(dtoRequest);

            Assert.Equal(3, actual.CheckIns.Count);
            Assert.True(actual.CheckIns.All(c => c.CheckedOut == null));
            var checkIn = actual.CheckIns.First();
            Assert.Single(checkIn.Dogs);
            var dog = checkIn.Dogs.First();
            Assert.Null(checkIn.CheckedOut);
            Assert.Equal(1, checkIn.User.UserId);
            Assert.Equal("test1", checkIn.User.FirstName);
            Assert.Equal("user1", checkIn.User.LastName);
            Assert.Equal("", checkIn.User.ProfilePictureUrl);
            Assert.Equal(1, dog.Id);
            Assert.Equal(1, dog.Breed);
            Assert.False(dog.IsGenderMale);
        }

        [Fact]
        public async Task GetCheckIns_Valid_Facility_All_Checkins_Returns_Expected()
        {
            var facilityId = 1;
            var onlyActiveCheckIns = false;
            var paginationRequest = new PaginationRequest { ItemsPerPage = 10, Page = 0 };
            var dtoRequest = new GetCheckInListDTO
            {
                PaginationRequest = paginationRequest,
                PlaceId = facilityId,
                OnlyActiveCheckIns = onlyActiveCheckIns
            };
            var actual = await _repo.GetCheckIns(dtoRequest);
            var count= await _context.CheckIns.CountAsync();

            Assert.Equal(count, actual.CheckIns.Count);
            Assert.True(actual.CheckIns.Exists(c => c.CheckedOut == null));
            Assert.True(actual.CheckIns.Exists(c => c.CheckedOut != null));
        }

        [Fact]
        public async Task GetCurrentlyCheckedIn_When_Checked_In_Returns_Checked_In_Data()
        {
            var userId = 1;
            var actual = await _repo.GetCurrentlyCheckedIn(userId);

            Assert.NotNull(actual);
            Assert.Equal(1, actual.Dogs.First().Id);
            Assert.Equal(1, actual.PlaceId);
        }

        [Fact]
        public async Task GetCurrentlyCheckedIn_When_Not_Checked_In_Returns_Null()
        {
            var userId = 3;
            var actual = await _repo.GetCurrentlyCheckedIn(userId);

            Assert.Null(actual);
        }
    }
}
