namespace SoftJail.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using DataProcessor.ImportDto;


    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var deserializedDepartments = JsonConvert.DeserializeObject<Department[]>(jsonString);

            var validDepartments = new List<Department>();

            var sb = new StringBuilder();

            foreach (var dep in deserializedDepartments)
            {
                if (!IsValid(dep))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool allCellsValid = dep.Cells.All(a => IsValid(a));
                if (!allCellsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validDepartments.Add(dep);
                sb.AppendLine($"Imported {dep.Name} with {dep.Cells.Count} cells");
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var deserializedPrisoners = JsonConvert.DeserializeObject<PrisonerDto[]>(jsonString);

            var validPrisoners = new List<Prisoner>();

            var sb = new StringBuilder();

            foreach (var prisoner in deserializedPrisoners)
            {
                if (!IsValid(prisoner))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool allMailsValid = prisoner.Mails.All(a => IsValid(a));
                if (!allMailsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var mails = CreateMails(prisoner.Mails);

                DateTime? releaseDate = null;

                if (prisoner.ReleaseDate != null)
                {
                    releaseDate = DateTime.ParseExact(prisoner.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                var incarcerationDate = DateTime.ParseExact(prisoner.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var p = new Prisoner
                {
                    FullName = prisoner.FullName,
                    Nickname = prisoner.Nickname,
                    Age = prisoner.Age,
                    Bail = prisoner.Bail,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    CellId = prisoner.CellId,
                    Mails = mails,
                };

                validPrisoners.Add(p);
                sb.AppendLine($"Imported {p.FullName} {p.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(OfficerDto[]), new XmlRootAttribute("Officers"));

            var deserializedOfficers = (OfficerDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var sb = new StringBuilder();

            var validOfficers = new List<Officer>();

            foreach (var officerDto in deserializedOfficers)
            {
                if (!IsValid(officerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool allPrisonersValid = officerDto.Prisoners.All(a => IsValid(a));
                if (!allPrisonersValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var positionIsValid = Enum.TryParse<Position>(officerDto.Position, out Position position);
                var weaponIsValid = Enum.TryParse<Weapon>(officerDto.Weapon, out Weapon weapon);

                if (!positionIsValid || !weaponIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var officerPrisoners = CreateOfficerPrisoners(context, officerDto.Prisoners);

                Officer officer = new Officer
                {
                    FullName = officerDto.Name,
                    Salary = officerDto.Money,
                    DepartmentId = officerDto.DepartmentId,
                    Position = position,
                    Weapon = weapon,
                    OfficerPrisoners = officerPrisoners,
                };

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static List<Mail> CreateMails(ICollection<MailDto> mails)
        {
            var result = new List<Mail>();

            foreach (var m in mails)
            {
                Mail mail = new Mail
                {
                    Address = m.Address,
                    Description = m.Description,
                    Sender = m.Sender,
                };

                result.Add(mail);
            }

            return result;
        }

        private static List<OfficerPrisoner> CreateOfficerPrisoners(SoftJailDbContext context, PrisonerIdDto[] prisonersDtos)
        {
            var result = new List<OfficerPrisoner>();

            foreach (var p in prisonersDtos)
            {
                OfficerPrisoner officerPrisoner = new OfficerPrisoner
                {
                    Prisoner = GetPrisoner(context, p.Id),
                };

                result.Add(officerPrisoner);
            }

            return result;
        }

        private static Prisoner GetPrisoner(SoftJailDbContext context, int id)
        {
            return context.Prisoners.Find(id);
        }

        private static bool IsValid(object obj)
        {
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, context, results, true);
        }
    }
}