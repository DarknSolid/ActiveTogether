
using ModelLib.ApiDTOs;
using ModelLib.Utils;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.DTOs.Users
{
    public class UserUpdateDTO
    {
        [RegularExpression(DataAnnotationConstants.RegexNoSpecialCharacters)]
        public string? FirstName { get; set; }
        [RegularExpression(DataAnnotationConstants.RegexNoSpecialCharacters)]
        public string? LastName { get; set; }
        public FileDetailedDTO? ProfilePicture { get; set; }
    }
}
