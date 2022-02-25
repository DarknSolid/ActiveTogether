namespace ModelLib.ApiDTOs
{
    public class MapSearchDTO
    {
        public BoundsDTO BoundsDTO { get; set; }

        public MapFilterDTO MapFilterDTO { get; set; }

        public MapSearchDTO()
        {
            MapFilterDTO = new MapFilterDTO();
        }
    }
}