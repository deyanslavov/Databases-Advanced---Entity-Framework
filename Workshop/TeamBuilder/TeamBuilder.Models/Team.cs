namespace TeamBuilder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(32)]
        public string Description { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Acronym { get; set; }

        public int CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public User Creator { get; set; }

        public ICollection<EventTeam> EventTeams { get; set; } = new HashSet<EventTeam>();

        public ICollection<Invitation> Invitations { get; set; } = new HashSet<Invitation>();

        public ICollection<UserTeam> UserTeams { get; set; } = new HashSet<UserTeam>();
    }
}
