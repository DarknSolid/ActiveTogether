namespace ModelLib.ApiDTOs
{
    public class MapSearchDTO
    {
        public float North { get; set; }
        public float South { get; set; }
        public float East { get; set; }
        public float West { get; set; }
        public MapFilterDTO MapFilterDTO { get; set; }

        public MapSearchDTO()
        {
            MapFilterDTO = new MapFilterDTO();
        }
    }
}