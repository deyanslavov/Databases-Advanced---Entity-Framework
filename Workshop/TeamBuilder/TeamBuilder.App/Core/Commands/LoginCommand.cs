namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class LoginCommand : IExecutable
    {
        // •	Login <username> <password>

        public string Execute(string[] args)
        {
            Checker.CheckLength(2, args);

            string username = args[0];
            string password = args[1];

            if (AuthenticationManager.IsAuthenticated())
            {
                throw new InvalidOperationException(Constants.ErrorMessages.LogoutFirst);
            }

            User user = ContextHelper.GetUserByCredentials(username, password);

            AuthenticationManager.Login(user);

            return string.Format(Constants.SuccessMessages.SuccessfulLogin, user.Username);
        }
    }
}
