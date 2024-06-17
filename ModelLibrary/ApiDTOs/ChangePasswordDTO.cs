using ModelLib.Utils;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.ApiDTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        [RegularExpression(DataAnnotationConstants.PasswordRegex)]
        public string CurrentPassword { get; set; }

        [Required]
        [RegularExpression(DataAnnotationConstants.PasswordRegex)]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Must match new Password")]
        public string NewPasswordRepeated { get; set; }
    }
}
