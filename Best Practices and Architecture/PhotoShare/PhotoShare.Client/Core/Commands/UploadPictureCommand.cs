namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Additional;
    using PhotoShare.Data;
    using PhotoShare.Models;

    public class UploadPictureCommand : IExecutable
    {
        // UploadPicture <albumName> <pictureTitle> <pictureFilePath>
        public string Execute(string[] data)
        {
            string albumName = data[0];
            string pictureTitle = data[1];
            string picturePath = data[2];

            if (Session.User == null || Session.User.ProfilePicture.Path != picturePath)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            if (!Checker.AlbumExists(albumName))
                throw new ArgumentException($"Album {albumName} not found!");

            using (var db = new PhotoShareContext())
            {
                Album album = db.Albums.FirstOrDefault(a => a.Name == albumName);

                Picture picture = new Picture()
                {
                    Path = picturePath,
                    Title = pictureTitle,
                    Album = album,
                };

                db.Pictures.Add(picture);
                db.SaveChanges();
            }

            return $"Picture {pictureTitle} added to {albumName}!";
        }
    }
}
