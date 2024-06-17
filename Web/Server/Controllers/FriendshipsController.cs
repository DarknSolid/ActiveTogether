using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Constants;
using ModelLib.DTOs.Authentication;
using ModelLib.Repositories;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route(ApiEndpoints.FRIENDSHIPS)]
    [Authorize]
    public class FriendshipsController : CustomControllerBase
    {
        private readonly IFriendshipsRepository _friendsRepository;

        public FriendshipsController(UserManager<ApplicationUser> userManager, IFriendshipsRepository friendsRepository) : base(userManager)
        {
            _friendsRepository = friendsRepository;
        }

        [HttpPost(ApiEndpoints.FRIENDSHIPS_ADD + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> AddFriend(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.CreateFriendRequestAsync((await GetAuthorizedUserIdAsync()).Value, id));

        }

        [HttpDelete(ApiEndpoints.FRIENDSHIPS_REMOVE + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> RemoveFriend(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.RemoveFriendAsync((await GetAuthorizedUserIdAsync()).Value, id));

        }

        [HttpPut(ApiEndpoints.FRIENDSHIPS_ACCEPT + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> AcceptFriendRequest(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.AcceptFriendRequestAsync((await GetAuthorizedUserIdAsync()).Value, id));

        }

        [HttpPut(ApiEndpoints.FRIENDSHIPS_DECLINE + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> DeclineFriendRequest(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.DeclineFriendRequestAsync(id, (await GetAuthorizedUserIdAsync()).Value));
        }

        [HttpGet(ApiEndpoints.FRIENDSIPS_STATUS + "{id}")]
        public async Task<ActionResult<RepositoryEnums.FriendShipStatus>> GetFriendStatus(int id)
        {
            return Ok(await _friendsRepository.GetFriendShipStatusAsync((await GetAuthorizedUserIdAsync()).Value, id));
        }

        [HttpPost(ApiEndpoints.FRIENDSHIPS_ALL)]
        public async Task<ActionResult<PaginationResult<UserListDTO>>> GetFriends([FromBody] StringPaginationRequest pagination)
        {
            return await _friendsRepository.GetFriendsAsync((await GetAuthorizedUserIdAsync()).Value, pagination);
        }

        [HttpPost(ApiEndpoints.FRIENDSHIPS_REQUESTS)]
        public async Task<ActionResult<PaginationResult<UserListDTO>>> GetFriendRequests([FromBody] StringPaginationRequest pagination)
        {
            return await _friendsRepository.GetFriendRequestsAsync((await GetAuthorizedUserIdAsync()).Value, pagination);
        }



    }
}
