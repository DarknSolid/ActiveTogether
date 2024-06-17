
namespace ModelLib.Utils
{
    public class DataAnnotationConstants
    {
        public const string RegexNoSpecialCharacters = @"^[a-zA-ZæøåÆØÅÄäÖöÜüẞß]+$";
        public const int PasswordMinLength = 6;
        public const string RegexPasswordUpperCase = @"[A-ZÆØÅÄÖÜß]+";
        public const string RegexPasswordLowerCase = @"[a-zæøåüäöẞ]+";
        public const string RegexContainDigit = @"[0-9]+";
        public const string RegexContainsSpecialCharacter = @"[#$^+=!*()@%&]";

        // https://stackoverflow.com/questions/48635152/regex-for-default-asp-net-core-identity-password
        public const string PasswordRegex = @"^(?=.*[a-zæøåüäöẞ])(?=.*[A-ZÆØÅÄÖÜß])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$";
    }
}
