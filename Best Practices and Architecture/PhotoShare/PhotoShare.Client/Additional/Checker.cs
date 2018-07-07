namespace PhotoShare.Client.Additional
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public static class Checker
    {
        public static bool UserExists(string username)
        {
            using (var db = new PhotoShareContext())
            {
                return db.Users.Any(u => u.Username == username);
            }
        }

        public static bool AlbumExists(string albumTitle)
        {
            using (var db = new PhotoShareContext())
            {
                return db.Albums.Any(a => a.Name == albumTitle);
            }
        }

        public static bool AlbumExists(int albumId)
        {
            using (var db = new PhotoShareContext())
            {
                return db.Albums.Any(a => a.Id == albumId);
            }
        }

        public static bool ColorExists(string color)
        {
            return Enum.TryParse(typeof(Color), color, true, out object parsed);
        }

        public static bool TagsExists(string[] tags)
        {
            using (var db = new PhotoShareContext())
            {
                return tags.All(t => db.Tags.Any(tag => tag.Name == t));
            }
        }

        public static bool TagsExists(string tagName)
        {
            using (var db = new PhotoShareContext())
            {
                return db.Tags.Any(t => t.Name == tagName);
            }
        }

        public static bool PermissionIsValid(string permission)
        {
            return Enum.TryParse(typeof(Role), permission, true, out object parsed);
        }
    }
}
