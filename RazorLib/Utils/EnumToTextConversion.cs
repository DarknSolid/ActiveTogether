using EntityLib.Entities;
using MudBlazor;
using System.Text.RegularExpressions;
using static EntityLib.Entities.Enums;

namespace RazorLib.Utils
{
    public static class EnumToTextConversion
    {
        public static string MoodToString(CheckInMood mood)
        {
            switch (mood)
            {
                case CheckInMood.Playful: return "Wants to play";
                case CheckInMood.Social: return "Wants to socialize";
                case CheckInMood.None: return "No mood";
                default: return mood.ToString();    }
        }

        public static string PostAreaToString(PostArea area)
        {
            switch (area)
            {
                case PostArea.DogTraining: return "Hundetræning";
                case PostArea.DogSitting: return "Hundepasning";
                case PostArea.DogParks: return "Hundeparker";
                case PostArea.Social: return "Socialisering";
                case PostArea.Health: return "Helbred";
                default: return area.ToString();
            }
        }

        public static string PostCategoryToString(PostCategory category)
        {
            switch (category)
            {
                case PostCategory.Help: return "Hjælp";
                case PostCategory.OfferService: return "Jeg tilbyder en Service";
                case PostCategory.TipsAndTricks: return "Tips og Tricks";
                case PostCategory.Question: return "Spørgsmål";
                default: return category.ToString();
            }
        }

        public static Color PostCategoryToColor(PostCategory category)
        {
            switch (category)
            {
                case PostCategory.Help: return Color.Error;
                case PostCategory.Question: return Color.Warning;
                case PostCategory.OfferService: return Color.Success;
                case PostCategory.TipsAndTricks: return Color.Info;
                default: return Color.Inherit;
            }
        }

        public static string PostCategoryToIcon(PostCategory category)
        {
            switch (category)
            {
                case PostCategory.Question: return @Icons.Material.Filled.HelpCenter;
                case PostCategory.Help: return @Icons.Material.Filled.Error;
                case PostCategory.OfferService: return Icons.Material.Filled.LocalOffer;
                case PostCategory.TipsAndTricks: return Icons.Material.Filled.Lightbulb;
                default: return "";
            }
        }

        /// <summary>
        /// Taken from https://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
        /// </summary>
        /// <param name="e">The enum to convert to human readable text</param>
        /// <returns></returns>
        public static string ApplySpaceOnCapitals(Enum e)
        {
            return Regex.Replace(e.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
    }
}
