using EntityLib;
using EntityLib.Entities;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Dogs;

namespace ModelLib.Repositories
{
    public interface IDogRepository
    {
        public Task<DogCreatedDTO> CreateAsync(int userId, DogCreateDTO dto);
        public Task<DogUpdatedDTO> UpdateAsync(int userId, DogUpdateDTO dto);
        public Task<List<DogListDTO>> GetAsync(int userId);
        public Task<DogDetailedDTO?> GetDetailedAsync(int dogId);

        public Task<IDictionary<int, string>> GetDogBreedsAsync();
        public Task<int> CreateDogBreedAsync(string breed);
    }
    public class DogRepository : IDogRepository
    {
        private readonly IApplicationDbContext _context;

        public DogRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DogCreatedDTO> CreateAsync(int userId, DogCreateDTO dto)
        {
            if (await BreedExists(dto.Breed) == false)
            {
                return new DogCreatedDTO
                {
                    Id = -1,
                    Response = RepositoryEnums.ResponseType.Conflict
                };
            }
            var entity = new Dog
            {
                Birth = dto.Birth.ToUniversalTime(),
                Description = dto.Description,
                DogBreedId = dto.Breed,
                IsGenderMale = dto.IsGenderMale,
                Name = dto.Name,
                UserId = userId,
                WeightClass = dto.WeightClass
            };
            _context.Dogs.Add(entity);
            await _context.SaveChangesAsync();

            return new DogCreatedDTO
            {
                Id = entity.Id,
                Response = RepositoryEnums.ResponseType.Created
            };
        }

        public async Task<int> CreateDogBreedAsync(string breed)
        {
            breed = breed.ToLower();
            var existing = await _context.DogBreeds.FirstOrDefaultAsync(db => db.Name == breed);
            if (existing != null)
            {
                return existing.Id;
            }
            var entity = new DogBreed { Name = breed };
            _context.DogBreeds.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<List<DogListDTO>> GetAsync(int userId)
        {
            var result = _context.Dogs.Include(d => d.DogBreed).Where(d => d.UserId == userId)
                .Select(d => new DogListDTO
                {
                    Birth = d.Birth,
                    Breed = d.DogBreedId,
                    Id = d.Id,
                    IsGenderMale = d.IsGenderMale,
                    Name = d.Name,
                    BreedName = d.DogBreed.Name
                });

            return await result.ToListAsync();
        }

        public async Task<DogDetailedDTO?> GetDetailedAsync(int dogId)
        {
            var entity = await _context.Dogs.Include(d => d.DogBreed).Where(d => d.Id == dogId)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return null;
            }

            return new DogDetailedDTO
            {
                Birth = entity.Birth,
                Description = entity.Description,
                Breed = entity.DogBreedId,
                IsGenderMale = entity.IsGenderMale,
                Name = entity.Name,
                UserId = entity.UserId,
                WeightClass = entity.WeightClass,
                Id = entity.Id,
                BreedName = entity.DogBreed.Name
            };
        }

        public async Task<IDictionary<int, string>> GetDogBreedsAsync()
        {
            return await _context.DogBreeds.ToDictionaryAsync(d => d.Id, d => d.Name);
        }

        public async Task<DogUpdatedDTO> UpdateAsync(int userId, DogUpdateDTO dto)
        {
            var entity = await _context.Dogs.Where(d => d.Id == dto.Id).FirstOrDefaultAsync();

            // if it does not exist or does not belong to the user
            if (entity == null || entity.UserId != userId)
            {
                return new DogUpdatedDTO
                {
                    Id = -1,
                    Response = RepositoryEnums.ResponseType.NotFound
                };
            }

            if (await BreedExists(dto.Breed) == false)
            {
                return new DogUpdatedDTO
                {
                    Id = -1,
                    Response = RepositoryEnums.ResponseType.Conflict
                };
            }

            entity.Birth = dto.Birth.ToUniversalTime();
            entity.Description = dto.Description;
            entity.Name = dto.Name;
            entity.IsGenderMale = dto.IsGenderMale;
            entity.DogBreedId = dto.Breed;
            entity.WeightClass = dto.WeightClass;

            _context.Dogs.Update(entity);
            await _context.SaveChangesAsync();
            return new DogUpdatedDTO()
            {
                Id = entity.Id,
                Response = RepositoryEnums.ResponseType.Updated
            };
        }

        private async Task<bool> BreedExists(int id)
{
            return await _context.DogBreeds.AnyAsync(db => db.Id == id);
        }
    }
}
