namespace PhotoShare.Client.Core.Commands
{
    using Models;
    using Data;
    using System.Linq;
    using System;
    using Contracts;
    using PhotoShare.Client.Additional;

    public class AddTownCommand : IExecutable
    {
        // AddTown <townName> <countryName>
        public string Execute(string[] data)
        {
            string townName = data[0];
            string country = data[1];

            if (Session.User == null)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                bool townExists = context.Towns.Any(t => t.Name == townName);

                if (townExists)
                {
                    throw new ArgumentException($"Town {townName} was already added!");
                }

                Town town = new Town
                {
                    Name = townName,
                    Country = country
                };

                context.Towns.Add(town);
                context.SaveChanges();

                return $"Town {townName} was added successfully!";
            }
        }
    }
}
