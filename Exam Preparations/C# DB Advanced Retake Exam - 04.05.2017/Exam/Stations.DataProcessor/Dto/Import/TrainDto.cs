namespace Stations.DataProcessor.Dto.Import
{
    using Stations.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class TrainDto
    {
        [Required]
        [MaxLength(10)]
        public string TrainNumber { get; set; }
        
        public string Type { get; set; }

        public SeatDto[] Seats { get; set; }
    }
}
