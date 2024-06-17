using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Authentication;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IFriendshipsRepository
    {
        public Task<ResponseType> CreateFriendRequestAsync(int userId, int friendId);
        public Task<ResponseType> DeclineFriendRequestAsync(int requesterId, int requesteeId);
        public Task<ResponseType> AcceptFriendRequestAsync(int userId, int friendRequesterId);
        public Task<ResponseType> RemoveFriendAsync(int userId, int friendId);
        public Task<PaginationResult<UserListDTO>> GetFriendsAsync(int userId, StringPaginationRequest request);
        public Task<FriendShipStatus> GetFriendShipStatusAsync(int userId, int friendId);
        public Task<PaginationResult<UserListDTO>> GetFriendRequestsAsync(int userId, StringPaginationRequest request);
    }

    public class FriendshipsRepository : RepositoryBase, IFriendshipsRepository
    {
        public FriendshipsRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<ResponseType> CreateFriendRequestAsync(int userId, int friendId)
        {
            if (userId == friendId)
            {
                return ResponseType.Conflict;
            }
            // if the user has already sent a request, or if they are already friends, or if the friend has sent a request to the user:
            var existingFriendRelation = await _context.Friendships
                .FirstOrDefaultAsync(x =>
                    (x.RequesterId == userId && x.RequesteeId == friendId) ||
                    (x.RequesterId == friendId && x.RequesteeId == userId));
            if (existingFriendRelation != null)
            {
                return ResponseType.Conflict;
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == friendId);
            if (existingUser == null)
            {
                return ResponseType.NotFound;
            }

            var newPendingFriendRequest = new Friendship
            {
                RequesterId = userId,
                RequesteeId = friendId,
                IsAccepted = false
            };

            _context.Friendships.Add(newPendingFriendRequest);
            await _context.SaveChangesAsync();
            return ResponseType.Created;
        }

        public async Task<ResponseType> AcceptFriendRequestAsync(int userId, int friendRequesterId)
        {
            var request = await _context.Friendships.FirstOrDefaultAsync(f => f.RequesterId == friendRequesterId && f.RequesteeId == userId);
            if (request == null)
            {
                return ResponseType.NotFound;
            }

            request.IsAccepted = true;
            await _context.SaveChangesAsync();
            return ResponseType.Updated;
        }


        public async Task<PaginationResult<UserListDTO>> GetFriendsAsync(int userId, StringPaginationRequest request)
        {
            var friends = _context.Friendships
                .Include(f => f.Requestee)
                .Include(f => f.Requester)
                .Where(f => (f.RequesterId == userId && f.IsAccepted == true) || (f.RequesteeId == userId && f.IsAccepted == true));
            
            var select = (IQueryable<Friendship> q) => q.Select(f => new UserListDTO
            {
                Id = f.RequesterId == userId ? f.RequesteeId : f.RequesterId,
                FirstName = f.RequesterId == userId ? f.Requestee.FirstName : f.Requester.FirstName,
                LastName = f.RequesterId == userId ? f.Requestee.LastName : f.Requester.LastName,
                FullNameNormalized = f.RequesterId == userId ? f.Requestee.FullNameNormalized: f.Requester.FullNameNormalized,
                ProfilePictureUrl = f.RequesterId == userId ? f.Requestee.ProfileImageUrl : f.Requester.ProfileImageUrl
            }
                    );
            var paginationResult = await RepositoryUtils.PaginateAsync(friends, select, request);
            return paginationResult;

        }

        public async Task<ResponseType> RemoveFriendAsync(int userId, int friendId)
        {
            var friendRelation = await _context.Friendships.FirstOrDefaultAsync(f => ((f.RequesterId == userId && f.RequesteeId == friendId) || (f.RequesterId == friendId && f.RequesteeId == userId)) && f.IsAccepted == true);
            if (friendRelation == null)
            {
                return ResponseType.NotFound;
            }
            _context.Friendships.Remove(friendRelation);
            await _context.SaveChangesAsync();

            return ResponseType.Deleted;
        }

        public async Task<ResponseType> DeclineFriendRequestAsync(int requesterId, int requesteeId)
        {
            var relation = await _context.Friendships.FirstOrDefaultAsync(f => f.RequesterId == requesterId && f.RequesteeId == requesteeId && f.IsAccepted == false);
            if (relation == null)
            {
                return ResponseType.NotFound;
            }
            _context.Friendships.Remove(relation);
            await _context.SaveChangesAsync();
            return ResponseType.Deleted;
        }

        public async Task<FriendShipStatus> GetFriendShipStatusAsync(int userId, int friendId)
        {
            var friendRelation = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == userId && f.RequesteeId == friendId) || (f.RequesterId == friendId && f.RequesteeId == userId));
            if (friendRelation == null)
            {
                return FriendShipStatus.NotFriends;
            }
            else if (friendRelation.IsAccepted == false)
            {
                if (friendRelation.RequesterId == userId)
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

        public async Task<PaginationResult<UserListDTO>> GetFriendRequestsAsync(int userId, StringPaginationRequest request)
        {
            var requests = _context.Friendships
                .Include(f => f.Requester)
                .Where(f => f.RequesteeId == userId && f.IsAccepted == false);
            var select = (IQueryable<Friendship> q) => q.Select(f => new UserListDTO
                {
                    FirstName = f.Requester.FirstName,
                    LastName = f.Requester.LastName,
                    FullNameNormalized = f.Requester.FullNameNormalized,
                    ProfilePictureUrl = f.Requester.ProfileImageUrl,
                    Id = f.RequesterId
                });

            var orderByFunc = (IQueryable<Friendship> query) =>
                query.OrderBy(f => f.Requester.FullNameNormalized);

            var keyOffsetFunc = (IQueryable<Friendship> query) =>
                query.Where(f => f.Requester.FullNameNormalized.CompareTo(request.LastString) > 0 || (f.Requester.FullNameNormalized.CompareTo(request.LastString) == 0 && f.RequesterId > request.LastId));

            var pagination = await RepositoryUtils.PaginateByStringAsync<Friendship, UserListDTO>(requests, orderByFunc, keyOffsetFunc, select, request);
            return pagination;
        }
    }
}
