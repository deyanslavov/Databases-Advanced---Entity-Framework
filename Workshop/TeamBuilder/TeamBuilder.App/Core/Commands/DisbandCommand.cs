namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class DisbandCommand : IExecutable
    {
        // •	Disband <teamName>

        public string Execute(string[] args)
        {
            Checker.CheckLength(1, args);

            User currentUser = AuthenticationManager.GetCurrentUser();

            string teamName = args[0];

            if (!Checker.IsTeamExisting(teamName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.TeamNotFound, teamName));
            }

            if (!Checker.IsUserCreatorOfTeam(teamName, currentUser))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.NotAllowed);
            }

            ContextHelper.DisbandTeam(teamName);

            string result = string.Format(Constants.SuccessMessages.SuccessfullyDisbandedTeam, teamName);
            return result;
        }
    }
}
