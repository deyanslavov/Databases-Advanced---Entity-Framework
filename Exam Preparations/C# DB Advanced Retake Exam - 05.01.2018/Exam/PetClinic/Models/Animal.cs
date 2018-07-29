namespace PetClinic.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Animal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Type { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Age { get; set; }

        public string PassportSerialNumber { get; set; }

        [Required]
        [ForeignKey("PassportSerialNumber")]
        public Passport Passport { get; set; }

        public ICollection<Procedure> Procedures { get; set; }
    }
}
