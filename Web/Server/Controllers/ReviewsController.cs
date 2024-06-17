using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using Microsoft.AspNetCore.Identity;
using EntityLib.Entities.Identity;
using ModelLib.ApiDTOs.Pagination;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.REVIEWS)]
    [ApiController]
    public class ReviewsController : CustomControllerBase
    {
        private readonly IReviewsRepository _reviewsRepository;

        public ReviewsController(IReviewsRepository reviewsRepository, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _reviewsRepository = reviewsRepository;
        }

        [HttpPost(ApiEndpoints.REVIEWS_LIST)]
        public async Task<ActionResult<PaginationResult<ReviewDetailedDTO>>> GetReviews(ReviewsDTOPaginationRequest dto)
        {
            return Ok( await _reviewsRepository.GetReviewsAsync(dto));
        }

        [Authorize]
        [HttpPost(ApiEndpoints.REVIEWS_CREATE)]
        public async Task<ActionResult<ReviewCreatedDTO>> CreateReview(ReviewCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var (response, value) = await _reviewsRepository.CreateReviewAsync(user.Id, dto);
            return ResolveRepositoryResponse(response, value);
        }

        [HttpPost(ApiEndpoints.REVIEWS_DETAILS)]
        public async Task<ActionResult<ReviewDetailedDTO?>> GetReview([FromBody] ReviewGetDTO dto)
        {
            return await _reviewsRepository.GetReviewAsync(dto.UserId, dto.PlaceId);
        }
    }
}
