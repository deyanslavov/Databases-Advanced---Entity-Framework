namespace Instagraph.DataProcessor
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;

    using Newtonsoft.Json;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;

    using Instagraph.Data;
    using Instagraph.Models;
    using Instagraph.DataProcessor.Dtos.Import;
    using System.Xml.Serialization;
    using System.IO;

    public class Deserializer
    {
        private const string SuccessfullyImportedPicturesMessage = "Successfully imported Picture {0}.";
        private const string SuccessfullyImportedUsersMessage = "Successfully imported User {0}.";
        private const string SuccessfullyImportedPostsMessage = "Successfully imported Post {0}.";
        private const string SuccessfullyImportedCommentsMessage = "Successfully imported Comment {0}.";
        private const string SuccessfullyImportedUsersFollowersMessage = "Successfully imported Follower {0} to User {1}.";
        private const string ErrorMessage = "Error: Invalid data.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var deserializedPictures = JsonConvert.DeserializeObject<PictureDto[]>(jsonString);

            var validPictures = new List<Picture>();

            var sb = new StringBuilder();

            foreach (var picDto in deserializedPictures)
            {
                if (!IsValid(picDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool pathExists = validPictures.Any(a => a.Path == picDto.Path);
                if (pathExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }

                Picture picture = new Picture
                {
                    Path = picDto.Path,
                    Size = picDto.Size,
                };

                validPictures.Add(picture);

                sb.AppendLine(string.Format(SuccessfullyImportedPicturesMessage, picture.Path));
            }

            context.Pictures.AddRange(validPictures);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var deserializedUsers = JsonConvert.DeserializeObject<UserDto[]>(jsonString);

            var validUsers = new List<User>();

            var sb = new StringBuilder();

            foreach (var userDto in deserializedUsers)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool usernameExists = validUsers.Any(u => u.Username == userDto.Username);
                bool pictureExists = context.Pictures.Any(p => p.Path == userDto.ProfilePicture);

                if (usernameExists || !pictureExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                int? pictureId = context.Pictures.AsNoTracking().SingleOrDefault(p => p.Path == userDto.ProfilePicture)?.Id;

                User user = new User
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    ProfilePictureId = pictureId.Value,
                };

                validUsers.Add(user);

                sb.AppendLine(string.Format(SuccessfullyImportedUsersMessage, user.Username));
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var deserializedUserFollowers = JsonConvert.DeserializeObject<UserFollowerDto[]>(jsonString);

            var validUserFollowers = new List<UserFollower>();

            var sb = new StringBuilder();

            foreach (var ufDto in deserializedUserFollowers)
            {
                if (!IsValid(ufDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool userExists = context.Users.Any(u => u.Username == ufDto.User);
                bool followerExists = context.Users.Any(u => u.Username == ufDto.Follower);

                if (!userExists || !followerExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                User user = FindUser(context, ufDto.User);
                User follower = FindUser(context, ufDto.Follower);

                UserFollower userFollower = new UserFollower
                {
                    User = user,
                    Follower = follower,
                };

                bool userFollowerExists = validUserFollowers.Any(a => a.Follower == follower && a.User == user);

                if (userFollowerExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validUserFollowers.Add(userFollower);

                sb.AppendLine(string.Format(SuccessfullyImportedUsersFollowersMessage, follower.Username, user.Username));
            }

            context.UsersFollowers.AddRange(validUserFollowers);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(PostDto[]), new XmlRootAttribute("posts"));

            var deserializedPosts = (PostDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var validPosts = new List<Post>();

            var sb = new StringBuilder();

            foreach (var postDto in deserializedPosts)
            {
                if (!IsValid(postDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool userExists = context.Users.Any(u => u.Username == postDto.User);
                bool pictureExist = context.Pictures.Any(p => p.Path == postDto.Picture);

                if (!userExists || !pictureExist)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                User user = FindUser(context, postDto.User);
                Picture picture = FindPicture(context, postDto.Picture);

                Post post = new Post
                {
                    User = user,
                    Picture = picture,
                    Caption = postDto.Caption,
                };

                validPosts.Add(post);

                sb.AppendLine(string.Format(SuccessfullyImportedPostsMessage, post.Caption));
            }

            context.Posts.AddRange(validPosts);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);
            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();
            var validComments = new List<Comment>();

            foreach (var el in elements)
            {
                string content = el.Element("content")?.Value;
                string username = el.Element("user")?.Value;
                bool postIdExists = int.TryParse(el.Element("post")?.Attribute("id")?.Value, out int postId);

                bool contentIsValid = content.Length <= 250;
                bool userExists = context.Users.Any(u => u.Username == username);
                bool postExists = context.Posts.Any(p => p.Id == postId);

                if (!contentIsValid || !userExists || !postExists || !postIdExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                User user = FindUser(context, username);

                Comment comment = new Comment
                {
                    Content = content,
                    PostId = postId,
                    User = user,
                };

                validComments.Add(comment);

                sb.AppendLine(string.Format(SuccessfullyImportedCommentsMessage, comment.Content));
            }

            context.Comments.AddRange(validComments);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }

        private static User FindUser(InstagraphContext context, string username)
        {
            return context.Users.SingleOrDefault(u => u.Username == username);
        }

        private static Picture FindPicture(InstagraphContext context, string path)
        {
            return context.Pictures.SingleOrDefault(p => p.Path == path);
        }
    }
}
