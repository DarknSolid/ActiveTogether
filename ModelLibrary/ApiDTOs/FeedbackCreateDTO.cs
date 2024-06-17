using System.ComponentModel.DataAnnotations;

namespace ModelLib.ApiDTOs
{
    public class FeedbackCreateDTO
    {
        /// <summary>
        /// The Uri of the page that the user is writing the feedback from.
        /// </summary>
        public string Uri { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public FeedbackSeverity Severity { get; set; }
        public bool MayContact { get; set; }

    }

    public enum FeedbackSeverity
    {
        Suggestion = 1,
        Bug = 2,
    }
}
