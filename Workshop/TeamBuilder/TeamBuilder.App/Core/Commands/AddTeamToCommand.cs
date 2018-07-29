namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class AddTeamToCommand : IExecutable
    {
        // •	AddTeamTo <eventName> <teamName>

        public string Execute(string[] args)
        {
            Checker.CheckLength(2, args);

            User currentUser = AuthenticationManager.GetCurrentUser();

            string eventName = args[0];
            string teamName = args[1];

            if (!Checker.IsEventExisting(eventName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.EventNotFound, eventName));
            }

            if (!Checker.IsTeamExisting(teamName))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.TeamNotFound, eventName));
            }

            if (!Checker.IsUserCreatorOfEvent(eventName, currentUser))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.NotAllowed);
            }

            if(Checker.IsTeamAlreadyAddedToEvent(teamName, eventName))
            {
                throw new InvalidOperationException(Constants.ErrorMessages.CannotAddSameTeamTwice);
            }

            ContextHelper.AddTeamToEvent(teamName, eventName);

            string result = string.Format(Constants.SuccessMessages.SuccessfullyAddedTeamToEvent, teamName, eventName);
            return result;
        }
    }
}
