
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public int OrganiserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? EventType { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? DistanceKm { get; set; }

        public DateTime StartDateTime { get; set; }

        public string? VenueName { get; set; }

        public string? City { get; set; }

        public string? Province { get; set; }

        public DateTime? RegistrationCloseDate { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? Organiser { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();

        public Route? Route { get; set; }
    }
}
