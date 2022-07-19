using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class FriendsRepositoryTests : TestBase
    {
        private readonly IFriendsRepository _friendsRepository;

        public FriendsRepositoryTests()
        {
            _friendsRepository = new FriendsRepository(_context);
        }

        [Theory]
        [InlineData(1, 2, ResponseType.Created, false)]
        [InlineData(1, -1, ResponseType.NotFound, true)]
        [InlineData(1, 3, ResponseType.Conflict, true)]
        [InlineData(1, 1, ResponseType.Conflict, true)]
        public async Task AddFriendRequestAsync_Given_Input_Returns_Expected(int userId, int friendId, ResponseType expectedResponse, bool expectNull)
        {
            var result = await _friendsRepository.CreateFriendRequestAsync(userId, friendId);
            var friendsRelation = await _context.Friends.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            var shouldBeNull = await _context.Friends.FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);

            Assert.Equal(expectedResponse, result);
            if (!expectNull)
            {
                Assert.NotNull(friendsRelation);
                Assert.False(friendsRelation.IsAccepted);
            }
        }

        [Theory]
        [InlineData(1, 3, ResponseType.Updated)]
        [InlineData(1, 2, ResponseType.NotFound)]
        public async Task AcceptFriendRequestAsync_Given_Input_Returns_Expected(int userId, int friendId, ResponseType expectedResponse)
        {
            var response = await _friendsRepository.AcceptFriendRequestAsync(userId, friendId);
            Assert.Equal(expectedResponse, response);
        }

        [Fact]
        public async Task GetFriends_Given_User_With_Two_Friends_And_Opposite_Requesters_Returns_Two_Friends()
        {
            var userId = 1;
            var expectedFriends = 2;
            var expectedFriend1 = 4;
            var expectedFriend2 = 5;
            var pagination = new PaginationRequest
            {
                ItemsPerPage = 10,
                Page = 0
            };

            var result = await _friendsRepository.GetFriendsAsync(userId, pagination);
            Assert.False(result.PaginationResult.HasNext);
            Assert.Equal(expectedFriends, result.Friends.Count);
            Assert.Equal(expectedFriend1, result.Friends.First().UserId);
            Assert.Equal(expectedFriend2, result.Friends.Last().UserId);
        }

        [Theory]
        [InlineData(1,4, ResponseType.Deleted)]
        [InlineData(4,1, ResponseType.Deleted)]
        [InlineData(1,2, ResponseType.NotFound)]
        public async Task RemoveFriendAsync_Given_Input_Returns_Expected(int userId, int friendId, ResponseType expectedRespone)
        {
            var countBefore = await _context.Friends.CountAsync();
            var result = await _friendsRepository.RemoveFriendAsync(userId, friendId);
            var countAfter = await _context.Friends.CountAsync();
            if (expectedRespone == ResponseType.Deleted)
            {
                Assert.Equal(countBefore - 1, countAfter);
            }
            Assert.Equal(expectedRespone, result);
        }

        [Theory]
        [InlineData(3,1, ResponseType.Deleted)]
        [InlineData(1,3, ResponseType.NotFound)]
        public async Task RemoveFriendRequest_Given_Input_Returns_Expected(int userId, int friendId, ResponseType expectedResponse)
        {
            var response = await _friendsRepository.DeclineFriendRequestAsync(userId, friendId);
            var relation = await _context.Friends.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            Assert.Null(relation);
            Assert.Equal(expectedResponse, response);
        }

        [Theory]
        [InlineData(3,1, FriendShipStatus.PendingRequest)]
        [InlineData(1,3, FriendShipStatus.CanAcceptRequest)]
        [InlineData(1,2, FriendShipStatus.NotFriends)]
        [InlineData(4,1, FriendShipStatus.Friends)]
        [InlineData(1,4, FriendShipStatus.Friends)]
        public async Task GetFriendShipStatus_Given_Input_Returns_Expected(int userId, int friendId, FriendShipStatus expectedResponse)
        {
            var result = await _friendsRepository.GetFriendShipStatusAsync(userId, friendId);
            Assert.Equal(expectedResponse, result);
        }

        [Theory]
        [InlineData(3,0, -1)]
        [InlineData(1,1, 3)]
        public async Task GetFriendRequests_Given_Input_Returns_Expected(int userId, int expectedCount, int expectedFriendId)
        {
            var pagination = new PaginationRequest
            {
                ItemsPerPage = 10,
                Page = 0
            };
            var result = await _friendsRepository.GetFriendRequestsAsync(userId, pagination);
            Assert.Equal(expectedCount, result.Friends.Count);
            if (expectedCount == 1)
            {
                Assert.Equal(expectedFriendId, result.Friends.First().UserId);
            }
        }
    }
}
