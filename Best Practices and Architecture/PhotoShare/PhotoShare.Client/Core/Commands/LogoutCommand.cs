namespace PhotoShare.Client.Core.Commands
{
    using Contracts;
    using PhotoShare.Client.Additional;
    using System;

    public class LogoutCommand : IExecutable
    {
        public string Execute(params string[] args)
        {
            if (Session.User == null)
            {
                throw new InvalidOperationException("You should log in first in order to logout.");
            }

            string username = Session.User.Username;

            Session.User = null;

            return $"User {username} successfully logged out!";
        }
    }
}
