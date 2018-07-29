namespace TeamBuilder.App.Core.Commands
{
    using Contracts;
    using Utilities;
    using TeamBuilder.Models;

    public class LogoutCommand : IExecutable
    {
        // Logout

        public string Execute(string[] args)
        {
            Checker.CheckLength(0, args);

            User user = AuthenticationManager.GetCurrentUser();

            AuthenticationManager.Logout();

            return string.Format(Constants.SuccessMessages.SuccessfulLogout, user.Username);
        }
    }
}
