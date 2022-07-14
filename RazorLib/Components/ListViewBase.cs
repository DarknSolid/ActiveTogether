using Microsoft.AspNetCore.Components;

namespace RazorLib.Components
{
    public partial class ListViewBase : ComponentBase
    {
        [Parameter]
        public int ItemGap { get; set; }

        [Parameter]
        public ListViewOrientation Orientation { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string _class;
        protected string _style;

        protected override void OnParametersSet()
        {
            _style = $"gap:{ItemGap}px; height: 100%; width: 100%;";
            _class = "d-flex ";
            if (Orientation == ListViewOrientation.Vertical)
            {
                _class += "flex-column ";
            }
            else
            {
                _class += "flex-row ";
            }
            base.OnParametersSet();

        }

        public enum ListViewOrientation
        {
            Horizontal,
            Vertical
        }
    }
}
