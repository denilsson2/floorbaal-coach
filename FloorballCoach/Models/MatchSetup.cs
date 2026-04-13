using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FloorballCoach.Models
{
    /// <summary>
    /// Represents a match lineup
    /// </summary>
    public class MatchSetup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string MatchName { get; set; } = string.Empty;

        public DateTime MatchDate { get; set; } = DateTime.Now;

        // Goalkeeper
        public int? StartingGoalkeeperId { get; set; }
        public Player? StartingGoalkeeper { get; set; }

        public int? BackupGoalkeeperId { get; set; }
        public Player? BackupGoalkeeper { get; set; }

        // 3 lines
        public int? Line1Id { get; set; }
        public Line? Line1 { get; set; }

        public int? Line2Id { get; set; }
        public Line? Line2 { get; set; }

        public int? Line3Id { get; set; }
        public Line? Line3 { get; set; }

        // Bench players (max 3 field players)
        public string? BenchPlayerIds { get; set; } // Comma-separated list of player IDs

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
