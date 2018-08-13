﻿namespace SoftJail.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    //    •	Id – integer, Primary Key
    //•	Description– text(required)
    //•	Sender – text(required)
    //•	Address – text, consisting only of letters, spaces and numbers, which ends with “ str.” (required) (Example: “62 Muir Hill str.“)
    //•	PrisonerId - integer, foreign key
    //•	Prisoner – the mail's Prisoner (required)

    public class Mail
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [RegularExpression(@"^[\w\d\s]+str.$")]
        public string Address { get; set; }

        public int PrisonerId { get; set; }

        [Required]
        [ForeignKey("PrisonerId")]
        public Prisoner Prisoner { get; set; }
    }
}
