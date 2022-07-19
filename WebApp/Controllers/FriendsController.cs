using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.Authentication;
using ModelLib.Repositories;

namespace WebApp.Controllers
{
    [ApiController]
    [Route(ApiEndpoints.FRIENDS)]
    [Authorize]
    public class FriendsController : CustomControllerBase
    {
        private readonly IFriendsRepository _friendsRepository;

        public FriendsController(UserManager<ApplicationUser> userManager, IFriendsRepository friendsRepository) : base(userManager)
        {
            _friendsRepository = friendsRepository;
        }

        [HttpPost(ApiEndpoints.FRIENDS_ADD + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> AddFriend(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.CreateFriendRequestAsync(await GetAuthorizedUserId(), id));

        }

        [HttpDelete(ApiEndpoints.FRIENDS_REMOVE + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> RemoveFriend(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.RemoveFriendAsync(await GetAuthorizedUserId(), id));

        }

        [HttpPut(ApiEndpoints.FRIENDS_ACCEPT + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> AcceptFriendRequest(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.AcceptFriendRequestAsync(await GetAuthorizedUserId(), id));

        }

        [HttpPut(ApiEndpoints.FRIENDS_DECLINE + "{id}")]
        public async Task<ActionResult<RepositoryEnums.ResponseType>> DeclineFriendRequest(int id)
        {
            return ResolveRepositoryResponse(await _friendsRepository.DeclineFriendRequestAsync(await GetAuthorizedUserId(), id));
        }

        [HttpGet(ApiEndpoints.FRIENDS_STATUS + "{id}")]
        public async Task<ActionResult<RepositoryEnums.FriendShipStatus>> GetFriendStatus(int id)
        {
            return Ok(await _friendsRepository.GetFriendShipStatusAsync(await GetAuthorizedUserId(), id));
        }

        [HttpPost(ApiEndpoints.FRIENDS_ALL)]
        public async Task<ActionResult<UserListPaginationDTO>> GetFriends([FromBody] PaginationRequest pagination)
        {
            return await _friendsRepository.GetFriendsAsync(await GetAuthorizedUserId(), pagination);
        }

        [HttpPost(ApiEndpoints.FRIENDS_REQUESTS)]
        public async Task<ActionResult<UserListPaginationDTO>> GetFriendRequests([FromBody] PaginationRequest pagination)
        {
            return await _friendsRepository.GetFriendRequestsAsync(await GetAuthorizedUserId(), pagination);
        }



    }
}
