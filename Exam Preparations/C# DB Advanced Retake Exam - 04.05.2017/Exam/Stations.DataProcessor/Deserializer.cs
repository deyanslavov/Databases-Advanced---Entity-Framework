namespace Stations.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    using Newtonsoft.Json;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Models;
    using Models.Enums;
    using Dto.Import;
    

    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportStations(StationsDbContext context, string jsonString)
        {
            var deserializedStations = JsonConvert.DeserializeObject<StationDto[]>(jsonString);

            var validStations = new List<Station>();

            var sb = new StringBuilder();

            foreach (var stationDto in deserializedStations)
            {
                if (!IsValid(stationDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (stationDto.Town == null)
                {
                    stationDto.Town = stationDto.Name;
                }

                bool stationExists = validStations.Any(s => s.Name == stationDto.Name);
                if (stationExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Station station = new Station
                {
                    Name = stationDto.Name,
                    Town = stationDto.Town,
                };

                validStations.Add(station);

                sb.AppendLine(string.Format(SuccessMessage, station.Name));
            }

            context.Stations.AddRange(validStations);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportClasses(StationsDbContext context, string jsonString)
        {
            var deserializedClasses = JsonConvert.DeserializeObject<SeatingClassDto[]>(jsonString);

            var validClasses = new List<SeatingClass>();

            var sb = new StringBuilder();

            foreach (var classDto in deserializedClasses)
            {
                if (!IsValid(classDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                bool seatingClassExists = validClasses.Any(a => a.Name == classDto.Name || a.Abbreviation == classDto.Abbreviation);
                if (seatingClassExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                SeatingClass seatingClass = new SeatingClass
                {
                    Name = classDto.Name,
                    Abbreviation = classDto.Abbreviation,
                };

                validClasses.Add(seatingClass);

                sb.AppendLine(string.Format(SuccessMessage, seatingClass.Name));
            }

            context.SeatingClasses.AddRange(validClasses);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportTrains(StationsDbContext context, string jsonString)
        {
            var deserializedTrains = JsonConvert.DeserializeObject<TrainDto[]>(jsonString);

            var validTrains = new List<Train>();

            var sb = new StringBuilder();

            foreach (var trainDto in deserializedTrains)
            {
                if (!IsValid(trainDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (trainDto.Type == null)
                {
                    trainDto.Type = "HighSpeed";
                }

                if (trainDto.Seats == null)
                {
                    trainDto.Seats = new SeatDto[0];
                }

                bool trainExists = validTrains.Any(a => a.TrainNumber == trainDto.TrainNumber);
                bool trainSeatsHaveNoNegativeValue = trainDto.Seats.All(a => a.Quantity >= 0);
                bool trainSeatsAreValid = trainDto.Seats.All(a => IsValid(a));

                if (!trainSeatsHaveNoNegativeValue || trainExists || !trainSeatsAreValid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var trainType = Enum.Parse<TrainType>(trainDto.Type);
                var seats = GetSeats(context, trainDto.Seats);

                Train train = new Train
                {
                    TrainNumber = trainDto.TrainNumber,
                    Type = trainType,
                    TrainSeats = seats,
                };

                validTrains.Add(train);

                sb.AppendLine(string.Format(SuccessMessage, train.TrainNumber));
            }

            context.Trains.AddRange(validTrains);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static List<TrainSeat> GetSeats(StationsDbContext context, SeatDto[] seats)
        {
            List<TrainSeat> result = new List<TrainSeat>();

            foreach (var seat in seats)
            {
                var seatingClassId = context.SeatingClasses.AsNoTracking().SingleOrDefault(a => a.Name == seat.Name && a.Abbreviation == seat.Abbreviation).Id;

                TrainSeat trainSeat = new TrainSeat
                {
                    Quantity = seat.Quantity.Value,
                    SeatingClassId = seatingClassId,
                };

                result.Add(trainSeat);
            }

            return result;
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            var deserializedTrips = JsonConvert.DeserializeObject<TripDto[]>(jsonString);

            var validTrips = new List<Trip>();

            var sb = new StringBuilder();

            foreach (var tripDto in deserializedTrips)
            {
                if (!IsValid(tripDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (tripDto.Status == null)
                {
                    tripDto.Status = "OnTime";
                }

                bool trainExists = context.Trains.Any(t => t.TrainNumber == tripDto.Train);
                bool originStationExists = context.Stations.Any(s => s.Name == tripDto.OriginStation);
                bool destinationStationExists = context.Stations.Any(s => s.Name == tripDto.DestinationStation);

                DateTime arrivalTime = DateTime.ParseExact(tripDto.ArrivalTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                DateTime departureTime = DateTime.ParseExact(tripDto.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                bool departureTimeIsBeforeArrivalTime = departureTime < arrivalTime;

                if (!trainExists || !originStationExists || !destinationStationExists || !departureTimeIsBeforeArrivalTime)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                TimeSpan? timeDifference = null;
                if (tripDto.TimeDifference != null)
                {
                    timeDifference = TimeSpan.ParseExact(tripDto.TimeDifference, @"hh\:mm", CultureInfo.InvariantCulture);
                }

                int trainId = context.Trains.AsNoTracking().SingleOrDefault(t => t.TrainNumber == tripDto.Train).Id;
                int originStationId = context.Stations.AsNoTracking().SingleOrDefault(s => s.Name == tripDto.OriginStation).Id;
                int destinationStationId = context.Stations.AsNoTracking().SingleOrDefault(s => s.Name == tripDto.DestinationStation).Id;
                var tripStatus = Enum.Parse<TripStatus>(tripDto.Status, true);

                Trip trip = new Trip
                {
                    Status = tripStatus,
                    TimeDifference = timeDifference,
                    TrainId = trainId,
                    OriginStationId = originStationId,
                    DestinationStationId = destinationStationId,
                    ArrivalTime = arrivalTime,
                    DepartureTime = departureTime,
                };

                validTrips.Add(trip);

                sb.AppendLine($"Trip from {tripDto.OriginStation} to {tripDto.DestinationStation} imported.");
            }

            context.Trips.AddRange(validTrips);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));
            var deserializedCards = (CardDto[])xmlSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var validCards = new List<CustomerCard>();

            var sb = new StringBuilder();

            foreach (var cardDto in deserializedCards)
            {
                if (!IsValid(cardDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (cardDto.Type == null)
                {
                    cardDto.Type = "Normal";
                }

                bool cardExists = validCards.Any(c => c.Name == cardDto.Name);
                if (cardExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var cardType = Enum.Parse<CardType>(cardDto.Type, true);

                CustomerCard customerCard = new CustomerCard
                {
                    Type = cardType,
                    Age = cardDto.Age,
                    Name = cardDto.Name,
                };

                validCards.Add(customerCard);

                sb.AppendLine(string.Format(SuccessMessage, customerCard.Name));
            }

            context.Cards.AddRange(validCards);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(TicketDto[]), new XmlRootAttribute("Tickets"));
            var deserializedTickets = (TicketDto[])xmlSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var validTickets = new List<Ticket>();

            var sb = new StringBuilder();

            foreach (var ticketDto in deserializedTickets)
            {
                if (!IsValid(ticketDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                DateTime departureTime = DateTime.ParseExact(ticketDto.Trip.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                int? tripId = context.Trips
                                     .SingleOrDefault(t => t.OriginStation.Name == ticketDto.Trip.OriginStation && 
                                        t.DestinationStation.Name == ticketDto.Trip.DestinationStation && 
                                            t.DepartureTime == departureTime)?.Id;

                if (tripId == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                
                Train train = context.Trips
                                     .Include(t => t.Train)
                                     .ThenInclude(t => t.TrainSeats)
                                     .ThenInclude(t => t.SeatingClass)
                                     .SingleOrDefault(t => t.OriginStation.Name == ticketDto.Trip.OriginStation && 
                                        t.DestinationStation.Name == ticketDto.Trip.DestinationStation && 
                                            t.DepartureTime == departureTime).Train;

                string abbreviation = ticketDto.Seat.Substring(0, 2);

                var seatingClass = train.TrainSeats.SingleOrDefault(t => t.SeatingClass.Abbreviation == abbreviation);

                if (seatingClass == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                bool seatsAvailable = train.TrainSeats.Any(t => t.Id == seatingClass.Id);

                if (!seatsAvailable)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                int quantity = int.Parse(ticketDto.Seat.Substring(2, ticketDto.Seat.Length - 2));

                bool seatExists = train.TrainSeats.FirstOrDefault(t => t.Id == seatingClass.Id).Quantity >= quantity;

                int? cardId = null;
                
                if (ticketDto.Card != null)
                {
                    bool cardExists = context.Cards.AsNoTracking().Any(c => c.Name == ticketDto.Card.Name);

                    if (!cardExists)
                    {
                        sb.AppendLine(FailureMessage);
                        continue;
                    }

                    cardId = context.Cards.AsNoTracking().SingleOrDefault(c => c.Name == ticketDto.Card.Name)?.Id;
                }

                if (!seatExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Ticket ticket = new Ticket
                {
                    TripId = tripId.Value,
                    CustomerCardId = cardId,
                    Price = ticketDto.Price,
                    SeatingPlace = ticketDto.Seat
                };

                validTickets.Add(ticket);

                sb.AppendLine($"Ticket from {ticketDto.Trip.OriginStation} to {ticketDto.Trip.DestinationStation} departing at {departureTime.ToString("dd/MM/yyyy HH:mm")} imported.");
            }

            context.Tickets.AddRange(validTickets);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}