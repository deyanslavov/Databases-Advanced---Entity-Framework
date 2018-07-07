namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Additional;
    using PhotoShare.Data;
    using PhotoShare.Models;

    public class ShareAlbumCommand : IExecutable
    {
        // ShareAlbum <albumId> <username> <permission>
        // For example:
        // ShareAlbum 4 dragon321 Owner
        // ShareAlbum 4 dragon11 Viewer
        public string Execute(params string[] args)
        {
            int albumId = int.Parse(args[0]);
            string username = args[1];
            string permission = args[2];

            if (Session.User == null || Session.User.Username != username)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            if (!Checker.AlbumExists(albumId))
                throw new ArgumentException($"Album {albumId} not found!");

            if (!Checker.UserExists(username))
                throw new ArgumentException($"User {username} not found!");

            if (!Checker.PermissionIsValid(permission))
                throw new ArgumentException("Permission must be either “Owner” or “Viewer”!");

            Role role = (Role)Enum.Parse(typeof(Role), permission);

            using (var db = new PhotoShareContext())
            {
                User user = db.Users.FirstOrDefault(u => u.Username == username);
                Album album = db.Albums.Find(albumId);

                AlbumRole albumRole = new AlbumRole()
                {
                    Album = album,
                    Role = role,
                    User = user,
                };

                db.AlbumRoles.Add(albumRole);
                db.SaveChanges();

                return $"Username {username} added to album {album.Name} ({permission})";
            }
        }
    }
}
