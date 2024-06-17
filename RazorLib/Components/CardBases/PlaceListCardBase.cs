using Microsoft.AspNetCore.Components;
using ModelLib.DTOs.Places;
using RazorLib.Utils;

namespace RazorLib.Components.CardBases
{
    public class PlaceListCardBase<T> : ComponentBase 
        where T : PlaceListDTO
    {

        [Parameter]
        public Action OnClick { get; set; }

        [Parameter, EditorRequired]
        public T PlaceListDTO { get; set; }

        protected Action _onClick;

        protected string _distanceKilometers;

        protected string _description = "";
        protected int _maxStringLength = 100;

        public PlaceListCardBase()
        {
            _onClick = () => { };
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _distanceKilometers = FormattingUtils.FormatDistance(PlaceListDTO.DistanceMeters);

            if (OnClick is not null)
            {
                _onClick = OnClick;
            }
        }

        protected string FormatRating(double rating)
        {
            if (rating % 1 == 0)
            {
                var ratingInt = (int)rating;
                return ratingInt.ToString();
            }
            return rating.ToString("n1");
        }
    }
}
