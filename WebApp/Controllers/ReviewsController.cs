using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.REVIEWS)]
    [ApiController]
    public class ReviewsController
    {
        private readonly IReviewsRepository _reviewsRepository;

        public ReviewsController(IReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        [Authorize]
        [HttpGet(ApiEndpoints.GET_REVIEWS)]
        public async Task<ActionResult<(PaginationResult, List<ReviewDetailedDTO>)>> GetReviews(ReviewsDTO dto)
        {
            return await _reviewsRepository.GetReviewsAsync(dto);
        }
    }
}
