using System;
using System.ComponentModel.DataAnnotations;

namespace FloorballCoach.Models
{
    /// <summary>
    /// Represents a floorball player
    /// </summary>
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        public Position Position { get; set; }

        public int JerseyNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Age => DateTime.Now.Year - DateOfBirth.Year - 
            (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        // Statistics
        public int GamesPlayed { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Points => Goals + Assists;
        public int Penalties { get; set; }

        // Player image path
        public string? ImagePath { get; set; }

        // Additional attributes
        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
        
        public bool IsInRoster { get; set; } = false;
        
        public int RosterOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
