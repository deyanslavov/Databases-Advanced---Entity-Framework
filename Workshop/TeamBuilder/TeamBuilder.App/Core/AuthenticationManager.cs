namespace TeamBuilder.App.Core
{
    using System;
    using Models;
    using Utilities;

    public static class AuthenticationManager
    {
        private static User currentUser;

        public static void Login(User user)
        {
            if (currentUser != null)
            {
                throw new InvalidOperationException(Constants.ErrorMessages.LogoutFirst);
            }

            currentUser = user;
        }

        public static void Logout()
        {
            if (currentUser == null)
            {
                throw new InvalidOperationException(Constants.ErrorMessages.LoginFirst);
            }

            currentUser = null;
        }

        public static bool IsAuthenticated()
        {
            return currentUser != null;
        }

        public static User GetCurrentUser()
        {
            if (!IsAuthenticated())
            {
                throw new InvalidOperationException(Constants.ErrorMessages.LoginFirst);
            }

            return currentUser;
        }
    }
}
