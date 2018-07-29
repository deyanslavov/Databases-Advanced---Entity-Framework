namespace TeamBuilder.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Invitation
    {
        [Key]
        public int Id { get; set; }

        public int InvitedUserId { get; set; }

        [ForeignKey("InvitedUserId")]
        public User InvitedUser { get; set; }

        public int TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
