using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Instructors;

namespace ModelLib.ApiDTOs
{
    public class DogTrainingRequest : DistancePaginationRequest
    {
        public SearchFilterDogTraining? SearchFilter { get; set; }
    }
}
