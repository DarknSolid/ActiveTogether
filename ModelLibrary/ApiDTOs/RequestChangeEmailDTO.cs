using System.ComponentModel.DataAnnotations;

namespace ModelLib.ApiDTOs
{
    public class RequestChangeEmailDTO
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        [EmailAddress]
        [Compare(nameof(NewEmail), ErrorMessage = "Must match new Email")]
        public string NewEmailRepeated { get; set; }
    }
}
