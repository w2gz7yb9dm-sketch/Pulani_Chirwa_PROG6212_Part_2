
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        public Enrolment? Enrolment { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        [Required]
        [MaxLength(20)]
        public string ResultStatus { get; set; } = "Finished";

        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CapturedByUserId { get; set; }

        public User? CapturedByUser { get; set; }
    }
}
