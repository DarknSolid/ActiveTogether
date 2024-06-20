using EntityLib;
using Microsoft.EntityFrameworkCore;
using ModelLib.DTOs.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Repositories
{
    public interface ICompanyRepository
    {
        Task<CompanyUserInfoDTO?> GetCompanyUserInfo(int userId);
    }

    public class CompanyRepository : RepositoryBase, ICompanyRepository
    {
        private readonly IBlobStorageRepository _blobStorageRepository;

        public CompanyRepository(IApplicationDbContext context, IBlobStorageRepository blobStorageRepository) : base(context)
        {
            _blobStorageRepository = blobStorageRepository;
        }


        public async Task<CompanyUserInfoDTO?> GetCompanyUserInfo(int userId)
        {
            var company = await _context.Companies
                .Include(c => c.Place)
                .Where(c => c.ApplicationUserId == userId)
                .Select(c => new CompanyUserInfoDTO
                {
                    CompanyId = c.PlaceId,
                    CompanyType = c.Place.FacilityType,
                    CompanyName = c.Place.Name,
                    CompanyProfilePictureUrl = c.Place.ImageUrls[0]
                })
                .FirstOrDefaultAsync();

            if (company == null)
            {
                return null;
            }

            company.CompanyProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(company.CompanyProfilePictureUrl)).Item2?.ToString();
            return company;
        }
    }
}
