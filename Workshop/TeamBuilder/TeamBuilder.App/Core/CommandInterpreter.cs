namespace TeamBuilder.App.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Utilities;
    using Contracts;

    public class CommandInterpreter
    {
        public IExecutable ParseCommand(string commandName)
        {
            Type commandType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(c => c.Name.ToLower() == commandName.ToLower() + "command");

            if (commandType == null)
            {
                throw new NotSupportedException(string.Format(Constants.ErrorMessages.CommandNotSupported, commandName));
            }

            IExecutable command = (IExecutable)Activator.CreateInstance(commandType);
            return command;
        }
    }
}
