using Microsoft.AspNetCore.Identity;

namespace EntityLib.Entities.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameNormalized { get; set; }
        // is either a blob URL or third party url, from e.g. Facebook or Google
        public string? ProfileImageUrl { get; set; }

        public bool AtLeastThirteenYearsOld { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<CheckIn> CheckIns { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Friendship> Friends { get; set; }
        public ICollection<Company> Companies { get; set; }
        public ICollection<Dog> Dogs { get; set; }
    }
}
