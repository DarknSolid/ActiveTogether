using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.CheckIns;
using ModelLib.Repositories;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.CHECKINS)]
    [Authorize]
    [ApiController]
    public class CheckInsController : CustomControllerBase
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckInsController(ICheckInRepository checkInRepository, UserManager<ApplicationUser> userManager)
        {
            _checkInRepository = checkInRepository;
            _userManager = userManager;
        }

        [HttpPost(ApiEndpoints.CHECKINS_CHECK_IN)]
        public async Task<ActionResult<int>> CheckIn([FromBody] CheckInCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var (response, id) = await _checkInRepository.CheckIn(user.Id, dto);
            return ResolveRepositoryResponse(response, id);
        }

        [HttpPut(ApiEndpoints.CHECKINS_CHECK_OUT)]
        public async Task<ActionResult<int>> CheckOut()
        {
            var user = await _userManager.GetUserAsync(User);
            var (response, id) = await _checkInRepository.CheckOut(user.Id);
            return ResolveRepositoryResponse(response, id);
        }

        [HttpPost(ApiEndpoints.CHECKINS_LIST)]
        public async Task<CheckInListDTOPagination> GetCheckIns([FromBody] GetCheckInListDTO dto)
        {
            var result = await _checkInRepository.GetCheckIns(dto);
            return result;
        }

        [HttpGet(ApiEndpoints.CHECKINS_CURRENT_CHECK_IN)]
        public async Task<CurrentlyCheckedInDTO?> GeturrentCheckIns()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _checkInRepository.GetCurrentlyCheckedIn(user.Id);
            return result;
        }
    }
}
