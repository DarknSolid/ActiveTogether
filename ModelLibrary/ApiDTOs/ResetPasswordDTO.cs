using ModelLib.Utils;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.ApiDTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Must be at least 6 characters")]
        [RegularExpression(DataAnnotationConstants.PasswordRegex, ErrorMessage = "Must contain a digit, special character, lower and upper case character")]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Password must match")]
        public string NewPasswordRepeated { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
