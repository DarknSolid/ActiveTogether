
namespace EntityLib.Entities
{
    public class Enums
    {
        public enum FacilityType
        {
            DogPark=1,
            DogInstructor=2,
            None=3
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
            Help = 1,
            Question = 4,
            TipsAndTricks = 2,
            OfferService = 3,
        }

        public enum DogWeightClass
        {
            Small=1,
            Medium=2,
            Large=3
        }

        public enum CheckInMood
        {
            Playful=1,
            Social=2,
            None=3
        }

        public enum InstructorCategory
        {
            Nosework=1,
            Agility=2,
            Puppy=3,
            Adult=4,
            Hunting=5,
            Other=6,
            Rally=7,
            Beginner=8,
            Parkour=9,
            Dance=10,
            Socializing=11,
            Obedience=12,
            Fitness=13,
            Yoga=14,
        }

        public enum InstructorFacility
        {
            Indoor=1,
            Outdoor=2,
            Agility=3
        }

        public enum DogRace
        {
            Labrador = 0,
            Rottweiler = 1,
            SpringerSpaniel = 2,
            CockerSpaniel = 3,
        }
    }
}
