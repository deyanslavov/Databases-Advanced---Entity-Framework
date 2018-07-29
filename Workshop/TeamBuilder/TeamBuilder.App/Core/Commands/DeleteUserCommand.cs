namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class DeleteUserCommand : IExecutable
    {
        // •	DeleteUser

        public string Execute(string[] args)
        {
            if (!AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException(Constants.ErrorMessages.LoginFirst);
            }

            User user = AuthenticationManager.GetCurrentUser();

            AuthenticationManager.Logout();

            ContextHelper.DeleteUser(user);

            return string.Format(Constants.SuccessMessages.SuccessfullyDeletedUser, user.Username);
        }
    }
}
