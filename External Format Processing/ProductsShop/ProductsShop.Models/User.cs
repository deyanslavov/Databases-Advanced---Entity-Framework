namespace ProductsShop.Models
{
    using Newtonsoft.Json;

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public int? Age { get; set; }

        public ICollection<Product> ProductsSold { get; set; }

        public ICollection<Product> ProductsBought { get; set; }
    }
}
