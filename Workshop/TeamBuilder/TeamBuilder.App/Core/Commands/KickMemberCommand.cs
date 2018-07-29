namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class KickMemberCommand : IExecutable
    {
        // •	KickMember <teamName> <username>

        public string Execute(string[] args)
        {
            Checker.CheckLength(2, args);

            User currentUser = AuthenticationManager.GetCurrentUser();

            string teamName = args[0];
            string userName = args[1];

            if (!Checker.IsTeamExisting(teamName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.TeamNotFound, teamName));
            }

            if (!Checker.IsUserExisting(userName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.UserNotFound, userName));
            }

            if (!Checker.IsMemberOfTeam(teamName, userName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.NotPartOfTeam, userName, teamName));
            }

            User userToBeKicket = ContextHelper.GetUserByUsername(userName);

            if(!Checker.IsUserCreatorOfTeam(teamName, currentUser))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.NotAllowed);
            }

            if (Checker.IsUserCreatorOfTeam(teamName, userToBeKicket))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.UseDisbandInstead);
            }

            ContextHelper.KickMember(teamName, userName);

            string result = string.Format(Constants.SuccessMessages.SuccessfullyKickedMember, userName, teamName);
            return result;
        }
    }
}
