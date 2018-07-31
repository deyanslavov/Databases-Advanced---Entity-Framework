namespace Stations.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Stations.Data;
    using Stations.DataProcessor.Dto.Export;
    using Stations.Models.Enums;

    public class Serializer
    {
        public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
        {
            DateTime date = DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var allTrains = context.Trips
                                .Include(t => t.Train)
                                .Where(t => t.Status == Models.Enums.TripStatus.Delayed && t.DepartureTime <= date)
                                .Select(a => new TrainDto
                                {
                                    MaxDelayedTime = a.TimeDifference.Value,
                                    TrainNumber = a.Train.TrainNumber,
                                    DelayedTimes = context.Trips
                                                          .Include(t => t.Train)
                                                          .Where(t => t.Status == Models.Enums.TripStatus.Delayed && 
                                                            t.DepartureTime <= date && t.Train.TrainNumber == a.Train.TrainNumber).Count()
                                })
                                .OrderByDescending(t => t.DelayedTimes)
                                .ThenByDescending(t => t.MaxDelayedTime)
                                .ThenBy(t => t.TrainNumber)
                                .ToArray();

            var trainsToExport = new List<TrainDto>();

            foreach (var t in allTrains)
            {
                if (trainsToExport.Any(train => train.TrainNumber == t.TrainNumber))
                {
                    continue;
                }

                trainsToExport.Add(t);
            }

            string json = JsonConvert.SerializeObject(trainsToExport);
            return json;
        }

        public static string ExportCardsTicket(StationsDbContext context, string cardType)
        {
            CardType type = Enum.Parse<CardType>(cardType = "debilitated", true);

            var allCards = context.Tickets
                              .Include(t => t.Trip)
                              .Include(t => t.CustomerCard)
                              .ThenInclude(c => c.BoughtTickets)
                              .Where(t => t.CustomerCard.Type == type)
                              .Select(t => new CardDto
                               {
                                   Name = t.CustomerCard.Name,
                                   Type = t.CustomerCard.Type.ToString(),
                                   Tickets = t.CustomerCard.BoughtTickets.Select(b => new TicketDto
                                   {
                                       OriginStation = b.Trip.OriginStation.Name,
                                       DestinationStation = b.Trip.DestinationStation.Name,
                                       DepartureTime = b.Trip.DepartureTime.ToString(@"dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                                   })
                                  .ToArray()
                               })
                               .OrderBy(c => c.Name)
                              .ToArray();

            var cardsToExport = new List<CardDto>();

            foreach (var c in allCards)
            {
                if (cardsToExport.Any(a => a.Name == c.Name))
                {
                    continue;
                }

                cardsToExport.Add(c);
            }

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));

            serializer.Serialize(new StringWriter(sb), cardsToExport.ToArray(), new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var xml = sb.ToString();
            return xml;
        }
    }
}