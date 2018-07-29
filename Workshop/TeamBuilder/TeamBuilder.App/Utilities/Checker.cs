namespace TeamBuilder.App.Utilities
{
    using System.Linq;
    using TeamBuilder.Data;
    using TeamBuilder.Models;

    using Microsoft.EntityFrameworkCore;
    using System;

    public static class Checker
    {
        public static void CheckLength(int expectedLength, string[] array)
        {
            if (array.Length != expectedLength)
            {
                throw new System.FormatException(Constants.ErrorMessages.InvalidArgumentsCount);
            }
        }

        public static bool IsTeamExisting(string teamName)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Teams.Any(t => t.Name.ToLower() == teamName.ToLower());
            }
        }

        public static bool IsUserExisting(string username)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Users.Any(u => u.Username.ToLower() == username.ToLower());
            }
        }

        public static bool IsInviteExisting(string teamName, User user)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Invitations.Any(i => i.Team.Name.ToLower() == teamName.ToLower() && i.InvitedUserId == user.Id && i.IsActive);
            }
        }

        public static bool IsTeamAlreadyAddedToEvent(string teamName, string eventName)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.EventsTeams
                    .Include(et => et.Team)
                    .Include(et => et.Event)
                    .Any(et => et.Team.Name == teamName && et.Event.Name == eventName);
            }
        }

        public static bool IsUserCreatorOfTeam(string teamName, User user)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Teams.Any(t => t.Name.ToLower() == teamName.ToLower() && t.Creator.Username.ToLower() == user.Username.ToLower());
            }
        }

        public static bool IsUserCreatorOfEvent(string eventName, User user)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Events.Any(e => e.Name.ToLower() == eventName.ToLower() && e.Creator.Username.ToLower() == user.Username.ToLower());
            }
        }

        public static bool IsMemberOfTeam(string teamName, string username)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Teams.Include(t => t.UserTeams).ThenInclude(ut => ut.User).Single(t => t.Name.ToLower() == teamName.ToLower())
                    .UserTeams.Any(ut => ut.User.Username.ToLower() == username.ToLower());
            }
        }

        public static bool IsEventExisting(string eventName)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                return context.Events.Any(e => e.Name.ToLower() == eventName.ToLower());
            }
        }

        public static bool IsAcronymValid(string acronym)
        {
            return acronym.Length == 3;
        }
    }
}
