using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using ModelLib.DTOs.Dogs;
using ModelLib.DTOs.Reviews;
using static EntityLib.Entities.Enums;

namespace RazorLib.Interfaces
{
    public interface IApiClient
    {
        public Task<MapSearchResultDTO> FetchMapMarkersAsync(MapSearchDTO mapSearchDTO);
        public Task<DogParkDetailedDTO> GetDogParkAsync(int id);
        public Task<ReviewListViewDTO> GetReviewsAsync(int id, ReviewType reviewType, int page, int pageCount);
        public Task<ReviewCreatedDTO> CreateReviewAsync(ReviewType reviewType, ReviewCreateDTO reviewCreateDTO);
        public Task<List<DogListDTO>> GetMyDogsAsync();
        public Task<DogDetailedDTO> GetDogDetailedAsync(int id);
        public Task<DogCreatedDTO> CreateDogAsync(DogCreateDTO dto);
        public Task<DogUpdatedDTO> UpdateDogAsync(DogUpdateDTO dto);
        public Task<IDictionary<int, string>> GetDogBreedsAsync();
    }
}
