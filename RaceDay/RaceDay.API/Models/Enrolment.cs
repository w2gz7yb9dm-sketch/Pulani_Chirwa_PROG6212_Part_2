
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        public User? Participant { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [MaxLength(20)]
        public string? RaceNumber { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        [Required]
        [MaxLength(20)]
        public string EnrolmentStatus { get; set; } = "Active";

        public Result? Result { get; set; }
    }
}
