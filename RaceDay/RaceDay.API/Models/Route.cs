
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Route
    {
        public int RouteId { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        [Required]
        [MaxLength(150)]
        public string RouteName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(9,6)")]
        public decimal StartLatitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal StartLongitude { get; set; }

        [MaxLength(500)]
        public string? MapUrl { get; set; }

        public int? ElevationGainM { get; set; }

        [MaxLength(1000)]
        public string? RouteNotes { get; set; }
    }
}
