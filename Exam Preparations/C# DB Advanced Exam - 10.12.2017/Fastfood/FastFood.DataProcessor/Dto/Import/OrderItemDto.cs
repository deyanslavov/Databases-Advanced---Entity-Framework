using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Item")]
    public class OrderItemDto
    {
        [Required]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("Quantity")]
        public int Quantity { get; set; }
    }
}
