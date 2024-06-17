using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.DogPark;
using ModelLib.Repositories;
using Moq;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using static EntityLib.Entities.Enums;
using static ModelLib.DTOEnums;
using static ModelLib.DTOs.DogPark.DogParkDetailedDTO;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class DogParkRepositoryTests : TestBase
    {

        private readonly IDogParkRepository _repository;
        private readonly Mock<ICheckInRepository> _checkInMock;
        private readonly Mock<IPlacesRepository> _placesMock;

        public DogParkRepositoryTests()
        {
            _checkInMock = new Mock<ICheckInRepository>();
            _placesMock = new Mock<IPlacesRepository>();
            _repository = new DogParkRepository(_context, _checkInMock.Object, _placesMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Creates_Place_and_Park()
        {
            var createDto = new DogParkCreateDTO
            {
                Bounds = null,
                Description = "test park description",
                Name = "test park",
                SquareKilometers = 1,
                Facilities = new List<DogParkFacilityType> { DogParkFacilityType.Lake },
                Point = new NpgsqlPoint(1, 2),
            };

            var maxCountBefore = await _context.Places.CountAsync();
            var (actualResponse, actualId) = await _repository.CreateAsync(createDto);
            var actualEntity = await _context.DogParks.Include(d => d.Place).FirstOrDefaultAsync(d => d.PlaceId == actualId);
            Assert.Equal(maxCountBefore + 1, actualId);
            Assert.Equal(ResponseType.Created, actualResponse);
            Assert.Null(actualEntity.AuthorId);
            Assert.Equal(createDto.SquareKilometers, actualEntity.SquareKilometers);
            Assert.Single(actualEntity.Facilities);
            Assert.Equal(createDto.Facilities.First(), actualEntity.Facilities.First());
            Assert.Equal(createDto.Name, actualEntity.Place.Name);
            Assert.Equal(createDto.Description, actualEntity.Place.Description);
            Assert.Equal(createDto.Point.X, actualEntity.Place.Location.X);
            Assert.Equal(createDto.Point.Y, actualEntity.Place.Location.Y);
        }

        [Fact]
        public async Task GetAsync_Returns_DogPark()
        {
            var placeId = 1;
            var userId = 1;
            var result = await _repository.GetAsync(userId, placeId);
            var expected = await _context.DogParks.Include(d => d.Place).FirstOrDefaultAsync(d => d.PlaceId == placeId);

            Assert.Equal(expected.SquareKilometers, result.SquareKilometers);
            Assert.Null(result.Facilities);
            Assert.Equal(expected.Place.Name, result.Name);
            Assert.Equal(expected.Place.Description, result.Description);
            Assert.Equal(expected.Place.Location.X, result.Location.X);
            Assert.Equal(expected.Place.Location.Y, result.Location.Y);
            Assert.Equal(await _context.Reviews.Where(r => r.PlaceId == placeId).CountAsync(), result.TotalReviews);
        }

        [Theory]
        [InlineData(2, 1, ReviewStatus.CanUpdateReview)]
        [InlineData(1, 3, ReviewStatus.MustCheckIn)]
        [InlineData(2, 2, ReviewStatus.CanReview)]
        public async Task GetAsync_Never_Checked_In_Returns_Can_Not_Review(int userId, int placeId, ReviewStatus expectedReviewStatus)
        {
            _checkInMock.Setup(x => x.HasCheckedOutBeforeAsync(It.Is<int>(i => i == 2), It.Is<int>(i => i == 2))).ReturnsAsync(true);
            var result = await _repository.GetAsync(userId, placeId);
            Assert.Equal(expectedReviewStatus, result.CurrentReviewStatus);
        }

        [Fact]
        public async Task ApproveDogParkRequestAsync_Given_Existing_Request_Returns_Created_And_Place_Id()
        {
            var dogParkRequestId = 1;
            var expectedPlaceId = await _context.Places.MaxAsync(p => p.Id) + 1;

            var (response, id) = await _repository.ApproveDogParkRequestAsync(dogParkRequestId);

            var actualDogPark = await _context.DogParks.Include(d => d.Place).FirstOrDefaultAsync(p => p.PlaceId == expectedPlaceId);
            var actualPendingDogPark = await _context.PendingDogParks.FirstOrDefaultAsync(p => p.Id == dogParkRequestId);

            Assert.Equal(ResponseType.Created, response);
            Assert.Equal(expectedPlaceId, id);
            Assert.Null(actualPendingDogPark);
            Assert.NotNull(actualDogPark);
            Assert.Equal(expectedPlaceId, actualDogPark.Place.Id);
            Assert.Equal(1, actualDogPark.AuthorId);

        }

        [Fact]
        public async Task ApproveDogParkRequestAsync_Given_Existing_Request_Returns_NotFound_And_Id_Negative_One()
        {
            var maxPlaceIdBefore = await _context.Places.MaxAsync(p => p.Id);

            var (response, id) = await _repository.ApproveDogParkRequestAsync(-1);

            var maxPlaceIdAfter = await _context.Places.MaxAsync(p => p.Id);


            Assert.Equal(ResponseType.NotFound, response);
            Assert.Equal(-1, id);
            Assert.Equal(maxPlaceIdBefore, maxPlaceIdAfter);
        }

        [Fact]
        public async Task RequestAsync_Given_Valid_Data_Returns_Created_And_Request_Id()
        {
            var timestampBefore = DateTime.UtcNow;
            var requesterId = 1;
            var expectedCreatedId = await _context.PendingDogParks.MaxAsync(p => p.Id) + 1;
            var dto = new DogParkRequestCreateDTO()
            {
                Description = "desc",
                Name = "name",
                Point = new NpgsqlPoint(1, 2),
                SquareKilometers = 5
            };

            var (actualResponse, actualId) = await _repository.RequestAsync(requesterId: requesterId, dto);
            var actualEntity = await _context.PendingDogParks.FirstOrDefaultAsync(p => p.Id == actualId);

            Assert.Equal(ResponseType.Created, actualResponse);
            Assert.Equal(expectedCreatedId, actualId);
            Assert.NotNull(actualEntity);
            Assert.Equal(dto.Name, actualEntity.Name);
            Assert.Equal(dto.Description, actualEntity.Description);
            Assert.Equal(dto.Point.X, actualEntity.Location.X);
            Assert.Equal(dto.Point.Y, actualEntity.Location.Y);
            Assert.Equal(dto.SquareKilometers, actualEntity.SquareKilometers);
            Assert.Equal(requesterId, actualEntity.RequesterId);
            Assert.True(timestampBefore < actualEntity.RequestDate);
        }

        [Theory]
        [InlineData(1, 0, -1, 1, 1)]
        public async Task GetCreatedByAuthorAsync_ReturnsExpected(int authorId, int currentPage, int lastId, int lastDateTimeHours, int expectedItemId)
        {
            var paginationRequest = new DateTimePaginationRequest()
            {
                ItemsPerPage = 1,
                LastDate = DateTime.UtcNow.AddHours(lastDateTimeHours),
                LastId = lastId,
                Page = currentPage
            };

            var result = await _repository.GetCreatedByAuthorAsync(authorId, paginationRequest);
            //Assert that all results comes from the correct author
            Assert.Single(result.Result);
            Assert.True(result.Result.All(p => _context.DogParks.Any(x => x.PlaceId == p.Id && x.AuthorId == authorId)));
            Assert.Equal(expectedItemId, result.Result.First().Id);
        }

        [Theory]
        [InlineData(1, 0, -1, 1, 1)]
        public async Task GetCreatedByRequesterAsync_ReturnsExpected(int requesterId, int currentPage, int lastId, int lastDateTimeHours, int expectedItemId)
        {
            var paginationRequest = new DateTimePaginationRequest()
            {
                ItemsPerPage = 1,
                LastDate = DateTime.UtcNow.AddHours(lastDateTimeHours),
                LastId = lastId,
                Page = currentPage
            };

            var result = await _repository.GetRequestsAsync(requesterId, paginationRequest);
            //Assert that all results comes from the correct author
            Assert.Single(result.Result);
            Assert.True(result.Result.All(p => _context.PendingDogParks.Any(x => x.Id == p.Id && x.RequesterId == requesterId)));
            Assert.Equal(expectedItemId, result.Result.First().Id);
        }
    }
}