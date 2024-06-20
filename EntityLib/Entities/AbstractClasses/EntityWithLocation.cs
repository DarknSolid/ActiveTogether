using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities.AbstractClasses
{
    public interface EntityWithLocation
    {
        [Key]
        abstract public int Id { get; set; }
        [Required]
        [Column(TypeName = "geometry (point)")]
        abstract public Point Location { get; set; }
    }
}
