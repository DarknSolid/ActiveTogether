using EntityLib;
using EntityLib.Entities;
using EntityLib.Entities.Identity;
using EntityLib.Entities.PostsAndComments;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IApplicationDbContext
    {

        static ApplicationDbContext()
        {
        }
        public DbSet<Place> Places { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<DogPark> DogParks { get; set; }
        public DbSet<PendingDogPark> PendingDogParks { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<InstructorCompanyFacility> InstructorCompanyFacilities { get; set; }
        public DbSet<InstructorCompanyCategory> InstructorCompanyCategories { get; set; }
        public DbSet<DogTraining> DogTrainings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("postgis");

            #region Review

            builder.Entity<Review>()
                .HasOne(r => r.Place)
                .WithMany(u => u.Reviews);

            builder.Entity<Review>()
                .HasIndex(r => new { r.PlaceId, r.DateTime });
            builder.Entity<Review>()
    .HasIndex(r => new { r.PlaceId, r.UserId }); // used for lookup on a specific review
            #endregion

            #region DogPark
            builder.Entity<DogPark>()
                .Property(d => d.Facilities)
                .HasConversion<string[]>();
            #endregion

            #region PendingDogPark
            builder.Entity<PendingDogPark>()
                .Property(d => d.Facilities)
                .HasConversion<int[]>();
            #endregion

            #region CheckIn
            builder.Entity<CheckIn>()
                .HasOne(c => c.User)
                .WithMany(u => u.CheckIns);
            builder.Entity<CheckIn>()
                .Property(c => c.Mood)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<CheckIn>()
                .HasIndex(r => new { r.PlaceId, r.CheckInDate });
            #endregion

            #region Friends
            builder.Entity<Friendship>().HasOne(c => c.Requester).WithMany(u => u.Friends);
            builder.Entity<Friendship>().HasIndex(c => new { c.RequesteeId, c.RequesterId });
            builder.Entity<Friendship>().HasIndex(c => new { c.RequesterId, c.RequesteeId });
            #endregion

            #region Users
            builder.Entity<ApplicationUser>()
                .Property(u => u.FullNameNormalized)
                // combine first name and last name and upper case them
                .HasComputedColumnSql(@"UPPER(""" + nameof(ApplicationUser.FirstName) + @""") || ' ' || UPPER(""" + nameof(ApplicationUser.LastName) + @""")",
                                        stored: true);
            builder.Entity<ApplicationUser>().HasIndex(u => new { u.FullNameNormalized, u.Id });

            builder.Entity<ApplicationUser>().Property(p => p.CreatedAt).HasDefaultValueSql("now()");

            #endregion

            #region Companies
            builder.Entity<Company>()
                .HasOne(i => i.Place)
                .WithOne(i => i.Company);

            builder.Entity<Company>()
                .HasOne(i => i.User)
                .WithMany(i => i.Companies);

            builder.Entity<Company>()
                .HasMany(i => i.InstructorFacilities)
                .WithOne(i => i.InstructorCompany);

            builder.Entity<Company>()
                .HasMany(i => i.InstructorCategories)
                .WithOne(i => i.InstructorCompany);

            builder.Entity<Company>().Property(p => p.CreatedAt).HasDefaultValueSql("now()");

            #endregion

            #region InstructorCompanyFacility
            builder.Entity<InstructorCompanyFacility>()
                .HasKey(i => new { i.InstructorCompanyId, i.InstructorFacility });
            #endregion

            #region InstructorCompanyCategory
            builder.Entity<InstructorCompanyCategory>()
                .HasKey(i => new { i.InstructorCompanyId, i.InstructorCategory });
            #endregion

            #region Posts
            //keys
            builder.Entity<PostLike>().HasKey(l => new { l.TargetId, l.UserId });
            builder.Entity<CommentLike>().HasKey(l => new { l.TargetId, l.UserId });

            //default values
            builder.Entity<Post>().Property(p => p.DateTime).HasDefaultValueSql("now()");
            builder.Entity<PostComment>().Property(p => p.DateTime).HasDefaultValueSql("now()");
            builder.Entity<PostLike>().Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            builder.Entity<CommentLike>().Property(p => p.CreatedAt).HasDefaultValueSql("now()");

            //indexes
            builder.Entity<Post>().HasIndex(p => new { p.DateTime, p.Id }); //for pagination with all posts, newest posts first.
            builder.Entity<PostComment>().HasIndex(p => p.Id); //for single lookup cases, such as delete
            builder.Entity<PostComment>().HasIndex(p => new { p.PostId, p.DateTime });
            #endregion

            base.OnModelCreating(builder);
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync(default);
        }

    }
}