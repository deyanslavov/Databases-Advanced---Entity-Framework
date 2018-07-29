namespace PetClinic.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.Models;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals
                                 .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                                 .Select(a => new
                                 {
                                     a.Passport.OwnerName,
                                     AnimalName = a.Name,
                                     a.Age,
                                     SerialNumber = a.PassportSerialNumber,
                                     RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy")
                                 })
                                 .OrderBy(a => a.Age)
                                 .ThenBy(a => a.SerialNumber)
                                 .ToArray();

            string json = JsonConvert.SerializeObject(animals, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        { 
            var procedures = context.Procedures
                .Select(p => new
                {
                    Passport = p.Animal.Passport.SerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime,
                    AnimalAids = p.ProcedureAnimalAids.Select(map => new
                    {
                        Name = map.AnimalAid.Name,
                        Price = map.AnimalAid.Price
                    }),
                    TotalPrice = p.ProcedureAnimalAids.Select(paa => paa.AnimalAid.Price).Sum(),
                })
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Passport)
                .ToArray();

            var xDoc = new XDocument(new XElement("Procedures"));

            foreach (var p in procedures)
            {
                xDoc.Root.Add
                (
                    new XElement("Procedure",
                        new XElement("Passport", p.Passport),
                        new XElement("OwnerNumber", p.OwnerNumber),
                        new XElement("DateTime", p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)),
                        new XElement("AnimalAids", p.AnimalAids.Select(aa =>
                            new XElement("AnimalAid",
                                new XElement("Name", aa.Name),
                                new XElement("Price", aa.Price)))),
                        new XElement("TotalPrice", p.TotalPrice))
                );
            }

            string result = xDoc.ToString();
            return result;
        }
    }
}
