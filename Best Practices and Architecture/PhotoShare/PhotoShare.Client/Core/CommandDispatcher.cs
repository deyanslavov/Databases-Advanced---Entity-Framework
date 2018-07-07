namespace PhotoShare.Client.Core
{
    using PhotoShare.Client.Contracts;
    using System;
    using System.Linq;
    using System.Reflection;

    public class CommandDispatcher
    {
        public IExecutable DispatchCommand(string[] commandParameters)
        {
            string commandName = commandParameters[0];

            Type commandType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Name.ToLower() == $"{commandName.ToLower()}command")
                .FirstOrDefault();

            if (commandType == null)
            {
                throw new Exception("The command you have entered is not a valid command!");
            }

            IExecutable command = (IExecutable)Activator.CreateInstance(commandType);
            return command;
        }
    }
}
