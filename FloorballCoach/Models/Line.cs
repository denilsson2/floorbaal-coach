using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FloorballCoach.Models
{
    /// <summary>
    /// Represents a line (5 players: 1 center, 2 forwards, 2 backs)
    /// </summary>
    public class Line
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int? CenterId { get; set; }
        public Player? Center { get; set; }

        public int? LeftForwardId { get; set; }
        public Player? LeftForward { get; set; }

        public int? RightForwardId { get; set; }
        public Player? RightForward { get; set; }

        public int? LeftBackId { get; set; }
        public Player? LeftBack { get; set; }

        public int? RightBackId { get; set; }
        public Player? RightBack { get; set; }

        public bool IsComplete => 
            CenterId.HasValue && 
            LeftForwardId.HasValue && 
            RightForwardId.HasValue && 
            LeftBackId.HasValue && 
            RightBackId.HasValue;
    }
}
