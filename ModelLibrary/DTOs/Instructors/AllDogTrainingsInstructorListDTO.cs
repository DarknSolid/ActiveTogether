
namespace ModelLib.DTOs.Instructors
{
    public class AllDogTrainingsInstructorListDTO
    {
        public IEnumerable<DogTrainingListDTO> ActiveTrainings { get; set; }
        public IEnumerable<DogTrainingListDTO> UpcommingTrainings { get; set; }
        public IEnumerable<DogTrainingListDTO> FinishedTrainings { get; set; }

        public AllDogTrainingsInstructorListDTO()
        {
            ActiveTrainings = new List<DogTrainingListDTO>();
            UpcommingTrainings = new List<DogTrainingListDTO>();
            FinishedTrainings = new List<DogTrainingListDTO>();
        }
    }
}
