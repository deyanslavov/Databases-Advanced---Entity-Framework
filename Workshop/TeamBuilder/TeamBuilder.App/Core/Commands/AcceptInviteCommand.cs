namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class AcceptInviteCommand : IExecutable
    {
        // •	AcceptInvite <teamName>

        public string Execute(string[] args)
        {
            Checker.CheckLength(1, args);

            User currentUser = AuthenticationManager.GetCurrentUser();

            string teamName = args[0];

            if (!Checker.IsTeamExisting(teamName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.TeamNotFound, teamName));
            }

            if (!Checker.IsInviteExisting(teamName, currentUser))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.InviteNotFound, teamName));
            }

            ContextHelper.AcceptInvite(teamName, currentUser);

            string result = string.Format(Constants.SuccessMessages.SuccessfullyAcceptedInvitation, currentUser.Username,
                 teamName);
            return result;
        }
    }
}
