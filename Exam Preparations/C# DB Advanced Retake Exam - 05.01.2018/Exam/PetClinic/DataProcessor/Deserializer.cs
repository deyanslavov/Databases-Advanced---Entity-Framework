namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Newtonsoft.Json;
    using Microsoft.EntityFrameworkCore;
    using PetClinic.Data;
    using PetClinic.DataProcessor.DTOs.Import;
    using PetClinic.Models;
    using System.Xml.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Error: Invalid data.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var deserializedAnimalAids = JsonConvert.DeserializeObject<AnimalAidDto[]>(jsonString);

            var validAnimalAids = new List<AnimalAid>();

            var sb = new StringBuilder();

            foreach (var animalAidDto in deserializedAnimalAids)
            {
                if (!IsValid(animalAidDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool animalAidExists = validAnimalAids.Any(a => a.Name == animalAidDto.Name);
                if (animalAidExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                AnimalAid animalAid = Mapper.Map<AnimalAid>(animalAidDto);

                validAnimalAids.Add(animalAid);

                sb.AppendLine($"Record {animalAid.Name} successfully imported.");
            }

            context.AnimalAids.AddRange(validAnimalAids);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var deserializedAnimals = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);

            var validAnimals = new List<Animal>();

            var sb = new StringBuilder();

            foreach (var animalDto in deserializedAnimals)
            {
                if (!IsValid(animalDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (!IsValid(animalDto.Passport))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool passportExists = validAnimals.Any(a => a.Passport.SerialNumber == animalDto.Passport.SerialNumber);
                if (passportExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Animal animal = Mapper.Map<Animal>(animalDto);

                DateTime dateTime = DateTime.ParseExact(animalDto.Passport.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                animal.Passport.RegistrationDate = dateTime;

                validAnimals.Add(animal);

                sb.AppendLine($"Record {animal.Name} Passport №: {animal.Passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(validAnimals);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));

            var deserializedVets = (VetDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var validVets = new List<Vet>();

            var sb = new StringBuilder();

            foreach (var vetDto in deserializedVets)
            {
                if (!IsValid(vetDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool vetExists = validVets.Any(v => v.PhoneNumber == vetDto.PhoneNumber);
                if (vetExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Vet vet = Mapper.Map<Vet>(vetDto);

                validVets.Add(vet);

                sb.AppendLine($"Record {vet.Name} successfully imported.");
            }

            context.Vets.AddRange(validVets);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);
            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();
            var validEntries = new List<Procedure>();

            foreach (var el in elements)
            {
                string vetName = el.Element("Vet")?.Value;
                string passportId = el.Element("Animal")?.Value;
                string dateTimeString = el.Element("DateTime")?.Value;

                int? vetId = context.Vets.SingleOrDefault(v => v.Name == vetName)?.Id;
                bool passportExists = context.Passports.Any(p => p.SerialNumber == passportId);

                bool dateIsValid = DateTime
                    .TryParseExact(dateTimeString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);

                var animalAidElements = el.Element("AnimalAids")?.Elements();

                if (vetId == null || !passportExists || animalAidElements == null || !dateIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var animalAidIds = new List<int>();
                bool allAidsExist = true;

                foreach (var aid in animalAidElements)
                {
                    string aidName = aid.Element("Name")?.Value;

                    int? aidId = context.AnimalAids.SingleOrDefault(a => a.Name == aidName)?.Id;

                    if (aidId == null || animalAidIds.Any(id => id == aidId))
                    {
                        allAidsExist = false;
                        break;
                    }

                    animalAidIds.Add(aidId.Value);
                }

                if (!allAidsExist)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var procedure = new Procedure()
                {
                    VetId = vetId.Value,
                    AnimalId = context.Animals.Single(a => a.PassportSerialNumber == passportId).Id,
                    DateTime = dateTime,
                };

                foreach (var id in animalAidIds)
                {
                    var mapping = new ProcedureAnimalAid()
                    {
                        Procedure = procedure,
                        AnimalAidId = id
                    };

                    procedure.ProcedureAnimalAids.Add(mapping);
                }

                bool isValid = IsValid(procedure);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validEntries.Add(procedure);
                sb.AppendLine("Record successfully imported.");
            }

            context.Procedures.AddRange(validEntries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object obj)
        {
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            List<ValidationResult> results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, results, true);

            return isValid;
        }
    }
}
