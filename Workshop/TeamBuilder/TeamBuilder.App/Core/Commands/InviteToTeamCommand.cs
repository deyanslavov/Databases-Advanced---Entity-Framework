namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class InviteToTeamCommand : IExecutable
    {
        // • InviteToTeam <teamName> <username>

        public string Execute(string[] args)
        {
            Checker.CheckLength(2, args);

            User currentUser = AuthenticationManager.GetCurrentUser();

            string teamName = args[0];
            string userName = args[1];

            if (!Checker.IsMemberOfTeam(teamName, currentUser.Username) || Checker.IsMemberOfTeam(teamName, userName))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.NotAllowed);
            }

            if (!Checker.IsTeamExisting(teamName) || !Checker.IsUserExisting(userName))
            {
                throw new ArgumentException(Constants.ErrorMessages.TeamOrUserNotExist);
            }

            User invitedUser = ContextHelper.GetUserByUsername(userName);

            if (Checker.IsInviteExisting(teamName, invitedUser))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.InviteIsAlreadySent);
            }

            bool isUserCreatorOfTheTeam = Checker.IsUserCreatorOfTeam(teamName, currentUser);

            string result = null;

            if (isUserCreatorOfTheTeam)
            {
                result = ContextHelper.AddToTeam(teamName, invitedUser);
            }
            else
            {
                result = ContextHelper.SendInvitation(teamName, invitedUser);
            }

            return result;
        }
    }
}
