namespace SoftJail.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    //    integer, Primary Key
    //•	Prisoner – the officer’s prisoner(required)
    //•	OfficerId – integer, Primary Key
    //•	Officer – the prisoner’s

    public class OfficerPrisoner
    {
        public int  PrisonerId { get; set; }

        [Required]
        public Prisoner Prisoner { get; set; }

        public int OfficerId { get; set; }

        [Required]
        public Officer Officer { get; set; }
    }
}
