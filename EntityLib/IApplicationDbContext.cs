using EntityLib.Entities;
using EntityLib.Entities.Identity;
using EntityLib.Entities.PostsAndComments;
using Microsoft.EntityFrameworkCore;

namespace EntityLib
{
    public interface IApplicationDbContext
    {
        public DbSet<Place> Places { get; set; }

        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<PendingDogPark> PendingDogParks { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Dog> Dogs { get; set; }

        public DbSet<ApplicationUser> Users {get;set;}
        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<DogCheckIn> DogCheckIns { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<InstructorCompanyFacility> InstructorCompanyFacilities { get; set; }
        public DbSet<InstructorCompanyCategory> InstructorCompanyCategories { get; set; }
        public DbSet<DogTraining> DogTrainings { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }

        public Task SaveChangesAsync();
    }
}
