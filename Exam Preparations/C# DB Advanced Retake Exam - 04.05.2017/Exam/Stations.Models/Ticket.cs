namespace Stations.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Ticket
    {
        public int Id { get; set; }

        [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(8)]
        [RegularExpression(@"^[a-zA-Z]{2}[\d]{1,6}$")]
        public string SeatingPlace { get; set; }

        public int TripId { get; set; }

        [Required]
        public Trip Trip { get; set; }

        public int? CustomerCardId { get; set; }

        public CustomerCard CustomerCard { get; set; }
    }
}
