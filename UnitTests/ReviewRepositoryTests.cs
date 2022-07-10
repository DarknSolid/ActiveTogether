using EntityLib.Entities;
using ModelLib.ApiDTOs;
using ModelLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.Equal(2, firstDogPark.ReviwerId);
            Assert.Equal(1, secondDogPark.ReviwerId);
        }
    }
}
