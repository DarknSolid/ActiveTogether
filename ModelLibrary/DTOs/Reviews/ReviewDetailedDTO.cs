

namespace ModelLib.DTOs.Reviews
{
    public class ReviewDetailedDTO : ReviewCreateDTO
    {
        public string ReviewerFirstName { get; set; }
        public string ReviewerLastName { get; set; }
        public string GetFullName() { return ReviewerFirstName + " " + ReviewerLastName; }
        public DateTime Date { get; set; }

        public int ReviewerId { get; set; }
    }
}
