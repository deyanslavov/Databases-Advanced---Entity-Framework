namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class CreateTeamCommand : IExecutable
    {
        // •	CreateTeam <name> <acronym> <description>

        public string Execute(string[] args)
        {
            // Validate input
            if (args.Length != 3 && args.Length != 2)
            {
                throw new InvalidOperationException(Constants.ErrorMessages.InvalidArgumentsCount);
            }

            // Validate user
            User currentUser = AuthenticationManager.GetCurrentUser();

            string teamName = args[0];
            string acronym = args[1];
            string description = null;

            if (args.Length == 3)
            {
                description = args[2];
            }

            // Validate team
            if (Checker.IsTeamExisting(teamName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.TeamExists, teamName));
            }

            if (!Checker.IsAcronymValid(acronym))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.InvalidAcronym, acronym));
            }

            Team team = new Team()
            {
                Name = teamName,
                Acronym = acronym,
                Description = description,
                CreatorId = currentUser.Id,
            };

            ContextHelper.AddEntity<Team>(team);

            UserTeam userTeam = new UserTeam()
            {
                TeamId = team.Id,
                UserId = currentUser.Id,
            };
            
            ContextHelper.AddEntity<UserTeam>(userTeam);

            return string.Format(Constants.SuccessMessages.SuccessfullyCreatedTeam, teamName);
        }
    }
}
