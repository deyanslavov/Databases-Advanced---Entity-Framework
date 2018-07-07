namespace PhotoShare.Client.Core.Commands
{
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Client.Additional;
    using PhotoShare.Client.Contracts;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;
    using System.Text;

    public class PrintFriendsListCommand : IExecutable
    {
        // PrintFriendsList <username>
        public string Execute(string[] data)
        {
            string username = data[0];

            if (!Checker.UserExists(username))
            {
                throw new ArgumentException($"User {username} not found!");
            }

            StringBuilder sb = new StringBuilder();

            using (var db = new PhotoShareContext())
            {
                var userFriends = db.Users
                    .Include(u => u.FriendsAdded)
                    .ThenInclude(f => f.Friend)
                    .FirstOrDefault(u => u.Username == username)
                    .FriendsAdded
                    .ToArray();

                foreach (var friend in userFriends.OrderBy(f => f.Friend.FirstName))
                {
                    sb.AppendLine(friend.Friend.Username);
                }
            }

            string result = sb.ToString().Trim();
            return result;
        }
    }
}
