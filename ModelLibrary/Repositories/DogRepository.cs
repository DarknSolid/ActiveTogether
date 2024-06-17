using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.DTOs.Dogs;

namespace ModelLib.Repositories
{
    public interface IDogRepository
    {
        public Task<(RepositoryEnums.ResponseType, int)> CreateAsync(int userId, DogCreateDTO dto);
        public Task<(RepositoryEnums.ResponseType, int)> UpdateAsync(int userId, DogUpdateDTO dto);
        public Task<RepositoryEnums.ResponseType> DeleteDogAsync(int userId, int dogId);
        public Task<List<DogListDTO>> GetAsync(int userId);
        public Task<DogDetailedDTO?> GetDetailedAsync(int dogId);
    }


    public class DogRepository : RepositoryBase, IDogRepository
    {
        private readonly IBlobStorageRepository _blobStorageRepository;

        public DogRepository(IApplicationDbContext context, IBlobStorageRepository blobStorageRepository) : base(context)
        {
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task<(RepositoryEnums.ResponseType, int)> CreateAsync(int userId, DogCreateDTO dto)
        {
            var entity = new Dog
            {
                Birth = dto.Birth.ToUniversalTime(),
                Description = dto.Description,
                Race = dto.Race,
                IsGenderMale = dto.IsGenderMale,
                Name = dto.Name,
                UserId = userId,
                WeightClass = dto.WeightClass
            };
            _context.Dogs.Add(entity);
            await _context.SaveChangesAsync();

            if (dto.ProfilePicture != null)
            {
                entity.ProfilePictureUrl = (await _blobStorageRepository.UploadPublicImageAsync(dto.ProfilePicture, BlobStorageRepository.DogProfilePicture, entity.Id)).Item2;
                await _context.SaveChangesAsync();
            }

            return new
            (
                RepositoryEnums.ResponseType.Created,
                entity.Id
            );
        }

        public async Task<RepositoryEnums.ResponseType> DeleteDogAsync(int userId, int dogId)
        {
            var entity = await _context.Dogs.FirstOrDefaultAsync(d => d.Id == dogId && d.UserId == userId);
            if (entity == null)
            {
                return RepositoryEnums.ResponseType.NotFound;
            }
            _context.Dogs.Remove(entity);
            await _context.SaveChangesAsync();
            await _blobStorageRepository.DeletePublicImageAsync(entity.ProfilePictureUrl);
            return RepositoryEnums.ResponseType.Deleted;
        }

        public async Task<List<DogListDTO>> GetAsync(int userId)
        {
            var result = await _context.Dogs.Where(d => d.UserId == userId)
                .Select(d => new DogListDTO
                {
                    Birth = d.Birth,
                    Race = d.Race,
                    Id = d.Id,
                    IsGenderMale = d.IsGenderMale,
                    Name = d.Name,
                    ProfilePictureUrl = d.ProfilePictureUrl
                }).ToListAsync();

            foreach (var d in result)
            {
                d.ProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(d.ProfilePictureUrl)).Item2?.ToString();
            }

            return result;
        }

        public async Task<DogDetailedDTO?> GetDetailedAsync(int dogId)
        {
            var dto = await _context.Dogs.Where(d => d.Id == dogId)
                .Select(entity => new DogDetailedDTO
                {
                    Birth = entity.Birth,
                    Description = entity.Description,
                    Race = entity.Race,
                    IsGenderMale = entity.IsGenderMale,
                    Name = entity.Name,
                    UserId = entity.UserId,
                    WeightClass = entity.WeightClass,
                    Id = entity.Id,
                    ProfilePictureUrl = entity.ProfilePictureUrl,
                    UserFirstName = entity.User.FirstName,
                    UserLastName = entity.User.LastName,
                    UserProfilePictureUrl = entity.User.ProfileImageUrl
                }).FirstOrDefaultAsync();

            if (dto != null)
            {
                dto.ProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(dto.ProfilePictureUrl)).Item2?.ToString();
                dto.UserProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(dto.UserProfilePictureUrl)).Item2?.ToString();
            }

            return dto;
        }

        public async Task<(RepositoryEnums.ResponseType, int)> UpdateAsync(int userId, DogUpdateDTO dto)
        {
            var entity = await _context.Dogs.Where(d => d.Id == dto.Id && d.UserId == userId).FirstOrDefaultAsync();

            // if it does not exist or does not belong to the user
            if (entity == null)
            {
                return new(
                    RepositoryEnums.ResponseType.NotFound,
                    -1
                );
            }

            entity.Birth = dto.Birth.ToUniversalTime();
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.IsGenderMale = dto.IsGenderMale;
            entity.Race = dto.Race;
            entity.WeightClass = dto.WeightClass;

            if (dto.ProfilePicture is not null)
            {
                if (dto.ProfilePicture.IsDeleteCommand)
                {
                    await _blobStorageRepository.DeletePublicImageAsync(entity.ProfilePictureUrl);
                    entity.ProfilePictureUrl = null;
                }
                else
                {
                    entity.ProfilePictureUrl = (await _blobStorageRepository.UploadPublicImageAsync(dto.ProfilePicture, BlobStorageRepository.DogProfilePicture, entity.Id)).Item2?.ToString();
                }
            }

            _context.Dogs.Update(entity);
            await _context.SaveChangesAsync();
            return new(
                RepositoryEnums.ResponseType.Updated,
                entity.Id
            );
        }
    }
}
