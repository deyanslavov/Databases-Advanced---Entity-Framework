namespace Stations.DataProcessor.Dto.Import
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    [XmlType("Ticket")]
    public class TicketDto
    {
        [Required]
        [XmlAttribute("price")]
        [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [XmlAttribute("seat")]
        [Required]
        [MaxLength(8)]
        [RegularExpression(@"^[a-zA-Z]{2}[\d]{1,6}$")]
        public string Seat { get; set; }

        [Required]
        [XmlElement("Trip")]
        public TicketTripDto Trip { get; set; }

        [XmlElement("Card")]
        public TicketCardDto Card { get; set; }
    }
}
