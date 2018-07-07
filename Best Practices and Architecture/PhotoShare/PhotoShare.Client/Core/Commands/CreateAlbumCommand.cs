namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Client.Additional;
    using PhotoShare.Client.Contracts;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CreateAlbumCommand : IExecutable
    {
        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>

        public string Execute(params string[] args)
        {
            string username = args[0];
            string albumTitle = args[1];
            string bgColor = args[2];
            string[] tagStrings = args.Skip(3).ToArray();

            if (Session.User == null || Session.User.Username != username)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            using (var db = new PhotoShareContext())
            {
                #region Checks

                if (!Checker.UserExists(username))
                {
                    throw new ArgumentException($"User {username} not found!");
                }

                if (Checker.AlbumExists(albumTitle))
                {
                    throw new ArgumentException($"Album {albumTitle} exists!");
                }

                if (!Checker.ColorExists(bgColor))
                {
                    throw new ArgumentException($"Color {bgColor} not found!");
                }

                if (!Checker.TagsExists(tagStrings))
                {
                    throw new ArgumentException("Invalid tags!");
                }

                #endregion

                User user = db.Users.FirstOrDefault(u => u.Username == username);
                Color color = Enum.Parse<Color>(bgColor, true);
                Album album = new Album() { Name = albumTitle, BackgroundColor = color };

                db.Albums.Add(album);
                db.SaveChanges();
                
                List<AlbumTag> tags = new List<AlbumTag>();

                foreach (var t in tagStrings)
                {
                    int tagId = db.Tags.FirstOrDefault(tg => tg.Name == t).Id;
                    AlbumTag tag = new AlbumTag()
                    {
                        AlbumId = album.Id,
                        TagId = tagId,
                    };

                    tags.Add(tag);
                }

                album.AlbumTags = tags;

                AlbumRole albumRole = new AlbumRole()
                {
                    AlbumId = album.Id,
                    UserId = user.Id,
                };

                user.AlbumRoles.Add(albumRole);

                db.SaveChanges();

                return $"Album {album.Name} successfully created!";
            }
        }
    }
}
