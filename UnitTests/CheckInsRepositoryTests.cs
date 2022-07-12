using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.CheckIns;
using ModelLib.Repositories;
using Moq;
using static EntityLib.Entities.Enums;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class CheckInsRepositoryTests : TestBase
    {
        private readonly ICheckInRepository _repo;
        private readonly Mock<IFacilityRepository> _facilityRepositoryMock;

        public CheckInsRepositoryTests()
        {
            _facilityRepositoryMock = new Mock<IFacilityRepository>();
            _repo = new CheckInRepository(_context, _facilityRepositoryMock.Object);
        }

        [Fact]
        public async Task CheckIn_Invalid_Facility_Returns_NotFound()
        {
            _facilityRepositoryMock
                .Setup(x => x.FacilityExists(It.IsAny<int>(), It.IsAny<FacilityType>()))
                .Returns(Task.FromResult(false));

            var userId = 3;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new(),
                FacilityId = facilityId,
                FacilityType = FacilityType.DogPark
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(RepositoryEnums.ResponseType.NotFound, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task CheckIn_Invalid_Dog_Returns_NotFound()
        {
            _facilityRepositoryMock
                .Setup(x => x.FacilityExists(It.IsAny<int>(), It.IsAny<FacilityType>()))
                .Returns(Task.FromResult(true));

            var userId = 3;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new() { 0},
                FacilityId = facilityId,
                FacilityType = FacilityType.DogPark
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(RepositoryEnums.ResponseType.NotFound, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task CheckIn_If_Checked_In_Returns_Conflict()
        {
            _facilityRepositoryMock
                .Setup(x => x.FacilityExists(It.IsAny<int>(), It.IsAny<FacilityType>()))
                .Returns(Task.FromResult(true));

            var userId = 2;
            var facilityId = 1;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new(),
                FacilityId = facilityId,
                FacilityType = FacilityType.DogPark
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
                FacilityId = facilityId,
                FacilityType = FacilityType.DogPark
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);

            Assert.Equal(RepositoryEnums.ResponseType.Conflict, response);
            Assert.Equal(-1, id);
        }

        [Fact]
        public async Task CheckIn_With_Valid_Data_Returns_Created_And_Correct_Data()
        {
            _facilityRepositoryMock
                .Setup(x => x.FacilityExists(It.IsAny<int>(), It.IsAny<FacilityType>()))
                .Returns(Task.FromResult(true));

            var userId = 4;
            var facilityId = 1;
            var dogId = 4;
            var dateBefore = DateTime.UtcNow;
            var createDto = new CheckInCreateDTO
            {
                DogsToCheckIn = new() { dogId },
                FacilityId = facilityId,
                FacilityType = FacilityType.DogPark
            };

            var (response, id) = await _repo.CheckIn(userId, createDto);
            var dateAfter = DateTime.UtcNow;
            var actualEntity = await _context.CheckIns.Include(c => c.DogCheckIns).FirstAsync(c => c.Id == id);

            Assert.Equal(RepositoryEnums.ResponseType.Created, response);
            Assert.Equal(5, id);
            Assert.Single(actualEntity.DogCheckIns);
            Assert.Equal(dogId, actualEntity.DogCheckIns.First().DogId);
            Assert.Equal(userId, actualEntity.UserId);
            Assert.Equal(facilityId, actualEntity.FacilityId);
            Assert.Equal(FacilityType.DogPark, actualEntity.FacilityType);
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
            var facilityType = FacilityType.DogPark;
            var onlyActiveCheckIns = true;
            var paginationRequest = new PaginationRequest { ItemsPerPage = 10, Page = 0 };
            var dtoRequest = new GetCheckInListDTO
            {
                PaginationRequest = paginationRequest,
                FacilityId = facilityId,
                FacilityType = facilityType,
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
            var facilityType = FacilityType.DogPark;
            var onlyActiveCheckIns = false;
            var paginationRequest = new PaginationRequest { ItemsPerPage = 10, Page = 0 };
            var dtoRequest = new GetCheckInListDTO
            {
                PaginationRequest = paginationRequest,
                FacilityId = facilityId,
                FacilityType = facilityType,
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
            Assert.Equal(1, actual.FacilityId);
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
