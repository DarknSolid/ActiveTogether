using ModelLib.Utils;
using NetTopologySuite.Algorithm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.ApiDTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(DataAnnotationConstants.RegexNoSpecialCharacters, ErrorMessage = "Special characters are not allowed")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Required")]
        [RegularExpression(DataAnnotationConstants.RegexNoSpecialCharacters, ErrorMessage = "Special characters are not allowed")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(6, ErrorMessage = "Must be at least 6 characters")]
        [RegularExpression(DataAnnotationConstants.PasswordRegex, ErrorMessage = "Must contain a digit, special character, lower and upper case character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [PasswordPropertyText(password:true)]
        [Compare(nameof(Password), ErrorMessage = "Must match Password")]
        public string RepeatedPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        [Compare(nameof(Email), ErrorMessage = "Must match Email")]
        public string RepeatedEmail { get; set; }

        public FileDetailedDTO? ProfilePicture { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm that you are at least 13 years old")]
        public bool ConfirmAtLeastThirteen { get; set; }
    }
}
