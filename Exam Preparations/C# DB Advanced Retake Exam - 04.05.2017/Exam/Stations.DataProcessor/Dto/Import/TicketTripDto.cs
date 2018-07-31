namespace Stations.DataProcessor.Dto.Import
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    [XmlType("Trip")]
    public class TicketTripDto
    {
        [Required]
        [XmlElement("OriginStation")]
        public string OriginStation { get; set; }

        [Required]
        [XmlElement("DestinationStation")]
        public string DestinationStation { get; set; }

        [Required]
        [XmlElement("DepartureTime")]
        public string DepartureTime { get; set; }
    }
}
