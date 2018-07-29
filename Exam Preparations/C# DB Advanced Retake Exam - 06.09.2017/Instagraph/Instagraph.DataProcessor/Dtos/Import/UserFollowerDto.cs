namespace Instagraph.DataProcessor.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;

    public class UserFollowerDto
    {
        [Required]
        public string User { get; set; }
        
        [Required]
        public string Follower { get; set; }
    }
}
