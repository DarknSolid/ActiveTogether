using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using Microsoft.AspNetCore.Identity;
using EntityLib.Entities.Identity;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.REVIEWS)]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsRepository _reviewsRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(IReviewsRepository reviewsRepository, UserManager<ApplicationUser> userManager)
        {
            _reviewsRepository = reviewsRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost(ApiEndpoints.REVIEWS_LIST)]
        public async Task<ActionResult<ReviewListViewDTO>> GetReviews(ReviewsDTO dto)
        {
            var (pagination, list) = await _reviewsRepository.GetReviewsAsync(dto);
            return new ReviewListViewDTO { PaginationResult = pagination, Reviews = list };
        }

        [Authorize]
        [HttpPost(ApiEndpoints.REVIEWS_CREATE)]
        public async Task<ActionResult<ReviewCreatedDTO>> CreateReview(ReviewCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);

            return await _reviewsRepository.CreateReviewAsync(user.Id, dto);
        }
    }
}
