namespace PhotoShare.Client.Core.Commands
{
    using System;

    using Models;
    using Data;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Additional;

    public class RegisterUserCommand : IExecutable
    {
        // RegisterUser <username> <password> <repeat-password> <email>
        public string Execute(string[] data)
        {
            string username = data[0];
            string password = data[1];
            string repeatPassword = data[2];
            string email = data[3];

            if (Session.User != null)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            if (password != repeatPassword)
            {
                throw new ArgumentException("Passwords do not match!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                bool userExists = context.Users.Any(u => u.Username == username);

                if (userExists)
                {
                    throw new InvalidOperationException($"Username {username} is already taken!");
                }

                User user = new User
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    IsDeleted = false,
                    RegisteredOn = DateTime.Now,
                    LastTimeLoggedIn = DateTime.Now
                };

            
                context.Users.Add(user);
                context.SaveChanges();
            }

            return "User " + username + " was registered successfully!";
        }
    }
}
