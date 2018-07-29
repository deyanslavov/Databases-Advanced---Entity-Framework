namespace PetClinic.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Passport
    {
        [Key]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[a-zA-Z]{7}[\d]{3}$")]
        public string SerialNumber { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [Required]
        [CheckOwnerPhoneNumber]
        public string OwnerPhoneNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }
    }

    public class CheckOwnerPhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string phoneNumber = (string)value;

            if (phoneNumber.StartsWith('0') && phoneNumber.Length == 10)
            {
                return true;
            }

            if (phoneNumber.StartsWith("+359") && phoneNumber.Length == 13)
            {
                return true;
            }

            return false;
        }
    }
}
