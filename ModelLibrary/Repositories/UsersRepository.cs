using EntityLib;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.Dogs;
using ModelLib.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IUsersRepository
    {
        public Task<PaginationResult<UserListDTO>> SearchUsersAsync(UserSearchDTOPaginationRequest request);
        public Task<UserDetailedDTO> GetUserAsync(int targetUserId, int? autherizedUserId = null);
        public Task<ResponseType> UploadProfilePictureAsync(FileDetailedDTO profilePicture, int userId);
        Task<ResponseType> UpdateProfile(int userId, UserUpdateDTO updateDTO);
    }

    public class UsersRepository : RepositoryBase, IUsersRepository
    {

        private readonly IFriendshipsRepository _friendshipsRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IBlobStorageRepository _blobStorageRepository;

        public UsersRepository(IApplicationDbContext context, IFriendshipsRepository friendshipsRepository, ICompanyRepository companyRepository, IBlobStorageRepository blobStorageRepository) : base(context)
        {
            _friendshipsRepository = friendshipsRepository;
            _companyRepository = companyRepository;
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task<PaginationResult<UserListDTO>> SearchUsersAsync(UserSearchDTOPaginationRequest request)
        {
            var searchString = request.SearchString.ToUpper();
            var lastString = request.PaginationRequest.LastString.ToUpper();
            var lastId = request.PaginationRequest.LastId;

            var query = _context.Users
                .Where(u => u.FullNameNormalized.StartsWith(searchString));

            var orderByFunc = (IQueryable<ApplicationUser> query) =>
                query.OrderBy(u => u.FullNameNormalized).ThenBy(u => u.Id);

            var keyOffsetFunc = (IQueryable<ApplicationUser> query) =>
                query.Where(u => u.FullNameNormalized.CompareTo(lastString) > 0 || (u.FullNameNormalized.CompareTo(lastString) == 0 && u.Id > lastId));

            var (pagination, paginationQuery) = await RepositoryUtils.GetPaginationQuery(query, orderByFunc, keyOffsetFunc, request.PaginationRequest);

            var dtoResult = await paginationQuery.Select(u => new UserListDTO
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                ProfilePictureUrl = u.ProfileImageUrl,
                Id = u.Id,
                FullNameNormalized = u.FullNameNormalized,
            }).ToListAsync();

            foreach (var dto in dtoResult)
            {
                var (_, url) = await _blobStorageRepository.GetPublicImageUrl(dto.ProfilePictureUrl);
                if (url != null)
                {
                    dto.ProfilePictureUrl = url.ToString();
                }
            }

            return new PaginationResult<UserListDTO>
            {
                Total = pagination.Total,
                CurrentPage = pagination.CurrentPage,
                HasNext = pagination.HasNext,
                LastId = dtoResult.LastOrDefault()?.Id ?? -1,
                Result = dtoResult,
            };

        }

        public async Task<UserDetailedDTO?> GetUserAsync(int targetUserId, int? authorizedUserId = null)
        {
            var entity = await _context.Users
                .Include(u => u.Dogs)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.FullNameNormalized,
                    u.ProfileImageUrl,
                    u.Email,
                    u.Dogs
                })
                .FirstOrDefaultAsync(u => u.Id == targetUserId);
            if (entity == null)
            {
                return null;
            }
            var company = await _companyRepository.GetCompanyUserInfo(targetUserId);

            var dto = new UserDetailedDTO
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                FullNameNormalized = entity.FullNameNormalized,
                Email = targetUserId == authorizedUserId ? entity.Email : null,
                FriendShipStatus = FriendShipStatus.NotFriends,
                HasCompany = company is not null,
                CompanyId = company?.CompanyId,
                CompanyType = company?.CompanyType,
                CompanyName = company?.CompanyName,
                CompanyProfilePictureUrl = company?.CompanyProfilePictureUrl,
                ProfilePictureUrl = entity.ProfileImageUrl,
                Dogs = entity.Dogs.Select(d => new DogListDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Race = d.Race,
                    Birth = d.Birth,
                    IsGenderMale = d.IsGenderMale,
                    ProfilePictureUrl = d.ProfilePictureUrl
                    
                }).ToList()
            };

            dto.ProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(dto.ProfilePictureUrl)).Item2?.ToString();

            foreach(var d in dto.Dogs)
            {
                d.ProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(d.ProfilePictureUrl)).Item2?.ToString();
            }

            if (authorizedUserId != null)
            {
                dto.FriendShipStatus = await _friendshipsRepository.GetFriendShipStatusAsync(authorizedUserId.Value, targetUserId);
            }

            return dto;
        }

        public async Task<ResponseType> UploadProfilePictureAsync(FileDetailedDTO profilePicture, int userId)
        {
            var (response, url) = await _blobStorageRepository.UploadPublicImageAsync(profilePicture, BlobStorageRepository.UserProfilePicture, userId);
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.ProfileImageUrl, u => url));
            return response;
        }

        public async Task<ResponseType> UpdateProfile(int userId, UserUpdateDTO updateDTO)
        {
            var userQuery = _context.Users.Single(u => u.Id == userId);
            if (updateDTO.FirstName is not null)
            {
                userQuery.FirstName = updateDTO.FirstName;
            }
            if (updateDTO.LastName is not null)
            {
                userQuery.LastName = updateDTO.LastName;

            }
            if (updateDTO.ProfilePicture is not null)
            {
                if (updateDTO.ProfilePicture.IsDeleteCommand)
                {
                    await _blobStorageRepository.DeletePublicImageAsync(BlobStorageRepository.UserProfilePicture, userId);
                    userQuery.ProfileImageUrl = null;
                }
                else if (updateDTO.ProfilePicture.ContainsFile())
                {
                    var (response, url) = await _blobStorageRepository.UploadPublicImageAsync(updateDTO.ProfilePicture, BlobStorageRepository.UserProfilePicture, userId);
                    userQuery.ProfileImageUrl = url;
                }
            }

            await _context.SaveChangesAsync();
            return ResponseType.Updated;
        }
    }

}
