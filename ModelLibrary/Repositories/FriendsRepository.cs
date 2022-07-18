using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Authentication;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IFriendsRepository
    {
        public Task<ResponseType> AddFriendRequestAsync(int userId, int friendId);
        public Task<ResponseType> RemoveFriendRequest(int userId, int friendId);
        public Task<ResponseType> AcceptFriendRequestAsync(int userId, int friendRequesterId);
        public Task<ResponseType> RemoveFriendAsync(int userId, int friendId);
        public Task<UserListPaginationDTO> GetFriends(int userId, PaginationRequest pagination);
        public Task<FriendShipStatus> GetFriendShipStatus(int userId, int friendId);
        public Task<UserListPaginationDTO> GetFriendRequests(int userId, PaginationRequest pagination);
    }

    public class FriendsRepository : IFriendsRepository
    {
        private readonly IApplicationDbContext _context;

        public FriendsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseType> AddFriendRequestAsync(int userId, int friendId)
        {
            // if the user has already sent a request, or if they are already friends, or if the friend has sent a request to the user:
            var existingFriendRelation = await _context.Friends
                .FirstOrDefaultAsync(x => 
                    (x.UserId == userId && x.FriendId == friendId) || 
                    (x.UserId == friendId && x.FriendId == userId));
            if (existingFriendRelation != null)
            {
                return ResponseType.Conflict;
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == friendId);
            if (existingUser == null)
            {
                return ResponseType.NotFound;
            }

            var newPendingFriendRequest = new Friends
            {
                UserId = userId,
                FriendId = friendId,
                IsAccepted = false
            };

            _context.Friends.Add(newPendingFriendRequest);
            await _context.SaveChangesAsync();
            return ResponseType.Created;
        }

        public async Task<ResponseType> AcceptFriendRequestAsync(int userId, int friendRequesterId)
        {
            var request = await _context.Friends.FirstOrDefaultAsync(f => f.UserId == friendRequesterId && f.FriendId == userId);
            if (request == null)
            {
                return ResponseType.NotFound;
            }

            request.IsAccepted = true;
            await _context.SaveChangesAsync();
            return ResponseType.Updated;
        }


        public async Task<UserListPaginationDTO> GetFriends(int userId, PaginationRequest pagination)
        {
            var friends = _context.Friends
                .Include(f => f.Friend)
                .Where(f => f.UserId == userId && f.IsAccepted == true)
                .Select(f => new UserListDTO 
                { 
                    UserId = f.FriendId,
                    FirstName = f.Friend.FirstName,
                    LastName = f.Friend.LastName,
                    ProfilePictureUrl = f.Friend.ProfileImageUrl
                })
                .Union(_context.Friends
                    .Include(f => f.User)
                    .Where(f => f.FriendId == userId && f.IsAccepted == true)
                    .Select(f => new UserListDTO
                        {
                            FirstName = f.User.FirstName,
                            LastName = f.User.LastName,
                            ProfilePictureUrl = f.User.ProfileImageUrl,
                            UserId = f.UserId
                        }
                    )
                );
            friends = friends.OrderBy(u => u.FirstName);
            var (paginationResult, paginationQuery) = await RepositoryUtils.GetPaginationQuery(friends, pagination);

            return new UserListPaginationDTO
            {
                Friends = await paginationQuery.ToListAsync(),
                PaginationResult = paginationResult
            };

        }

        public async Task<ResponseType> RemoveFriendAsync(int userId, int friendId)
        {
            var friendRelation = await _context.Friends.FirstOrDefaultAsync(f => ((f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)) && f.IsAccepted == true);
            if (friendRelation == null)
            {
                return ResponseType.NotFound;
            }
            _context.Friends.Remove(friendRelation);
            await _context.SaveChangesAsync();

            return ResponseType.Deleted;
        }

        public async Task<ResponseType> RemoveFriendRequest(int userId, int friendId)
        {
            var relation = await _context.Friends.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId && f.IsAccepted == false);
            if (relation == null)
            {
                return ResponseType.NotFound;
            }
            _context.Friends.Remove(relation);
            await _context.SaveChangesAsync();
            return ResponseType.Deleted;
        }

        public async Task<FriendShipStatus> GetFriendShipStatus(int userId, int friendId)
        {
            var friendRelation = await _context.Friends
                .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
            if (friendRelation == null)
            {
                return FriendShipStatus.NotFriends;
            }
            else if (friendRelation.IsAccepted == false)
            {
                if (friendRelation.UserId == userId)
                {
                    return FriendShipStatus.PendingRequest;
                }
                else
                {
                    return FriendShipStatus.CanAcceptRequest;
                }
            }
            else
            {
                return FriendShipStatus.Friends;
            }
        }

        public async Task<UserListPaginationDTO> GetFriendRequests(int userId, PaginationRequest paginationRequest)
        {
            var requests = _context.Friends
                .Include(f => f.User)
                .Where(f => f.FriendId == userId && f.IsAccepted == false)
                .OrderBy(f => f.User.UserName + " " + f.User.LastName)
                .Select(f => new UserListDTO
                {
                    FirstName = f.User.FirstName,
                    LastName = f.User.LastName,
                    ProfilePictureUrl = f.User.ProfileImageUrl,
                    UserId = f.UserId
                });

            var (pagination, paginatedQuery) = await RepositoryUtils.GetPaginationQuery(requests, paginationRequest);
            return new UserListPaginationDTO
            {
                Friends = await paginatedQuery.ToListAsync(),
                PaginationResult = pagination
            };
        }
    }
}
