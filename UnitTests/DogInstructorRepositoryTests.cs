using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.DTOs.Instructors;
using ModelLib.Repositories;
using Moq;
using static ModelLib.Repositories.RepositoryEnums;

namespace UnitTests
{
    public class DogInstructorRepositoryTests : TestBase
    {
        private readonly IDogInstructorRepository _dogInstructorRepository;
        private readonly Mock<IPlacesRepository> _placesRepository;
        private readonly Mock<IBlobStorageRepository> _blobStorageRepository;

        public DogInstructorRepositoryTests()
        {
            _placesRepository = new Mock<IPlacesRepository>();
            _blobStorageRepository = new Mock<IBlobStorageRepository>();
            _dogInstructorRepository = new DogInstructorRepository(_context, _placesRepository.Object, _blobStorageRepository.Object);
        }

        [Fact]
        public async Task CreateInstructor_Existing_Name_Returns_Conflict()
        {
            var instructorCountBefore = _context.Places.Count();
            var dto = new InstructorCreateDTO
            {
                Categories = new List<Enums.InstructorCategory>() { Enums.InstructorCategory.Adult, Enums.InstructorCategory.Nosework },
                InstructorFacilities = new List<Enums.InstructorFacility>() { Enums.InstructorFacility.Indoor },
                Description = "test",
                Email = "adf@df.dk",
                Name = "",
                PhoneNumber = "11111111",
                Point = new(1, 1)
            };

            var (response, id) = await _dogInstructorRepository.CreateInstructorAsync(1, dto);

            Assert.Equal(ResponseType.Conflict, response);
            Assert.Equal(-1, id);
            Assert.Equal(instructorCountBefore, _context.Places.Count());
        }

        [Fact]
        public async Task CreateInstructor_Happy_Path_Creates_Place_Company_Categories_Facilities()
        {
            var expectedId = _context.Places.Count() + 1;
            var userId = 2;
            var dto = new InstructorCreateDTO
            {
                Categories = new List<Enums.InstructorCategory>() { Enums.InstructorCategory.Adult, Enums.InstructorCategory.Nosework },
                InstructorFacilities = new List<Enums.InstructorFacility>() { Enums.InstructorFacility.Indoor },
                Description = "test",
                Email = "adf@df.dk",
                Name = "hey",
                PhoneNumber = "11111111",
                Point = new(1, 1)
            };

            var (response, id) = await _dogInstructorRepository.CreateInstructorAsync(userId, dto);
            var actualEntity = await _context.Companies
                .Include(c => c.Place)
                .Include(c => c.InstructorCategories)
                .Include(c => c.InstructorFacilities)
                .FirstOrDefaultAsync(c => c.PlaceId == id);

            Assert.Equal(ResponseType.Created, response);
            Assert.Equal(expectedId, id);
            Assert.NotNull(actualEntity);

            Assert.Equal(dto.Name, actualEntity.Place.Name);
            Assert.Equal(dto.Description, actualEntity.Place.Description);
            Assert.Equal(dto.Email, actualEntity.Email);
            Assert.Equal(dto.PhoneNumber, actualEntity.Phone);
            Assert.Equal(userId, actualEntity.ApplicationUserId);

            Assert.True(actualEntity.InstructorCategories.All(c => dto.Categories.Contains(c.InstructorCategory)));
            Assert.Equal(dto.Categories.Count(), actualEntity.InstructorCategories.Count());

            Assert.True(actualEntity.InstructorFacilities.All(f => dto.InstructorFacilities.Contains(f.InstructorFacility)));
            Assert.Equal(dto.InstructorFacilities.Count(), actualEntity.InstructorFacilities.Count());

            Assert.True(dto.Point.X == actualEntity.Place.Location.X && dto.Point.Y == actualEntity.Place.Location.Y);
        }

        [Fact]
        public async Task UpdateInstructor_Happy_Path_Updates_Company_Place_Categories_Facilities()
        {
            var userId = 1;
            var companyId = 4;
            var dto = new InstructorUpdateDTO
            {
                Categories = new List<Enums.InstructorCategory>() { Enums.InstructorCategory.Puppy },
                InstructorFacilities = new List<Enums.InstructorFacility>() { Enums.InstructorFacility.Outdoor },
                Description = "new descirption",
                Name = "new name",
                Point = new NpgsqlTypes.NpgsqlPoint(2, 2),
                InstructorCompanyId = companyId
            };

            var response = await _dogInstructorRepository.UpdateInstructorAsync(userId: userId, updateDto: dto);
            var actualEntity = await _context.Companies
                .Include(c => c.Place)
                .Include(c => c.InstructorFacilities)
                .Include(c => c.InstructorCategories)
                .FirstOrDefaultAsync(c => c.PlaceId == companyId);


            Assert.Equal(ResponseType.Updated, response);
            Assert.NotNull(actualEntity);

            Assert.Equal(dto.Name, actualEntity.Place.Name);
            Assert.Equal(dto.Description, actualEntity.Place.Description);
            Assert.Equal(userId, actualEntity.ApplicationUserId);

            Assert.True(actualEntity.InstructorCategories.All(c => dto.Categories.Contains(c.InstructorCategory)));
            Assert.Equal(dto.Categories.Count(), actualEntity.InstructorCategories.Count());

            Assert.True(actualEntity.InstructorFacilities.All(f => dto.InstructorFacilities.Contains(f.InstructorFacility)));
            Assert.Equal(dto.InstructorFacilities.Count(), actualEntity.InstructorFacilities.Count());

            Assert.True(dto.Point.X == actualEntity.Place.Location.X && dto.Point.Y == actualEntity.Place.Location.Y);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 4)]
        public async Task UpdateInstructor_Invalid_User_Company_Combinations(int userId, int companyId)
        {
            var dto = new InstructorUpdateDTO
            {
                Categories = new List<Enums.InstructorCategory>() { Enums.InstructorCategory.Puppy },
                InstructorFacilities = new List<Enums.InstructorFacility>() { Enums.InstructorFacility.Outdoor },
                Description = "new descirption",
                Name = "new name",
                Point = new NpgsqlTypes.NpgsqlPoint(2, 2),
                InstructorCompanyId = companyId
            };

            var response = await _dogInstructorRepository.UpdateInstructorAsync(userId: userId, updateDto: dto);
            Assert.Equal(ResponseType.NotFound, response);
        }

        [Fact]
        public async Task UpdateInstructor_One_Field_Only_Updates_One_Field()
        {
            var userId = 1;
            var companyId = 4;

            var dto = new InstructorUpdateDTO
            {
                Description = "new description",
                InstructorCompanyId = companyId
            };

            // Clone company state before update:
            var actualEntityBefore = await _context.Companies
                .Include(c => c.Place)
                .Include(c => c.InstructorFacilities)
                .Include(c => c.InstructorCategories)
                .Select(c => new Company
                {
                    Place = new Place
                    {
                        Description = c.Place.Description,
                        Name = c.Place.Name,
                        Location = c.Place.Location
                    },
                    ApplicationUserId = userId,
                    Email = c.Email,
                    InstructorCategories = c.InstructorCategories.ToList(),
                    InstructorFacilities = c.InstructorFacilities.ToList(),
                    Phone = c.Phone,
                    PlaceId = c.PlaceId,
                    User = c.User
                })
                .FirstOrDefaultAsync(c => c.PlaceId == companyId);

            var response = await _dogInstructorRepository.UpdateInstructorAsync(userId: userId, updateDto: dto);

            var actualEntity = await _context.Companies
                .Include(c => c.Place)
                .Include(c => c.InstructorFacilities)
                .Include(c => c.InstructorCategories)
                .FirstOrDefaultAsync(c => c.PlaceId == companyId);


            Assert.Equal(ResponseType.Updated, response);
            Assert.NotEqual(dto.Description, actualEntityBefore.Place.Description);
            Assert.Equal(actualEntityBefore.Place.Name, actualEntity.Place.Name);
            Assert.Equal(dto.Description, actualEntity.Place.Description);
            Assert.Equal(userId, actualEntity.ApplicationUserId);

            Assert.True(actualEntity.InstructorCategories.All(c => actualEntityBefore.InstructorCategories.Any(ca => c.InstructorCategory == ca.InstructorCategory)));
            Assert.Equal(actualEntityBefore.InstructorCategories.Count(), actualEntity.InstructorCategories.Count());


            Assert.True(actualEntity.InstructorFacilities.All(c => actualEntityBefore.InstructorFacilities.Any(ca => c.InstructorFacility == ca.InstructorFacility)));
            Assert.Equal(actualEntityBefore.InstructorFacilities.Count(), actualEntity.InstructorFacilities.Count());

            Assert.True(actualEntityBefore.Place.Location.X == actualEntity.Place.Location.X && actualEntityBefore.Place.Location.Y == actualEntity.Place.Location.Y);
        }

        [Fact]
        public async Task GetInstructor_Returns_Expected()
        {
            var (response, instructor) = await _dogInstructorRepository.GetInstructorAsync(null, 4);

            Assert.Equal(ResponseType.Ok, response);
            Assert.NotNull(instructor);
            Assert.Equal("", instructor.Name);
            Assert.Single(instructor.Categories);
            Assert.Single(instructor.Facilities);
        }

        [Fact]
        public async Task GetInstructor_Invalid_Returns_Not_Found()
        {
            var (response, instructor) = await _dogInstructorRepository.GetInstructorAsync(null, 3);

            Assert.Equal(ResponseType.NotFound, response);
            Assert.Null(instructor);
        }
    }
}
