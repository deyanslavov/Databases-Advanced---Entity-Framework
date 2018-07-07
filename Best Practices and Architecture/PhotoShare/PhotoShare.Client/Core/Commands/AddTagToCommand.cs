namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Additional;
    using PhotoShare.Data;
    using PhotoShare.Models;

    public class AddTagToCommand : IExecutable
    {
        // AddTagTo <albumName> <tag>

        public string Execute(params string[] args)
        {
            string albumTitle = args[0];
            string tagName = args[1];

            if (Session.User == null || !Session.User.AlbumRoles.Any(ar => ar.Album.Name == albumTitle))
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            if (!Checker.AlbumExists(albumTitle) || !Checker.TagsExists(tagName))
            {
                throw new ArgumentException("Either tag or album do not exist!");
            }

            using (var db = new PhotoShareContext())
            {
                Album album = db.Albums.FirstOrDefault(a => a.Name == albumTitle);
                Tag tag = db.Tags.FirstOrDefault(t => t.Name == tagName);

                AlbumTag albumTag = new AlbumTag()
                {
                    Album = album,
                    Tag = tag,
                };

                album.AlbumTags.Add(albumTag);

                db.SaveChanges();
            }

            return $"Tag {tagName} added to {albumTitle}!";
        }
    }
}
