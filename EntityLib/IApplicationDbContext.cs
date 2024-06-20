using EntityLib.Entities;
using EntityLib.Entities.Chatting;
using EntityLib.Entities.EventsAndMeetups;
using EntityLib.Entities.Gatherings;
using EntityLib.Entities.Identity;
using EntityLib.Entities.Matching;
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

        public DbSet<ApplicationUser> Users {get;set;}
        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<CheckIn> CheckIns { get; set; }

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

        public Task SaveChangesAsync();
    }
}
