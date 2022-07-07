using MobileBlazor.Utils;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using RazorLib.Interfaces;
using RazorLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTOs.Authentication;
using static EntityLib.Entities.DogParkFacility;

namespace MobileBlazor.Mocks
{
    public class MockedApiClient : IMobileApiClient, IApiClient
    {

        private readonly UserInfoDTO _user;
        private readonly DogParkListDTO _dogParkListDTO;

        public MockedApiClient()
        {
            _user = new UserInfoDTO { Email = "mock@user.com", ProfilePictureUrl = "", UserName = "IAmAMockUser" };
            _dogParkListDTO = new DogParkListDTO
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
            return Task.FromResult(new DogParkDetailedDTO
            {
                Id = id,
                Description = "A mocked dog park right here! This park is so great and beautiful that you won't believe your own eyes ;D",
                Name = _dogParkListDTO.Name,
                Latitude = _dogParkListDTO.Latitude,
                Longitude = _dogParkListDTO.Longitude,
                Rating = 4,
                ImageUrls = new List<string> { "https://www.dogparkconsulting.com/wp-content/uploads/2017/01/DogParkF_sm-1024x756.jpg" },
                Facilities = new List<DogPackFacilityType> {  DogPackFacilityType.Fenced, DogPackFacilityType.Grassfield }
            });
        }

        public Task<(PaginationResult, List<RatingDTO>)> GetDogParkRatings(int id, int page, int pageCount)
        {
            var result = new List<RatingDTO>();
            var random = new Random();
            var start = page * pageCount;
            var stop = page * pageCount + pageCount;
            for (int i = start; i < stop; i++)
            {
                result.Add(
                    new RatingDTO
                    {
                        Author = "Author" + i,
                        Comment = "A random comment here" + i,
                        Date = DateTime.Now,
                        Rating = Convert.ToInt32(random.Next(0,6))
                    }
                );
            }

            var paginationResult = new PaginationResult 
            { 
                CurrentPage = page, 
                HasNext = true 
            };

            return Task.FromResult((paginationResult, result));
        }
        #endregion
    }
}
