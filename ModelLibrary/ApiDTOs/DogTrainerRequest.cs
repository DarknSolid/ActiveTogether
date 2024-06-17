using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Instructors;

namespace ModelLib.ApiDTOs
{
    public class DogTrainerRequest : DistancePaginationRequest
    {
        public SearchFilterDogTrainer? SearchFilter { get; set; }
    }
}
