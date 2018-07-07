namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;

    using Contracts;
    using Microsoft.EntityFrameworkCore;
    using PhotoShare.Client.Additional;
    using PhotoShare.Data;
    using PhotoShare.Models;

    public class AcceptFriendCommand : IExecutable
    {
        // AcceptFriend <username1> <username2>
        public string Execute(string[] data)
        {
            string username1 = data[0];
            string username2 = data[1];

            if (Session.User == null)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            if (!Checker.UserExists(username1))
                throw new ArgumentException($"{username1} not found!");

            if (!Checker.UserExists(username2))
                throw new ArgumentException($"{username2} not found!");

            using (var db = new PhotoShareContext())
            {
                User user1 = db.Users.FirstOrDefault(u => u.Username == username1);
                User user2 = db.Users.Include(u => u.FriendsAdded).FirstOrDefault(u => u.Username == username2);

                bool alreadyFriends = user2.FriendsAdded.Any(u => u.Friend.FirstName == user1.FirstName);

                if (alreadyFriends)
                {
                    throw new InvalidOperationException($"{username2} is already a friend to {username1}");
                }

                bool isTheFriendAUserFriend = user2.FriendsAdded.Any(f => f.Friend == user1);

                if (!isTheFriendAUserFriend)
                {
                    throw new InvalidOperationException($"{username2} has not added {username1} as a friend");
                }

                var acceptFriendship = new Friendship()
                {
                    User = user1,
                    Friend = user2
                };

                user1.FriendsAdded.Add(acceptFriendship);
                db.SaveChanges();

                return $"{user2.Username} accepted {user1.Username} as a friend";
            }
        }
    }
}
