using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Reviews;
using RazorLib.Models;
using static EntityLib.Entities.Enums;

namespace RazorLib.Interfaces
{
    public interface IApiClient
    {
        public Task<MapSearchResultDTO> FetchMapMarkers(MapSearchDTO mapSearchDTO);
        public Task<DogParkDetailedDTO> GetDogPark(int id);
        public Task<ReviewListViewDTO> GetReviews(int id, ReviewType reviewType, int page, int pageCount);
        public Task<ReviewCreatedDTO> CreateReview(ReviewType reviewType, ReviewCreateDTO reviewCreateDTO);
    }
}
