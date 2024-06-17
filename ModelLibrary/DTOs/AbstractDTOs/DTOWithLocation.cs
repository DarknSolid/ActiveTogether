
using FisSst.BlazorMaps;
using NpgsqlTypes;

namespace ModelLib.DTOs.AbstractDTOs
{
    public abstract class DTOWithLocation
    {
        abstract public int Id { get; set; }
        abstract public NpgsqlPoint Location { get; set; }
    }
}
