using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Constants;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.Users;
using ModelLib.Repositories;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.USERS)]
    [ApiController]
    public class UsersController : CustomControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        public UsersController(UserManager<ApplicationUser> userManager, IUsersRepository usersRepository) : base(userManager)
        {
            _usersRepository = usersRepository;
        }

        [HttpPost(ApiEndpoints.USERS_SEARCH)]
        public async Task<ActionResult<PaginationResult<UserListDTO>>> SearchUsers([FromBody] UserSearchDTOPaginationRequest request)
        {
            return Ok(await _usersRepository.SearchUsersAsync(request));
        }

        [Authorize]
        [HttpGet(ApiEndpoints.USERS_GET + "{id}")]
        public async Task<ActionResult<UserDetailedDTO>> GetUsers(int id)
        {
            return await _usersRepository.GetUserAsync(autherizedUserId: (await GetAuthorizedUserIdAsync()).Value, targetUserId: id);
        }

        [Authorize]
        [HttpPut(ApiEndpoints.USERS_UPDATE)]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
        {
            var user = await GetAuthorizedUserIdAsync();
            var response = await _usersRepository.UpdateProfile(user.Value, dto);
            return Ok(response);
        }
    }
}
