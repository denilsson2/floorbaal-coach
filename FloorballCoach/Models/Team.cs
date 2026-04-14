using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FloorballCoach.Models
{
    /// <summary>
    /// Represents a floorball team
    /// </summary>
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ShortName { get; set; }

        [MaxLength(50)]
        public string? TeamColor { get; set; }

        [MaxLength(500)]
        public string? LogoPath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Navigation property
        public ICollection<TeamRoster> TeamRosters { get; set; } = new List<TeamRoster>();
    }
}
