namespace AutoMapper.Client.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    using AutoMapper.Client.Contracts;

    public class CommandInterpreter
    {
        private readonly IServiceProvider serviceProvider;

        public CommandInterpreter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ICommand ParseCommand(string commandName)
        {
            Type commandType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Name.ToLower() == $"{commandName.ToLower()}command")
                .FirstOrDefault();

            if (commandType == null)
            {
                throw new ArgumentException("Invalid command!");
            }

            ConstructorInfo constructor = commandType.GetConstructors().First();

            Type[] constructorParams = constructor.GetParameters()
                .Select(p => p.ParameterType).ToArray();

            var constructorArgs = constructorParams.Select(p => serviceProvider.GetService(p)).ToArray();

            ICommand command = (ICommand)constructor.Invoke(constructorArgs);

            return command;
        }
    }
}
