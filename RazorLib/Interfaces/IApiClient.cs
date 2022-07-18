using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.CheckIns;
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

        #region Reviews
        public Task<ReviewListViewDTO> GetReviewsAsync(int id, int page, int pageCount);
        public Task<ReviewCreatedDTO> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO);
        #endregion

        #region Dogs
        public Task<List<DogListDTO>> GetMyDogsAsync();
        public Task<List<DogListDTO>> GetDogsAsync(int userId);
        public Task<DogDetailedDTO> GetDogDetailedAsync(int id);
        public Task<int?> CreateDogAsync(DogCreateDTO dto);
        public Task DeleteDogAsync(int dogId);
        public Task UpdateDogAsync(DogUpdateDTO dto);
        public Task<IDictionary<int, string>> GetDogBreedsAsync();
        #endregion

        #region Checkins
        public Task<int> CheckIn(CheckInCreateDTO dto);
        public Task<int> CheckOut();
        public Task<CheckInListDTOPagination> GetCheckIns(GetCheckInListDTO dto);
        public Task<CurrentlyCheckedInDTO?> GetCurrentCheckIn();
        #endregion
    }
}
