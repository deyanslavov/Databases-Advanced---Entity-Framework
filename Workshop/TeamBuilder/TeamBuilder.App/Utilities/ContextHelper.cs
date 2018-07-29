namespace TeamBuilder.App.Utilities
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Text;
    using TeamBuilder.Models;

    public static class ContextHelper
    {
        public static void AddEntity<TEntity>(object entity)
            where TEntity : class, new()
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                context.Add((TEntity)entity);
                context.SaveChanges();
            }
        }

        public static string ShowTeam(string teamName)
        {
            StringBuilder sb = new StringBuilder();

            Team team = GetTeamByTeamName(teamName);

            sb.AppendLine($"{team.Name} {team.Acronym}");
            sb.AppendLine("Members:");

            foreach (var u in team.UserTeams)
            {
                sb.AppendLine($"-{u.User.Username}");
            };

            return sb.ToString().Trim();
        }

        public static string ShowEvent(string eventName)
        {
            StringBuilder sb = new StringBuilder();

            Event @event = GetEventByName(eventName);

            sb.AppendLine($"{@event.Name} {@event.StartDate.ToString()} {@event.EndDate.ToString()}");
            sb.AppendLine($"{@event.Description}");
            sb.AppendLine("Teams:");

            foreach (var t in @event.ParticipatingEventTeams)
            {
                sb.AppendLine($"-{t.Team.Name}");
            };

            return sb.ToString().Trim();
        }

        public static User GetUserByUsername(string username)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                User user = context.Users.FirstOrDefault(u => u.Username == username);

                return user ?? throw new ArgumentException(Constants.ErrorMessages.UserOrPasswordIsInvalid);
            }
        }

        public static void DisbandTeam(string teamName)
        {
            using(TeamBuilderContext context = new TeamBuilderContext())
            {
                Team team = GetTeamByTeamName(teamName);

                Invitation[] invitations = context.Invitations
                                                  .Where(i => i.TeamId == team.Id)
                                                  .ToArray();

                context.Invitations.RemoveRange(invitations);

                User creator = GetUserByUsername(team.Creator.Username);
                creator.CreatedTeams.Remove(team);

                context.Users.Update(creator);
                context.Teams.Remove(team);

                context.SaveChanges();
            }
        }

        public static void AddTeamToEvent(string teamName, string eventName)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                Event @event = GetEventByName(eventName);
                Team team = GetTeamByTeamName(teamName);

                EventTeam eventTeam = new EventTeam()
                {
                    EventId = @event.Id,
                    TeamId = team.Id,
                };

                @event.ParticipatingEventTeams.Add(eventTeam);
                team.EventTeams.Add(eventTeam);

                context.EventsTeams.Add(eventTeam);

                context.SaveChanges();
            }
        }

        private static Event GetEventByName(string eventName)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                Event @event = context.Events
                                     .Include(e => e.ParticipatingEventTeams)
                                     .ThenInclude(et => et.Team)
                                     .Where(e => e.Name == eventName)
                                     .FirstOrDefault();
                return @event ?? throw new ArgumentException(string.Format(Constants.ErrorMessages.EventNotFound, eventName));
            }
        }

        public static void DeclineInvite(string teamName, User currentUser)
        {
            Invitation invitation = GetInvitationByTeamName(teamName, currentUser.Id);

            UpdateInvitation(invitation);
        }

        public static void AcceptInvite(string teamName, User currentUser)
        {
            Invitation invitation = GetInvitationByTeamName(teamName, currentUser.Id);
            
            UserTeam userTeam = new UserTeam()
            {
                TeamId = GetTeamByTeamName(teamName).Id,
                UserId = currentUser.Id,
            };

            UpdateInvitation(invitation);

            AddEntity<UserTeam>(userTeam);
        }

        public static void KickMember(string teamName, string userName)
        {
            Team team = GetTeamByTeamName(teamName);
            User user = GetUserByUsername(userName);

            RemoveUserFromTeam(team, user);
        }

        private static void RemoveUserFromTeam(Team team, User user)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                UserTeam userTeam = context.UsersTeams
                    .Where(ut => ut.TeamId == team.Id && ut.UserId == user.Id)
                    .FirstOrDefault();

                team.UserTeams.Remove(userTeam);
                context.UsersTeams.Remove(userTeam);

                context.SaveChanges();
            }
        }

        private static void UpdateInvitation(Invitation invitation)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                invitation.IsActive = false;
                context.Invitations.Update(invitation);

                context.SaveChanges();
            }
        }

        private static Invitation GetInvitationByTeamName(string teamName, int userId)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                Invitation invitation = context.Invitations.Include(i => i.Team)
                                               .FirstOrDefault(i => i.Team.Name.ToLower() == teamName.ToLower() && i.InvitedUserId == userId);
                
                return invitation ?? throw new ArgumentException(string.Format(Constants.ErrorMessages.InviteNotFound, teamName));
            }
        }

        public static User GetUserByCredentials(string username, string password)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                User user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                return user ?? throw new ArgumentException(Constants.ErrorMessages.UserOrPasswordIsInvalid);
            }
        }

        public static void DeleteUser(User user)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public static string SendInvitation(string teamName, User invitedUser)
        {
            Team team = GetTeamByTeamName(teamName);

            Invitation invitation = new Invitation()
            {
                InvitedUserId = invitedUser.Id,
                TeamId = team.Id,
            };

            invitedUser.ReceivedInvitations.Add(invitation);
            AddEntity<Invitation>(invitation);

            string result = string.Format(Constants.SuccessMessages.SuccessfullySentInvitation, teamName, invitedUser.Username);
            return result;
        }

        public static string AddToTeam(string teamName, User invitedUser)
        {
            Team team = GetTeamByTeamName(teamName);

            Invitation invitation = new Invitation()
            {
                InvitedUserId = invitedUser.Id,
                TeamId = team.Id,
                IsActive = false,
            };

            UserTeam userTeam = new UserTeam()
            {
                TeamId = team.Id,
                UserId = invitedUser.Id,
            };

            invitedUser.UserTeams.Add(userTeam);
            invitedUser.ReceivedInvitations.Add(invitation);
            AddEntity<Invitation>(invitation);
            AddEntity<UserTeam>(userTeam);

            string result = string.Format(Constants.SuccessMessages.SuccessfullyAddedToTeam, invitedUser.Username, teamName);
            return result;
        }

        private static Team GetTeamByTeamName(string teamName)
        {
            using (TeamBuilderContext context = new TeamBuilderContext())
            {
                Team team = context.Teams
                    .Include(t => t.UserTeams)
                    .ThenInclude(ut => ut.User)
                    .FirstOrDefault(t => t.Name == teamName);

                return team ?? throw new ArgumentException(string.Format(Constants.ErrorMessages.TeamNotFound, teamName));
            }
        }
    }
}
