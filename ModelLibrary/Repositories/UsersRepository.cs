using EntityLib;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Repositories
{
    public interface IUsersRepository
    {
        public Task<UserSearchPaginationResultDTO> SearchUsers(UserSearchDTOPagination request);
    }

    public class UsersRepository : RepositoryBase, IUsersRepository
    {

        public UsersRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserSearchPaginationResultDTO> SearchUsers(UserSearchDTOPagination request)
        {
            var query = _context.Users
                .Where(u => (u.FirstName + " " + u.LastName)
                .StartsWith(request.SearchString))
                .OrderBy(u => u.FirstName + " " + u.LastName);

            var (pagination, paginationQuery) = await RepositoryUtils.GetPaginationQuery(query, request.Pagination);

            var dtoResult = await paginationQuery.Select(u => new UserListDTO
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                ProfilePictureUrl = u.ProfileImageUrl,
                UserId = u.Id
            }).ToListAsync();

            return new UserSearchPaginationResultDTO
            {
                Pagination = pagination,
                Users = dtoResult
            };
            
        }
    }
}
