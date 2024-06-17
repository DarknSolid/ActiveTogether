
namespace FisSst.BlazorMaps.Models.Tooltips
{
    public class TooltipOptions : InteractiveLayerOptions
    {
        public bool Permanent { get; set; }
        /// <summary>
        /// top, bottom, left, right, center, auto
        /// </summary>
        public string Direction { get; set; }
        public Point Offset { get; set; }
        public bool Sticky { get; set; }
        public float Opacity { get; set; }
        public string ClassName { get; set; }

        public TooltipOptions()
        {
            Pane = DefaultPane;
            Direction = "auto";
            BubblingMouseEvents = false;
            Interactive = true;
            Offset = new Point(0, 0);
            Sticky = false;
            Opacity = 0.9f;
            ClassName = "";
        }

        private const string DefaultPane = "tooltipPane";
    }
}
