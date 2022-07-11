using MobileBlazor.Utils;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Dogs;
using ModelLib.DTOs.Reviews;
using ModelLib.Repositories;
using RazorLib.Interfaces;
using WebApp.DTOs.Authentication;
using static EntityLib.Entities.Enums;

namespace MobileBlazor.Mocks
{
    public class MockedApiClient : IMobileApiClient, IApiClient
    {

        private readonly UserInfoDTO _user;
        private readonly DogParkListDTO _dogParkListDTO;
        private readonly Random _random;

        public MockedApiClient()
        {
            _random = new();
            _user = new() { Email = "mock@user.com", ProfilePictureUrl = "", UserName = "IAmAMockUser" };
            _dogParkListDTO = new()
            {
                Id = 1,
                Name = "The Mocked Dog Park",
                Latitude = 55.676098f, // Copenhagen coords
                Longitude = 12.568337f,
            };


        }

        #region IMobileApiClient implementation:
        public Task<UserInfoDTO> FacebookLogin()
        {
            return Task.FromResult(_user);
        }

        public Task<UserInfoDTO> GetUserInfo()
        {
            return Task.FromResult(_user);
        }

        public Task<UserInfoDTO> Login(string email, string password)
        {
            return Task.FromResult(_user);
        }

        public Task Logout() { return Task.CompletedTask; }
        #endregion

        #region IApiClient implementation:
        public Task<MapSearchResultDTO> FetchMapMarkersAsync(MapSearchDTO mapSearchDTO)
        {
            return Task.FromResult<MapSearchResultDTO>(new MapSearchResultDTO
            {
                DogParkListDTOs = new List<DogParkListDTO> { _dogParkListDTO }
            });
        }

        public Task<DogParkDetailedDTO> GetDogParkAsync(int id)
        {
            // generate a random review status enum value:
            Array reviewValues = Enum.GetValues(typeof(DogParkDetailedDTO.ReviewStatus));
            var reviewStatus = (DogParkDetailedDTO.ReviewStatus)reviewValues.GetValue(_random.Next(reviewValues.Length));

            return Task.FromResult(new DogParkDetailedDTO
            {
                Id = id,
                TotalReviews = 97,
                CurrentReviewStatus = reviewStatus,
                Description = "A mocked dog park right here! This park is so great and beautiful that you won't believe your own eyes ;D",
                Name = _dogParkListDTO.Name,
                Latitude = _dogParkListDTO.Latitude,
                Longitude = _dogParkListDTO.Longitude,
                Rating = 4,
                ImageUrls = new List<string> { "https://www.dogparkconsulting.com/wp-content/uploads/2017/01/DogParkF_sm-1024x756.jpg" },
                Facilities = new List<DogPackFacilityType> { DogPackFacilityType.Fenced, DogPackFacilityType.Grassfield }
            });
        }

        public async Task<ReviewListViewDTO> GetReviewsAsync(int id, ReviewType reviewType, int page, int pageCount)
        {
            var result = new List<ReviewDetailedDTO>();
            var start = page * pageCount;
            var stop = page * pageCount + pageCount;

            // Simulate response time
            await SimulateRequestDelay();

            for (int i = start; i < stop; i++)
            {
                result.Add(
                    new ReviewDetailedDTO
                    {
                        ReviewerFirstName = "John",
                        ReviewerLastName = "Doe " + i,
                        Title = "my title here",
                        Description = "A random comment here" + i,
                        Date = DateTime.Now,
                        Rating = Convert.ToInt32(_random.Next(0, 6)),
                    }
                );
            }

            var paginationResult = new PaginationResult
            {
                CurrentPage = page,
                HasNext = true
            };

            return new ReviewListViewDTO()
            {
                PaginationResult = paginationResult,
                Reviews = result
            };
        }

        private async Task SimulateRequestDelay()
        {
            await Task.Delay(_random.Next(300, 700));
        }

        public async Task<ReviewCreatedDTO> CreateReviewAsync(ReviewType reviewType, ReviewCreateDTO reviewCreateDTO)
        {
            await SimulateRequestDelay();
            return new ReviewCreatedDTO()
            {
                ResponseType = RepositoryEnums.ResponseType.Created,
                ReviewType = reviewType,
                RevieweeId = reviewCreateDTO.RevieweeId,
                ReviewerId = 0
            };
        }

        public async Task<List<DogListDTO>> GetMyDogsAsync()
        {
            await SimulateRequestDelay();
            var result = new List<DogListDTO>();
            for (int i = 0; i < 3; i++)
            {
                result.Add(new DogListDTO
                {
                    Birth = DateTime.Now,
                    Id = i + 1,
                    IsGenderMale = i % 2 == 0,
                    Name = "Dog " + (i + 1),
                    Breed = (i + 1),
                    BreedName = "Breed " + (i + 1)
                });
            }
            return result;
        }

        public async Task<DogDetailedDTO> GetDogDetailedAsync(int id)
        {
            await SimulateRequestDelay();
            return new DogDetailedDTO
            {
                Id = 1,
                Birth = DateTime.Now,
                IsGenderMale = true,
                Name = "Dog " + 1,
                Breed = 1,
                BreedName = "Breed " + 1,
                Description = "My heckin' cute doggo",
                WeightClass = DogWeightClass.Medium,
            };
        }

        public Task<DogCreatedDTO> CreateDogAsync(DogCreateDTO dto)
        {
            return Task.FromResult(new DogCreatedDTO
            {
                Id = 5,
                Response = RepositoryEnums.ResponseType.Created
            });
        }

        public async Task<IDictionary<int, string>> GetDogBreedsAsync()
        {
            await SimulateRequestDelay();
            return new Dictionary<int, string>()
            {
                { 1, "Labrador"},
                { 2, "Rottweiler" },
                { 3, "Crusty" },
                { 4, "Chihuahua" }
            };
        }

        public async Task<DogUpdatedDTO> UpdateDogAsync(DogUpdateDTO dto)
        {
            await SimulateRequestDelay();
            return new DogUpdatedDTO
            {
                Id = dto.Id,
                Response = RepositoryEnums.ResponseType.Updated
            };
        }

        public async Task<List<DogListDTO>> GetDogsAsync(int userId)
        {
            return await GetMyDogsAsync();
        }
        #endregion
    }
}
