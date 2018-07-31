namespace Stations.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Enums;

    public class Trip
    {
        public int Id { get; set; }

        public int OriginStationId { get; set; }

        [Required]
        public Station OriginStation { get; set; }

        public int DestinationStationId { get; set; }

        [Required]
        public Station DestinationStation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public int TrainId { get; set; }

        [Required]
        public Train Train { get; set; }

        [Required]
        public TripStatus Status { get; set; } = TripStatus.OnTime;

        public TimeSpan? TimeDifference { get; set; }
    }
}
