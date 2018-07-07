namespace PhotoShare.Client.Core.Commands
{
    using Models;
    using Data;
    using Utilities;
    using System.Linq;
    using System;
    using Contracts;
    using PhotoShare.Client.Additional;

    public class AddTagCommand : IExecutable
    {
        // AddTag <tag>
        public string Execute(string[] data)
        {
            string tagName = data[0].ValidateOrTransform();

            if (Session.User == null)
            {
                throw new InvalidOperationException("Invalid credentials!");
            }

            using (PhotoShareContext context = new PhotoShareContext())
            {
                bool tagExists = context.Tags.Any(t => t.Name == tagName);

                if (tagExists)
                {
                    throw new ArgumentException($"Tag {tagName} exists!");
                }

                context.Tags.Add(new Tag
                {
                    Name = tagName
                });

                context.SaveChanges();
            }

            return tagName + " was added successfully to database!";
        }
    }
}
