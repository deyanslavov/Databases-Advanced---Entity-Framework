namespace AutoMapper.Client.Core
{
    using System;
    using System.Linq;
    using Contracts;

    public class Engine
    {
        private readonly IServiceProvider serviceProvider;

        public Engine(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Run()
        {
            try
            {
                while (true)
                {
                    Console.Write("Please enter a command: ");

                    string[] input = Console.ReadLine().Split();

                    string commandName = input[0];

                    CommandInterpreter commandInterpreter = (CommandInterpreter)this.serviceProvider.GetService(typeof(CommandInterpreter));

                    ICommand command = commandInterpreter.ParseCommand(commandName);

                    string result = command.Execute(input.Skip(1).ToArray());

                    Console.WriteLine(result);
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
