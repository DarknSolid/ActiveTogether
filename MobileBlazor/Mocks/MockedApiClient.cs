using MobileBlazor.Utils;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using ModelLib.Reviews;
using RazorLib.Interfaces;
using RazorLib.Pages;
using WebApp.DTOs.Authentication;
using static EntityLib.Entities.DogParkFacility;

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
            _dogParkListDTO = new ()
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
        public Task<MapSearchResultDTO> FetchMapMarkers(MapSearchDTO mapSearchDTO)
        {
            return Task.FromResult<MapSearchResultDTO>(new MapSearchResultDTO
            {
                DogParkListDTOs = new List<DogParkListDTO> { _dogParkListDTO }
            });
        }

        public Task<DogParkDetailedDTO> GetDogPark(int id)
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
                Facilities = new List<DogPackFacilityType> {  DogPackFacilityType.Fenced, DogPackFacilityType.Grassfield }
            });
        }

        public async Task<(PaginationResult, List<ReviewDetailedDTO>)> GetDogParkRatings(int id, int page, int pageCount)
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
                        AuthorName = "Author" + i,
                        Title = "my title here",
                        Comment = "A random comment here" + i,
                        Date = DateTime.Now,
                        Rating = Convert.ToInt32(_random.Next(0,6)),
                    }
                );
            }

            var paginationResult = new PaginationResult 
            { 
                CurrentPage = page, 
                HasNext = true 
            };

            return (paginationResult, result);
        }

        private async Task SimulateRequestDelay()
        {
            await Task.Delay(_random.Next(300, 700));
        }

        public async Task CreateReview(CreateReview.ReviewTypes reviewType, ReviewCreateDTO reviewCreateDTO)
        {
            await SimulateRequestDelay();
        }
        #endregion
    }
}
