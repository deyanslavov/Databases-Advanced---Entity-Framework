namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Additional;

    public class ModifyUserCommand : IExecutable
    {
        // ModifyUser <username> <property> <new value>
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // !!! Cannot change username
        public string Execute(string[] data)
        {
            string username = data[0];
            string property = data[1];
            string newValue = data[2];

            if (Session.User == null || Session.User.Username != username)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            using (var db = new PhotoShareContext())
            {
                User user = db.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    throw new Exception($"{username} does not exist in the database!");
                }

                switch (property.ToLower())
                {
                    case "password":
                        UpdateUserPassword(newValue, user);
                        break;
                    case "borntown":
                        UpdateUserBornTown(newValue, db, user);
                        break;
                    case "currenttown":
                        UpdateUserCurrentTown(newValue, db, user);
                        break;
                    default:
                        throw new ArgumentException($"Property {property} not supported!");
                }

                db.SaveChanges();
            }

            return $"User {username} {property} is {newValue}.";
        }

        private static void UpdateUserCurrentTown(string newValue, PhotoShareContext db, User user)
        {
            bool townExists = db.Towns.Any(t => t.Name == newValue);

            if (!townExists)
            {
                throw new ArgumentException($"Town {newValue} not found!");
            }

            Town town = db.Towns.FirstOrDefault(t => t.Name == newValue);

            user.CurrentTown = town;
        }

        private static void UpdateUserBornTown(string newValue, PhotoShareContext db, User user)
        {
            bool townExists = db.Towns.Any(t => t.Name == newValue);

            if (!townExists)
            {
                throw new ArgumentException($"Town {newValue} not found!");
            }

            Town town = db.Towns.FirstOrDefault(t => t.Name == newValue);

            user.BornTown = town;
        }

        private static void UpdateUserPassword(string newValue, User user)
        {
            bool newPasswordIsValid = newValue.Any(p => Char.IsLower(p) && char.IsDigit(p));

            if (!newPasswordIsValid)
            {
                throw new ArgumentException("Invalid password!");
            }

            user.Password = newValue;
        }
    }
}
