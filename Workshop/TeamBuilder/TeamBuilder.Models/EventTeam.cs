namespace TeamBuilder.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class EventTeam
    {
        public int TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }
    }
}
