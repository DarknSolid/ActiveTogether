using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
    public class SearchFilterDogTraining
    {
        public InstructorCategory? Category { get; set; }
        public DayOfWeek? Day { get; set; }
        public TimeSpan? AfterTime { get; set; }
        public TimeSpan? BeforeTime { get; set; }
        public int? TrainerId { get; set; }
    }
}
