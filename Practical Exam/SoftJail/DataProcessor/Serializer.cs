namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using System.Xml.Serialization;
    using System.IO;
    using System.Xml;
    using System.Text;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                                   .Include(p => p.PrisonerOfficers)
                                   .ThenInclude(a => a.Officer)
                                   .Where(p => ids.Any(a => a == p.Id))
                                   .Select(p => new
                                   {
                                       Id = p.Id,
                                       Name = p.FullName,
                                       CellNumber = p.Cell.CellNumber,
                                       Officers = p.PrisonerOfficers.Select(x => new
                                       {
                                           OfficerName = x.Officer.FullName,
                                           Department = x.Officer.Department.Name,
                                       })
                                       .OrderBy(o => o.OfficerName)
                                       .ToArray(),
                                       TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Select(x => x.Officer.Salary).Sum(), 2)
                                   })
                                   .OrderBy(p => p.Name)
                                   .ThenBy(p => p.Id)
                                   .ToArray();


            var jsonString = JsonConvert.SerializeObject(prisoners, new JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            })
            .Trim();

            return jsonString;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',');

            var prisoners = context.Prisoners
                                   .Include(p => p.Mails)
                                   .Where(p => names.Any(n => n == p.FullName))
                                   .ToArray();

            var result = new List<PrisonerDto>();

            foreach (var p in prisoners)
            {
                PrisonerDto prisonerDto = new PrisonerDto
                {
                    Id = p.Id,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Name = p.FullName,
                };

                var messages = new List<MailDto>();

                foreach (var m in p.Mails)
                {
                    var charArray = m.Description.ToCharArray();
                    Array.Reverse(charArray);
                    var desc = new string(charArray);

                    MailDto mailDto = new MailDto { Description = desc };

                    messages.Add(mailDto);
                }

                prisonerDto.EncryptedMessages = messages.ToArray();

                result.Add(prisonerDto);
            }

            var export = result.OrderBy(a => a.Name).ThenBy(a => a.Id).ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(PrisonerDto[]), new XmlRootAttribute("Prisoners"));

            serializer.Serialize(new StringWriter(sb), export, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var xml = sb.ToString();
            return xml;

        }
    }
}