using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using Moq;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class ReviewRepositoryTests : TestBase
    {
        private readonly IReviewsRepository _repo;
        private readonly Mock<ICheckInRepository> _mock;
        private readonly Mock<IBlobStorageRepository> _blobStorageRepository;

        public ReviewRepositoryTests()
        {
            _mock = new Mock<ICheckInRepository>();
            _blobStorageRepository = new Mock<IBlobStorageRepository>();
            _repo = new ReviewsRepository(_context, _mock.Object, _blobStorageRepository.Object);

        }

        [Fact]
        public async Task GetReviews_DogPark_Returns_DogPark_reviews()
        {
            var paginationRequest = new DateTimePaginationRequest { ItemsPerPage = 10, Page = 0, LastDate = DateTime.UtcNow.AddDays(1), LastId = -1 };
            var request = new ReviewsDTOPaginationRequest { PlaceId = 1, PaginationRequest = paginationRequest };
            var expectedCount = 2;

            var pagination = await _repo.GetReviewsAsync(request);
            var firstDogPark = pagination.Result.First();
            var secondDogPark = pagination.Result[1];

            Assert.Equal(expectedCount, pagination.Result.Count);
            Assert.Equal(request.PlaceId, firstDogPark.Id);
            Assert.Equal(request.PlaceId, secondDogPark.Id);
            Assert.Equal(2, firstDogPark.ReviewerId);
            Assert.Equal(1, secondDogPark.ReviewerId);
        }

        [Fact]
        public async Task CreateReview_Valid_DogPark_creates_and_returns_DogPark_id()
        {
            _mock.Setup(m => m.HasCheckedOutBeforeAsync(2, 2)).Returns(Task.FromResult(true));
            var userId = 2;
            var dto = new ReviewCreateDTO
            {
                Id = 2,
                Description = "description",
                Title = "title",
                Rating = 4
            };

            var (actualResponse, actualDto) = await _repo.CreateReviewAsync(userId, dto);
            var actualEntity = await _context.Reviews.Where(r =>
                r.PlaceId == dto.Id &&
                r.UserId == userId
            ).FirstOrDefaultAsync();

            Assert.NotNull(dto);
            Assert.NotNull(actualEntity);
            Assert.Equal(ResponseType.Created, actualResponse);
            Assert.Equal(dto.Id, actualDto.PlaceId);
            Assert.Equal(userId, actualDto.UserId);
            Assert.Equal(dto.Id, actualEntity.PlaceId);
            Assert.Equal(userId, actualEntity.UserId);
            Assert.Equal(dto.Title, actualEntity.Title);
            Assert.Equal(dto.Description, actualEntity.Description);
            Assert.Equal(dto.Rating, actualEntity.Rating);
        }

        [Fact]
        public async Task CreateReview_Valid_DogPark_updates_and_returns_DogPark_id()
        {
            _mock.Setup(m => m.HasCheckedOutBeforeAsync(1, 1)).Returns(Task.FromResult(true));
            var uesrId = 1;
            var dto = new ReviewCreateDTO
            {
                Id = 1,
                Description = "description",
                Title = "title",
                Rating = 4
            };

            var (actualResponse, actualDto) = await _repo.CreateReviewAsync(uesrId, dto);
            var actualEntity = await _context.Reviews.Where(r =>
                r.PlaceId == dto.Id &&
                r.UserId == uesrId
            ).FirstOrDefaultAsync();

            Assert.NotNull(actualDto);
            Assert.NotNull(actualEntity);
            Assert.Equal(ResponseType.Updated, actualResponse);
            Assert.Equal(dto.Id, actualDto.PlaceId);
            Assert.Equal(uesrId, actualDto.UserId);
            Assert.Equal(dto.Id, actualEntity.PlaceId);
            Assert.Equal(uesrId, actualEntity.UserId);
            Assert.Equal(dto.Title, actualEntity.Title);
            Assert.Equal(dto.Description, actualEntity.Description);
            Assert.Equal(dto.Rating, actualEntity.Rating);
        }

        [Fact]
        public async Task CreateReview_Invalid_DogPark_returns_not_found()
        {
            var reviewerId = 1;
            var dto = new ReviewCreateDTO
            {
                Id = -1,
                Description = "description",
                Title = "title",
                Rating = 4
            };
            var entitiesBefore = await _context.Reviews.CountAsync();
            var (actualResponse, actualDto) = await _repo.CreateReviewAsync(reviewerId, dto);
            var entitiesAfter = await _context.Reviews.CountAsync();

            Assert.Equal(entitiesBefore, entitiesAfter);
            Assert.Null(actualDto);
            Assert.Equal(ResponseType.NotFound, actualResponse);


        }

        [Fact]
        public async Task CreateReview_Never_Checked_In_Returns_Conflict()
        {
            _mock.Setup(m => m.HasCheckedOutBeforeAsync(2, 2)).Returns(Task.FromResult(false));
            var userId = 1;
            var dto = new ReviewCreateDTO
            {
                Id = 1,
                Description = "description",
                Title = "title",
                Rating = 4
            };
            var entitiesBefore = await _context.Reviews.CountAsync();
            var (actualResponse, actualDto) = await _repo.CreateReviewAsync(userId, dto);
            var entitiesAfter = await _context.Reviews.CountAsync();

            Assert.Equal(entitiesBefore, entitiesAfter);
            Assert.Null(actualDto);
            Assert.Equal(ResponseType.Conflict, actualResponse);


        }
    }
}
