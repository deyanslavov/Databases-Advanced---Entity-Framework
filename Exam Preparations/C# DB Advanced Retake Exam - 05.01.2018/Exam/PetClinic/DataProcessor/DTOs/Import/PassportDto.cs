namespace PetClinic.DataProcessor.DTOs.Import
{
    using PetClinic.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PassportDto
    {
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[a-zA-Z]{7}[\d]{3}$")]
        public string SerialNumber { get; set; }
        
        [Required]
        [CheckOwnerPhoneNumber]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        public string RegistrationDate { get; set; }
    }
}
