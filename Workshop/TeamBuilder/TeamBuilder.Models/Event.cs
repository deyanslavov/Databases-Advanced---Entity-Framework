namespace TeamBuilder.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public int CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public User Creator { get; set; }

        public ICollection<EventTeam> ParticipatingEventTeams { get; set; } = new List<EventTeam>();
    }
}
