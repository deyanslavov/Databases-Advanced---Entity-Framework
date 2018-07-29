namespace TeamBuilder.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserTeam
    {
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }
    }
}
