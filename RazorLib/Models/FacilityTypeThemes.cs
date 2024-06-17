using MudBlazor;
using static EntityLib.Entities.Enums;

namespace RazorLib.Models
{
    public class FacilityTypeThemes
    {
        private Dictionary<FacilityType, MudTheme> _facilityToTheme;

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
                { FacilityType.None, new BaseTheme() },
                {FacilityType.DogPark, dogParkTheme },
                {FacilityType.DogInstructor, instructorTheme }
            };
        }

        public MudTheme GetTheme(FacilityType facilityType)
        {
            return _facilityToTheme.GetValueOrDefault(facilityType, _facilityToTheme[FacilityType.None]);
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
