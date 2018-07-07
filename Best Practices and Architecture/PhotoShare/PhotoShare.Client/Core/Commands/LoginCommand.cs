namespace PhotoShare.Client.Core.Commands
{
    using Contracts;
    using PhotoShare.Client.Additional;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class LoginCommand : IExecutable
    {
        public string Execute(params string[] args)
        {
            string username = args[0];
            string password = args[1];

            if (Session.User != null)
            {
                throw new ArgumentException("You should logout first!");
            }


            using (var db = new PhotoShareContext())
            {
                User user = db.Users.FirstOrDefault(u => u.Username == username);

                bool passwordMatches = user?.Password == password;

                if (!Checker.UserExists(username) || !passwordMatches)
                {
                    throw new ArgumentException("Invalid username or password!");
                }

                Session.User = user;
            }

            return $"User {username} successfully logged in!";
        }
    }
}
