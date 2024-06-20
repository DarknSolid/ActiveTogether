using EntityLib;
using EntityLib.Entities;
using EntityLib.Entities.Chatting;
using EntityLib.Entities.EventsAndMeetups;
using EntityLib.Entities.Gatherings;
using EntityLib.Entities.Identity;
using EntityLib.Entities.Matching;
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
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Gathering> Meetups { get; set; }
        public DbSet<GatheringParticipant> MeetupParticipants { get; set; }
        public DbSet<Event> Events { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("postgis");

            #region Place
            builder.Entity<Place>().HasOne(p => p.CreatedBy).WithMany(u => u.CreatedPlaces);
            #endregion

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
            builder.Entity<Post>().HasOne(m => m.User).WithMany(u => u.Posts);

            //indexes
            builder.Entity<Post>().HasIndex(p => new { p.DateTime, p.Id }); //for pagination with all posts, newest posts first.
            builder.Entity<PostComment>().HasIndex(p => p.Id); //for single lookup cases, such as delete
            builder.Entity<PostComment>().HasIndex(p => new { p.PostId, p.DateTime });
            #endregion

            #region Comments
            builder.Entity<PostComment>().HasOne(m => m.User).WithMany(u => u.Comments);
            #endregion

            #region Messages
            builder.Entity<Message>().HasOne(m => m.User).WithMany(u => u.Messages);
            #endregion

            #region Reactions
            builder.Entity<Reaction>().HasKey(l => new { l.TargetType, l.TargetId, l.UserId });

            builder.Entity<Reaction>().HasOne(t => t.Post).WithMany(p => p.Reactions);
            builder.Entity<Reaction>().HasOne(t => t.Comment).WithMany(c => c.Reactions);
            builder.Entity<Reaction>().HasOne(t => t.Message).WithMany(p => p.Reactions);
            #endregion

            #region Chats
            builder.Entity<Chat>().HasOne(c => c.Place).WithMany(p => p.Chats);
            #endregion

            #region ChatMembers
            builder.Entity<ChatMember>().HasOne(cm => cm.User).WithMany(u => u.ChatMembers);
            builder.Entity<ChatMember>().HasOne(cm => cm.Chat).WithMany(c => c.ChatMembers);
            builder.Entity<ChatMember>().HasOne(cm => cm.LastRead).WithMany(m => m.LastReadMembers);
            #endregion

            #region Matches
            builder.Entity<Match>().HasOne(m => m.TargetOne).WithMany(u => u.MatchOne);
            builder.Entity<Match>().HasOne(m => m.TargetTwo).WithMany(u => u.MatchTwo);
            builder.Entity<Match>().HasOne(m => m.Chat).WithMany(u => u.Matches);
            #endregion

            #region Likes
            builder.Entity<Like>().HasOne(l => l.Liker).WithMany(u => u.LikeRequester);
            builder.Entity<Like>().HasOne(l => l.Likee).WithMany(u => u.LikeRequestee);
            #endregion

            #region Gatherings
            builder.Entity<Gathering>().HasOne(m => m.Place).WithMany(p => p.Meetups);
            builder.Entity<Gathering>().HasOne(m => m.HeldAtPlace).WithMany(p => p.MeetupReferences);
            #endregion

            #region GatheringParticipants
            builder.Entity<GatheringParticipant>().HasKey(gp => new { gp.MeetupId, gp.User });
            builder.Entity<GatheringParticipant>().HasOne(gp => gp.User).WithMany(u => u.Participations);
            builder.Entity<GatheringParticipant>().HasOne(gp => gp.Meetup).WithMany(m => m.Participants);
            #endregion

            #region Events
            builder.Entity<Event>().HasOne(e => e.Gathering).WithMany(g => g.Events);
            builder.Entity<Event>().HasOne(e => e.Company).WithMany(c => c.Events);
            #endregion

            base.OnModelCreating(builder);
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync(default);
        }

    }
}