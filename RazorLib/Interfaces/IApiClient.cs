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
        public Task<ReviewListViewDTO> GetReviewsAsync(int id, FacilityType reviewType, int page, int pageCount);
        public Task<ReviewCreatedDTO> CreateReviewAsync(FacilityType reviewType, ReviewCreateDTO reviewCreateDTO);
        public Task<List<DogListDTO>> GetMyDogsAsync();
        public Task<List<DogListDTO>> GetDogsAsync(int userId);
        public Task<DogDetailedDTO> GetDogDetailedAsync(int id);
        public Task<int?> CreateDogAsync(DogCreateDTO dto);
        public Task DeleteDogAsync(int dogId);
        public Task UpdateDogAsync(DogUpdateDTO dto);
        public Task<IDictionary<int, string>> GetDogBreedsAsync();
    }
}
