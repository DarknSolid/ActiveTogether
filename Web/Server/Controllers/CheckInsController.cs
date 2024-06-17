using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Constants;
using ModelLib.DTOs.CheckIns;
using ModelLib.Repositories;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.CHECKINS)]
    [Authorize]
    [ApiController]
    public class CheckInsController : CustomControllerBase
    {
        private readonly ICheckInRepository _checkInRepository;

        public CheckInsController(ICheckInRepository checkInRepository, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _checkInRepository = checkInRepository;
        }

        [HttpPost(ApiEndpoints.CHECKINS_CHECK_IN)]
        public async Task<ActionResult<int>> CheckIn([FromBody] CheckInCreateDTO dto)
        {
            var (response, id) = await _checkInRepository.CheckIn((await GetAuthorizedUserIdAsync()).Value, dto);
            return ResolveRepositoryResponse(response, id);
        }

        [HttpPut(ApiEndpoints.CHECKINS_CHECK_OUT)]
        public async Task<ActionResult<int>> CheckOut()
        {
            var (response, id) = await _checkInRepository.CheckOut((await GetAuthorizedUserIdAsync()).Value);
            return ResolveRepositoryResponse(response, id);
        }

        [AllowAnonymous]
        [HttpPost(ApiEndpoints.CHECKINS_LIST)]
        public async Task<PaginationResult<CheckInListDTO>> GetCheckIns([FromBody] CheckInListDTOPaginationRequest dto)
        {
            var result = await _checkInRepository.GetCheckIns(dto);
            return result;
        }

        [HttpGet(ApiEndpoints.CHECKINS_CURRENT_CHECK_IN)]
        public async Task<CurrentlyCheckedInDTO?> GetCurrentCheckIns()
        {
            var result = await _checkInRepository.GetCurrentlyCheckedIn((await GetAuthorizedUserIdAsync()).Value);
            return result;
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.CHECKINS_STATISTICS + "{placeId}")]
        public async Task<CheckInStatisticsDetailedDTO> GetCheckInsStatistics(int placeId)
        {
            return await _checkInRepository.GetStatisticsAsync(placeId: placeId, lookBackDays: 30);
        }
    }
}
