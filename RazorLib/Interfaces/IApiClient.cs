using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using RazorLib.Models;

namespace RazorLib.Interfaces
{
    public interface IApiClient
    {
        public Task<MapSearchResultDTO> FetchMapMarkers(MapSearchDTO mapSearchDTO);
        public Task<DogParkDetailedDTO> GetDogPark(int id);
        public Task<(PaginationResult, List<RatingDTO>)> GetDogParkRatings(int id, int page, int pageCount); 
    }
}
