

using EntityLib.Entities.AbstractClasses;

namespace ModelLib.DTOs.Reviews
{
    public class ReviewDetailedDTO : DateAndIntegerId
    {
        override public int Id { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string ReviewerFirstName { get; set; }
        public string ReviewerLastName { get; set; }
        public string GetFullName() { return ReviewerFirstName + " " + ReviewerLastName; }
        override public DateTime DateTime { get; set; }
        public int ReviewerId { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
