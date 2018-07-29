namespace Instagraph.DataProcessor
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Instagraph.DataProcessor.Dtos.Export;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var comments = context.Posts
                                  .Include(p => p.Picture)
                                  .Include(p => p.User)
                                  .Include(p => p.Comments)
                                  .Where(p => p.Comments.Count == 0)
                                  .OrderBy(p => p.Id)
                                  .Select(p => new
                                  {
                                      p.Id,
                                      Picture = p.Picture.Path,
                                      User = p.User.Username,
                                  })
                                  .ToArray();

            var json = JsonConvert.SerializeObject(comments, Formatting.Indented);
            return json;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var users = context.Users
                .Where(u => u.Posts
                    .Any(p => p.Comments
                        .Any(c => u.Followers
                            .Any(f => f.FollowerId == c.UserId))))
                .OrderBy(u => u.Id)
                .ProjectTo<PopularUserDto>()
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(users, Formatting.Indented);

            return jsonString;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var users = context.Users
                               .Include(u => u.Posts)
                               .Include(u => u.Comments)
                               .Select(u => new 
                               {
                                   u.Username,
                                   Post = u.Posts.OrderByDescending(p => p.Comments.Count).FirstOrDefault()
                               })
                               .Select(u => new
                               {
                                   u.Username,
                                   MostComments = u.Post == null ? 0 : u.Post.Comments.Count
                               })
                               .OrderByDescending(u => u.MostComments)
                               .ThenBy(u => u.Username)
                               .ToArray();

            XDocument xDocument = new XDocument(new XElement("users"));

            foreach (var u in users)
            {
                var userInfo = new XElement("user",
                                new XElement("Username", u.Username),
                                new XElement("MostComments", u.MostComments));

                xDocument.Root.Add(userInfo);
            }

            string result = xDocument.ToString();
            return result;
        }
    }
}
