using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Reviews;
using RazorLib.Models;
using static RazorLib.Pages.CreateReview;

namespace RazorLib.Interfaces
{
    public interface IApiClient
    {
        public Task<MapSearchResultDTO> FetchMapMarkers(MapSearchDTO mapSearchDTO);
        public Task<DogParkDetailedDTO> GetDogPark(int id);
        public Task<(PaginationResult, List<ReviewDetailedDTO>)> GetDogParkRatings(int id, int page, int pageCount);
        public Task CreateReview(ReviewTypes reviewType, ReviewCreateDTO reviewCreateDTO);
    }
}
