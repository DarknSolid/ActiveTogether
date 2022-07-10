using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class ReviewRepositoryTests : TestBase
    {
        private readonly IReviewsRepository _repo;

        public ReviewRepositoryTests()
        {
            _repo = new ReviewsRepository(_context);
        }

        [Fact]
        public async Task GetReviews_DogPark_Returns_DogPark_reviews()
        {
            var paginationRequest = new PaginationRequest { ItemsPerPage = 10, Page = 0 };
            var request = new ReviewsDTO { RevieweeId = 1, ReviewType = Enums.ReviewType.DogPark, PaginationRequest = paginationRequest };
            var expectedCount = 2;

            var (pagination, dogParkReviews) = await _repo.GetReviewsAsync(request);
            var firstDogPark = dogParkReviews.First();
            var secondDogPark = dogParkReviews[1];

            Assert.Equal(expectedCount, dogParkReviews.Count);
            Assert.Equal(request.RevieweeId, firstDogPark.RevieweeId);
            Assert.Equal(request.RevieweeId, secondDogPark.RevieweeId);
            Assert.Equal(Enums.ReviewType.DogPark, firstDogPark.ReviewType);
            Assert.Equal(Enums.ReviewType.DogPark, secondDogPark.ReviewType);
            Assert.Equal(2, firstDogPark.ReviewerId);
            Assert.Equal(1, secondDogPark.ReviewerId);
        }

        [Fact]
        public async Task CreateReview_Valid_DogPark_creates_and_returns_DogPark_id()
        {
            var reviewerId = 2;
            var dto = new ReviewCreateDTO
            {
                ReviewType = Enums.ReviewType.DogPark,
                RevieweeId = 2,
                Description = "description",
                Title = "title",
                Rating = 4
            };

            var actual = await _repo.CreateReviewAsync(reviewerId, dto);
            var actualEntity = await _context.Reviews.Where(r =>
                r.RevieweeId == dto.RevieweeId &&
                r.ReviewerId == reviewerId &&
                r.ReviewType == dto.ReviewType
            ).FirstOrDefaultAsync();

            Assert.NotNull(actual);
            Assert.NotNull(actualEntity);
            Assert.Equal(ResponseType.Created, actual.ResponseType);
            Assert.Equal(dto.ReviewType, actual.ReviewType);
            Assert.Equal(dto.RevieweeId, actual.RevieweeId);
            Assert.Equal(reviewerId, actual.ReviewerId);
            Assert.Equal(dto.ReviewType, actualEntity.ReviewType);
            Assert.Equal(dto.RevieweeId, actualEntity.RevieweeId);
            Assert.Equal(reviewerId, actualEntity.ReviewerId);
            Assert.Equal(dto.Title, actualEntity.Title);
            Assert.Equal(dto.Description, actualEntity.Description);
            Assert.Equal(dto.Rating, actualEntity.Rating);
        }

        [Fact]
        public async Task CreateReview_Valid_DogPark_updates_and_returns_DogPark_id()
        {
            var reviewerId = 1;
            var dto = new ReviewCreateDTO
            {
                ReviewType = Enums.ReviewType.DogPark,
                RevieweeId = 1,
                Description = "description",
                Title = "title",
                Rating = 4
            };

            var actual = await _repo.CreateReviewAsync(reviewerId, dto);
            var actualEntity = await _context.Reviews.Where(r =>
                r.RevieweeId == dto.RevieweeId &&
                r.ReviewerId == reviewerId &&
                r.ReviewType == dto.ReviewType
            ).FirstOrDefaultAsync();

            Assert.NotNull(actual);
            Assert.NotNull(actualEntity);
            Assert.Equal(ResponseType.Updated, actual.ResponseType);
            Assert.Equal(dto.ReviewType, actual.ReviewType);
            Assert.Equal(dto.RevieweeId, actual.RevieweeId);
            Assert.Equal(reviewerId, actual.ReviewerId);
            Assert.Equal(dto.ReviewType, actualEntity.ReviewType);
            Assert.Equal(dto.RevieweeId, actualEntity.RevieweeId);
            Assert.Equal(reviewerId, actualEntity.ReviewerId);
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
                ReviewType = Enums.ReviewType.DogPark,
                RevieweeId = -1,
                Description = "description",
                Title = "title",
                Rating = 4
            };
            var entitiesBefore = await _context.Reviews.CountAsync();
            var actual = await _repo.CreateReviewAsync(reviewerId, dto);
            var entitiesAfter = await _context.Reviews.CountAsync();

            Assert.Equal(entitiesBefore, entitiesAfter);
            Assert.Equal(ResponseType.NotFound, actual.ResponseType);


        }
    }
}
