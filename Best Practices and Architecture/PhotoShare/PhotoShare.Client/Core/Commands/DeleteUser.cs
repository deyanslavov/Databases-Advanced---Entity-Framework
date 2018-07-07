namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Data;
    using PhotoShare.Models;
    using Contracts;
    using PhotoShare.Client.Additional;

    public class DeleteUser : IExecutable
    {
        // DeleteUser <username>
        public string Execute(string[] data)
        {
            string username = data[0];

            if (Session.User == null || Session.User.Username != username)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                User user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }

                bool userAlreadyDeleted = user.IsDeleted.Value;

                if (userAlreadyDeleted)
                {
                    throw new InvalidOperationException($"User {username} is already deleted!");
                }

                context.Users.Remove(user);

                context.SaveChanges();

                return $"User {username} was deleted successfully!";
            }
        }
    }
}
