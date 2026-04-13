using System;
using System.ComponentModel.DataAnnotations;

namespace FloorballCoach.Models
{
    /// <summary>
    /// Match statistics - for future expansion of the app
    /// </summary>
    public class MatchStatistics
    {
        [Key]
        public int Id { get; set; }

        public int MatchSetupId { get; set; }
        public MatchSetup? MatchSetup { get; set; }

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        // In-game statistics
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Shots { get; set; }
        public int ShotsOnGoal { get; set; }
        public int Penalties { get; set; }
        public TimeSpan TimeOnIce { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
