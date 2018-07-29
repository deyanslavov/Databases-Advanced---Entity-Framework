    namespace PetClinic.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Procedure
    {
        public int Id { get; set; }

        public int AnimalId { get; set; }

        //[Required]
        [ForeignKey("AnimalId")]
        public Animal Animal { get; set; }

        public int VetId { get; set; }

        //[Required]
        [ForeignKey("VetId")]
        public Vet Vet { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [NotMapped]
        public decimal Cost => this.ProcedureAnimalAids.Sum(p => p.AnimalAid.Price);

        public ICollection<ProcedureAnimalAid> ProcedureAnimalAids { get; set; } = new HashSet<ProcedureAnimalAid>();
    }
}
