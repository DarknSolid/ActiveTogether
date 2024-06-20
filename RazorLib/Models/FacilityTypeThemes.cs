using MudBlazor;
using static EntityLib.Entities.Enums;

namespace RazorLib.Models
{
    public class FacilityTypeThemes
    {
        private Dictionary<PlaceType, MudTheme> _facilityToTheme;

        public FacilityTypeThemes()
        {

            var dogParkTheme = new BaseTheme();
            dogParkTheme.Palette.Primary = "#259B24";
            dogParkTheme.Palette.Secondary = "#5AF158";

            var instructorTheme = new BaseTheme();
            instructorTheme.Palette.Primary = "#E91E63";
            instructorTheme.Palette.Secondary = "#FF4081";

            _facilityToTheme = new()
            {
                { PlaceType.None, new BaseTheme() },
                {PlaceType.DogPark, dogParkTheme },
                {PlaceType.DogInstructor, instructorTheme }
            };
        }

        public MudTheme GetTheme(PlaceType facilityType)
        {
            return _facilityToTheme.GetValueOrDefault(facilityType, _facilityToTheme[PlaceType.None]);
        }

        private class BaseTheme : MudTheme
        {
            public BaseTheme()
            {
                Palette = Palette = new()
                {
                    Background = "#FCFAF6",
                    BackgroundGrey = "#f5f2ed",
                    Primary = "00CBA6",
                    AppbarBackground = "#FCFAF6"
                };
            }
        }
    }

}
