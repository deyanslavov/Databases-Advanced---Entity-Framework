namespace TeamBuilder.App.Core
{
    using System;
    using System.Linq;
    using Contracts;

    public class Engine
    {
        private CommandInterpreter commandInterpreter;

        public Engine(CommandInterpreter commandInterpreter)
        {

            this.commandInterpreter = commandInterpreter;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    Console.Write("Please enter a command: ");
                    string[] input = Console.ReadLine().Split();

                    string commandName = input[0];
                    string[] args = input.Skip(1).ToArray();

                    IExecutable command = this.commandInterpreter.ParseCommand(commandName);

                    string result = command.Execute(args);

                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
