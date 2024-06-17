using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Constants;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Instructors;
using ModelLib.Repositories;
using Web.Server.Constants;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.DOG_PARKS)]
    [ApiController]
    public class DogParkController : CustomControllerBase
    {
        private readonly IDogParkRepository _dogParkRepository;

        public DogParkController(UserManager<ApplicationUser> userManager, IDogParkRepository dogParkRepository) : base(userManager)
        {
            _dogParkRepository = dogParkRepository;
        }

        [HttpGet(ApiEndpoints.DOG_PARKS_GET + "{dogParkId}")]
        public async Task<ActionResult<DogParkDetailedDTO?>> Get(int dogParkId)
        {
            return await _dogParkRepository.GetAsync((await GetAuthorizedUserIdAsync()), dogParkId);
        }

        [Authorize]
        [HttpPost(ApiEndpoints.DOG_PARKS_CREATE_REQUEST)]
        public async Task<ActionResult<int>> CreateRequest([FromBody] DogParkRequestCreateDTO dto)
        {
            var (response, result) = await _dogParkRepository.RequestAsync((await GetAuthorizedUserIdAsync()).Value, dto);
            return ResolveRepositoryResponse(response, result);
        }

        [HttpPost(ApiEndpoints.DOG_PARKS_GET_REQUESTS)]
        public async Task<ActionResult<DateTimePaginationResult<DogParkRequestDetailedDTO>>> GetRequests([FromBody] DateTimePaginationRequest dto)
        {
            return await _dogParkRepository.GetRequestsAsync((await GetAuthorizedUserIdAsync()).Value, dto);
        }

        [Authorize]
        [HttpPost(ApiEndpoints.DOG_PARKS_GET_APPROVED_DOG_PARKS)]
        public async Task<ActionResult<DateTimePaginationResult<DogParkListDTO>>> GetApprovedParks([FromBody] DateTimePaginationRequest dto)
        {
            return await _dogParkRepository.GetCreatedByAuthorAsync((await GetAuthorizedUserIdAsync()).Value, dto);
        }


        [HttpPost(ApiEndpoints.DOG_PARKS_APPROVE_DOG_PARK + "{requestId}")]
        [Authorize(Roles = RoleConstants.ADMIN)]
        public async Task<IActionResult> GetApprovedParks(int requestId)
        {
            var (response, result) = await _dogParkRepository.ApproveDogParkRequestAsync(requestId);
            return ResolveRepositoryResponse(response, result);
        }

        [HttpPost(ApiEndpoints.DOG_PARKS_LIST)]
        public async Task<ActionResult<DistancePaginationResult<DogParkListDTO>>> GetList([FromBody] DogParksDTOPaginationRequest request)
        {
            var instructors = await _dogParkRepository.GetParksAsync(request);
            return Ok(instructors);
        }

        [HttpPost(ApiEndpoints.DOG_PARKS_AREA)]
        public async Task<ActionResult<IEnumerable<DogParkListDTO>>> GetArea([FromBody] SearchAreaDTO searchArea)
        {
            var instructors = await _dogParkRepository.GetParksInAreaAsync(searchArea);
            return Ok(instructors);
        }

    }
}
