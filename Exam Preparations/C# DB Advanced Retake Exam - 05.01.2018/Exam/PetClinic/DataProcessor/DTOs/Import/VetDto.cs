﻿namespace PetClinic.DataProcessor.DTOs.Import
{
    using PetClinic.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Vet")]
    public class VetDto
    {
        [Required]
        [StringLength(40, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Profession { get; set; }

        [Required]
        [Range(22, 65)]
        public int Age { get; set; }

        [Required]
        [CheckOwnerPhoneNumber]
        public string PhoneNumber { get; set; }
    }
}
