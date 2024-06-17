using EntityLib.Entities.AbstractClasses;

namespace ModelLib.DTOs.Authentication
{
    public class UserListDTO : IntegerId
    {
        override public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get => FirstName + " " + LastName; }
        public string? ProfilePictureUrl { get; set; }
        public string FullNameNormalized { get; set; }
    }
}
