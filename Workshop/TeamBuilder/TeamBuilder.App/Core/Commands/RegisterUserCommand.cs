namespace TeamBuilder.App.Core.Commands
{
    using System;

    using Contracts;
    using Utilities;
    using TeamBuilder.Models;
    using System.Linq;
    using TeamBuilder.Data;

    public class RegisterUserCommand : IExecutable
    {
        //•	RegisterUser <username> <password> <repeat-password> <firstName> <lastName> <age> <gender>

        public string Execute(string[] args)
        {
            // Validate input
            Checker.CheckLength(7, args);

            string username = args[0];
            string password = args[1];
            string repeatPassword = args[2];
            string firstName = args[3];
            string lastName = args[4];
            bool isNumber = int.TryParse(args[5], out int age);
            bool genderIsValid = Enum.TryParse(typeof(Gender), args[6], out object gender);

            // Validate username
            if (username.Length < Constants.MinUsernameLength || username.Length > Constants.MaxUsernameLength)
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.UsernameNotValid, username));
            }

            // Validate password
            if (!password.Any(Char.IsDigit) || !password.Any(Char.IsUpper))
            {
                throw new ArgumentException(string.Format(Constants.ErrorMessages.PasswordNotValid, password));
            }

            // Validate passwords match
            if (password != repeatPassword)
            {
                throw new ArgumentException(Constants.ErrorMessages.PasswordDoesNotMatch);
            }

            // Validate age 
            if (!isNumber || age <= 0)
            {
                throw new ArgumentException(Constants.ErrorMessages.AgeNotValid);
            }

            // Validate gender
            if (!genderIsValid)
            {
                throw new ArgumentException(Constants.ErrorMessages.GenderNotValid);
            }

            // Check if username is already taken
            if (Checker.IsUserExisting(username))
            {
                throw new InvalidOperationException(string.Format(Constants.ErrorMessages.UsernameIsTaken, username));
            }

            this.RegisterUser(username, password, firstName, lastName, age, (Gender)gender);

            return string.Format(Constants.SuccessMessages.SuccessfullyRegisteredUser, username);
        }

        private void RegisterUser(string username,
                                  string password,
                                  string firstName,
                                  string lastName,
                                  int age,
                                  Gender gender)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                Gender = gender,
            };

            ContextHelper.AddEntity<User>(user);
        }
    }
}
