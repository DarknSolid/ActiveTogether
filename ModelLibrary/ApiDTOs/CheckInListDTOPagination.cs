using ModelLib.DTOs.CheckIns;

namespace ModelLib.ApiDTOs
{
    public class CheckInListDTOPagination
    {
        public PaginationResult PaginationResult { get; set; }
        public List<CheckInListDTO> CheckIns { get; set; }
    }
}
