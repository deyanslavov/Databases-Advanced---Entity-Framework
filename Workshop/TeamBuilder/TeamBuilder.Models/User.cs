namespace TeamBuilder.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(25)")]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }

        [Column(TypeName = "nvarchar(25)")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(25)")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,30}$")]
        public string Password { get; set; }

        public Gender Gender { get; set; }

        public int Age { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Invitation> ReceivedInvitations { get; set; } = new HashSet<Invitation>();

        public ICollection<UserTeam> UserTeams { get; set; } = new HashSet<UserTeam>();

        public ICollection<Team> CreatedTeams { get; set; } = new HashSet<Team>();

        public ICollection<Event> CreatedEvents { get; set; } = new HashSet<Event>();
    }
}
