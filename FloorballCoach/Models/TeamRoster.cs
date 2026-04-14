using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FloorballCoach.Models
{
    /// <summary>
    /// Junction table for Team-Player many-to-many relationship
    /// Represents which players are in which team's roster
    /// </summary>
    public class TeamRoster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }

        [Required]
        public int PlayerId { get; set; }

        /// <summary>
        /// When the player joined this team's roster
        /// </summary>
        public System.DateTime JoinedDate { get; set; } = System.DateTime.Now;

        /// <summary>
        /// Player is active in this team's roster
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation property to Team
        /// </summary>
        [ForeignKey(nameof(TeamId))]
        public Team? Team { get; set; }

        /// <summary>
        /// Navigation property to Player
        /// </summary>
        [ForeignKey(nameof(PlayerId))]
        public Player? Player { get; set; }
    }
}
