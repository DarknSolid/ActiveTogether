using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class UsersRepositoryTests : TestBase
    {

        private readonly IUsersRepository _usersRepository;
        private readonly Mock<IFriendshipsRepository> _mock;
        private readonly Mock<ICompanyRepository> _companyRepository;
        private readonly Mock<IBlobStorageRepository> _blobStorageRepository;

        public UsersRepositoryTests()
        {
            _mock = new Mock<IFriendshipsRepository>();
            _companyRepository = new Mock<ICompanyRepository>();
            _blobStorageRepository = new Mock<IBlobStorageRepository>();
            _usersRepository = new UsersRepository(_context, _mock.Object, _companyRepository.Object, _blobStorageRepository.Object);
        }

        [Theory]
        [InlineData("A", 4)]
        [InlineData("a", 4)]
        public async Task SearchUsersAsync_Given_Valid_Prefix_Returns_Users(string searchString, int expectedResultCount)
        {
            var paginationRequest = new StringPaginationRequest
            {
                ItemsPerPage = 10,
                Page = 0,
                LastId = -1,
                LastString = ""
            };
            var request = new UserSearchDTOPaginationRequest
            {
                PaginationRequest = paginationRequest,
                SearchString = searchString
            };

            var result = await _usersRepository.SearchUsersAsync(request);

            Assert.Equal(expectedResultCount, result.Result.Count);
        }

        [Fact]
        public async Task SearchUsersAsync_Given_Invalid_Prefix_Returns_Empty()
        {
            var searchString = "Z";
            var paginationRequest = new StringPaginationRequest
            {
                ItemsPerPage = 10,
                Page = 0,
                LastId = -1,
                LastString = ""
            };
            var request = new UserSearchDTOPaginationRequest
            {
                PaginationRequest = paginationRequest,
                SearchString = searchString
            };

            var result = await _usersRepository.SearchUsersAsync(request);

            Assert.Empty(result.Result);
        }

        [Fact]
        public async Task SearchUsersAsync_Page_1_Returns_Corresponding_User()
        {
            var searchString = "A";
            var paginationRequest = new StringPaginationRequest
            {
                ItemsPerPage = 1,
                Page = 1,
                LastId = 10,
                LastString = "A"
            };
            var request = new UserSearchDTOPaginationRequest
            {
                PaginationRequest = paginationRequest,
                SearchString = searchString
            };

            var result = await _usersRepository.SearchUsersAsync(request);

            Assert.Equal(7, result.Result.First().Id);
            Assert.Equal("test7 user7", result.Result.First().FullName);
            Assert.Equal("AB", result.Result.First().FullNameNormalized);
        }

        [Fact]
        public async Task GetUserAsync_Valid_User_Returns_User_With_Expected_Friendship_Status()
        {
            _mock.Setup(f => f.GetFriendShipStatusAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(FriendShipStatus.PendingRequest);
            var result = await _usersRepository.GetUserAsync(autherizedUserId: 1, targetUserId: 2);

            Assert.Equal(2, result.Id);
            Assert.Equal(FriendShipStatus.PendingRequest, result.FriendShipStatus);
            _mock.Verify(_ => _.GetFriendShipStatusAsync(It.Is<int>(i => i == 1), It.Is<int>(i => i == 2)));
        }

        [Fact]
        public async Task GetUserAsync_Invalid_User_Returns_Null()
        {
            _mock.Setup(f => f.GetFriendShipStatusAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(FriendShipStatus.PendingRequest);
            var result = await _usersRepository.GetUserAsync(autherizedUserId: 1, targetUserId: -2);

            Assert.Null(result);
        }
    }
}
