using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using Moq;
using static EntityLib.Entities.Enums;
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
            var request = new ReviewsDTO { PlaceId = 1, PaginationRequest = paginationRequest };
            var expectedCount = 2;

            var (pagination, dogParkReviews) = await _repo.GetReviewsAsync(request);
            var firstDogPark = dogParkReviews.First();
            var secondDogPark = dogParkReviews[1];

            Assert.Equal(expectedCount, dogParkReviews.Count);
            Assert.Equal(request.PlaceId, firstDogPark.PlaceId);
            Assert.Equal(request.PlaceId, secondDogPark.PlaceId);
            Assert.Equal(2, firstDogPark.ReviewerId);
            Assert.Equal(1, secondDogPark.ReviewerId);
        }

        [Fact]
        public async Task CreateReview_Valid_DogPark_creates_and_returns_DogPark_id()
        {
            var reviewerId = 2;
            var dto = new ReviewCreateDTO
            {
                PlaceId = 2,
                Description = "description",
                Title = "title",
                Rating = 4
            };

            var (actualResponse, actualDto) = await _repo.CreateReviewAsync(reviewerId, dto);
            var actualEntity = await _context.Reviews.Where(r =>
                r.PlaceId == dto.PlaceId &&
                r.UserId == reviewerId
            ).FirstOrDefaultAsync();

            Assert.NotNull(dto);
            Assert.NotNull(actualEntity);
            Assert.Equal(ResponseType.Created, actualResponse);
            Assert.Equal(dto.PlaceId, actualDto.RevieweeId);
            Assert.Equal(reviewerId, actualDto.ReviewerId);
            Assert.Equal(dto.PlaceId, actualEntity.PlaceId);
            Assert.Equal(reviewerId, actualEntity.UserId);
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
                PlaceId = 1,
                Description = "description",
                Title = "title",
                Rating = 4
            };

            var (actualResponse, actualDto) = await _repo.CreateReviewAsync(reviewerId, dto);
            var actualEntity = await _context.Reviews.Where(r =>
                r.PlaceId == dto.PlaceId &&
                r.UserId == reviewerId
            ).FirstOrDefaultAsync();

            Assert.NotNull(actualDto);
            Assert.NotNull(actualEntity);
            Assert.Equal(ResponseType.Updated, actualResponse);
            Assert.Equal(dto.PlaceId, actualDto.RevieweeId);
            Assert.Equal(reviewerId, actualDto.ReviewerId);
            Assert.Equal(dto.PlaceId, actualEntity.PlaceId);
            Assert.Equal(reviewerId, actualEntity.UserId);
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
                PlaceId = -1,
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
    }
}
