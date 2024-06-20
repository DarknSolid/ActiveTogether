
namespace EntityLib.Entities
{
    public class Enums
    {
        public enum Gender
        {
            Male = 0,
            Female = 1,
            NonBinary = 2,
            Other = 3
        }
        public enum PlaceType
        {
            DogPark = 0,
            PublicFacility = 1,
            DogInstructor = 2,
            Company = 3,
            None = 4,
            Meetup = 5,
            Event = 6
        }

        public enum Interests
        {
            Run = 0,
            BeachVolley = 1,
            Football = 2,
            Basket = 3,
            Paddle = 4,
            Tennis = 5,
            Badminton = 6,
            Mountainbike = 7,
            Yoga = 8,
            MartialArts = 9,
            CrossFit = 10,
            Calisthenics = 11,
            TeamTraining = 12,
            PersonalTraining = 13,
            Riding = 14,
        }

        public enum DogParkFacilityType
        {
            Lake = 0,
            Grassfield = 1,
            Agility = 2,
            Fenced = 3,
            Forest = 4,
            Leashed = 5,
            Unleashed = 6,
            Private = 7
        }

        public enum PostArea
        {
            Social = 3,
            DogParks = 2,
            Health = 4,
            DogTraining = 1,
            DogSitting = 5,
        }

        public enum PostCategory
        {
            Help = 0,
            Question = 1,
            TipsAndTricks = 2,
            OfferService = 3,
        }

        public enum CheckInMood
        {
            Social = 0,
            Learn = 1,
            Serious = 2
        }

        public enum InstructorCategory
        {
            Nosework = 1,
            Agility = 2,
            Puppy = 3,
            Adult = 4,
            Hunting = 5,
            Other = 6,
            Rally = 7,
            Beginner = 8,
            Parkour = 9,
            Dance = 10,
            Socializing = 11,
            Obedience = 12,
            Fitness = 13,
            Yoga = 14,
        }

        public enum InstructorFacility
        {
            Indoor = 1,
            Outdoor = 2,
            Agility = 3
        }
    }
}
