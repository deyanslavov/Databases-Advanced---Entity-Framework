namespace Stations.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Card")]
    public class CardDto
    {
        [Required]
        [MaxLength(128)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [Range(0, 120)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [XmlElement("CardType")]
        public string Type { get; set; }
    }
}
